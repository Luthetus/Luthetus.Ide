using Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.Semantics;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.TextEditorCase;

public class SemanticModelCSharp : ISemanticModel
{
    private SemanticModelResultCSharp? _recentSemanticModelResult;
    private SemanticResult _semanticResult = new();

    public ISemanticResult? SemanticResult => _semanticResult;

    public TextEditorSymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan)
    {
        var semanticModelResult = ParseWithResult(model);

        if (semanticModelResult is null)
            return null;

        var boundScope = semanticModelResult.ParserSession.Binder.BoundScopes
            .Where(bs => bs.StartingIndexInclusive <= textSpan.StartingIndexInclusive &&
                         (bs.EndingIndexExclusive ?? int.MaxValue) >= textSpan.EndingIndexExclusive)
            // Get the closest scope
            .OrderBy(bs => textSpan.StartingIndexInclusive - bs.StartingIndexInclusive)
            .FirstOrDefault();

        if (boundScope is null)
            return null;

        var textSpanText = textSpan.GetText();

        while (boundScope.Parent is not null &&
               !boundScope.VariableDeclarationMap.ContainsKey(textSpanText))
        {
            boundScope = boundScope.Parent;
        }

        if (!boundScope.VariableDeclarationMap.ContainsKey(textSpanText))
            return null;

        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            textSpanText,
            boundScope.BoundScopeKey);

        if (semanticModelResult.ParserSession.Binder.SymbolDefinitions.TryGetValue(
                symbolDefinitionId,
                out var symbolDefinition))
        {
            return new TextEditorSymbolDefinition(
                symbolDefinition.Symbol.TextSpan.ResourceUri,
                symbolDefinition.Symbol.TextSpan.StartingIndexInclusive);
        }

        return null;
    }

    public void Parse(
        TextEditorModel model)
    {
        _ = ParseWithResult(model);
    }

    public SemanticModelResultCSharp? ParseWithResult(
        TextEditorModel model)
    {
        var text = model.GetAllText();

        model.Lexer.Lex(
            text,
            model.RenderStateKey);

        var textEditorLexerC = (IdeCSharpLexer)model.Lexer;
        var recentLexSession = textEditorLexerC.RecentLexSession;

        if (recentLexSession is null)
            return null;

        var parserSession = new Parser(
            recentLexSession.SyntaxTokens,
            recentLexSession.Diagnostics);

        var compilationUnit = parserSession.Parse();

        _semanticResult = _semanticResult with 
        {
            DiagnosticTextSpanTuples = compilationUnit.Diagnostics.Select(x =>
            {
                var textEditorDecorationKind = x.DiagnosticLevel switch
                {
                    TextEditorDiagnosticLevel.Hint => TextEditorSemanticDecorationKind.DiagnosticHint,
                    TextEditorDiagnosticLevel.Suggestion => TextEditorSemanticDecorationKind.DiagnosticSuggestion,
                    TextEditorDiagnosticLevel.Warning => TextEditorSemanticDecorationKind.DiagnosticWarning,
                    TextEditorDiagnosticLevel.Error => TextEditorSemanticDecorationKind.DiagnosticError,
                    TextEditorDiagnosticLevel.Other => TextEditorSemanticDecorationKind.DiagnosticOther,
                    _ => throw new NotImplementedException(),
                };

                var textSpan = x.TextEditorTextSpan with
                {
                    DecorationByte = (byte)textEditorDecorationKind
                };

                return (x, textSpan);
            }).ToImmutableList()
        };

        _semanticResult = _semanticResult with 
        {
            SymbolMessageTextSpanTuples = parserSession.Binder.Symbols
                .Select(x => ($"({x.GetType().Name}){x.TextSpan.GetText()}", x.TextSpan))
                .ToImmutableList()
        };

        var semanticModelResult = new SemanticModelResultCSharp(
            text,
            parserSession,
            compilationUnit);

        _recentSemanticModelResult = semanticModelResult;

        return semanticModelResult;
    }
}
