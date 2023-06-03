using Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.BinderCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.Semantics;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.TextEditorCase;

public class SemanticModelCSharp : ISemanticModel
{
    private readonly Binder _sharedBinder;

    private SemanticResultCSharp? _semanticResult;

    public SemanticModelCSharp(Binder sharedBinder)
    {
        _sharedBinder = sharedBinder;
    }

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

    public SemanticResultCSharp? ParseWithResult(
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

        var compilationUnit = parserSession.Parse(_sharedBinder);

        var localSemanticResult = new SemanticResultCSharp(
            text,
            parserSession,
            compilationUnit);

        localSemanticResult = localSemanticResult with 
        {
            DiagnosticTextSpanTuples = compilationUnit.Diagnostics
                .Where(x => x.TextEditorTextSpan.ResourceUri == model.ResourceUri)
                .Select(x =>
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

        localSemanticResult = localSemanticResult with 
        {
            SymbolMessageTextSpanTuples = parserSession.Binder.Symbols
                .Where(x => x.TextSpan.ResourceUri == model.ResourceUri)
                .Select(x => ($"({x.GetType().Name}){x.TextSpan.GetText()}", x.TextSpan))
                .ToImmutableList()
        };

        _semanticResult = localSemanticResult;

        return localSemanticResult;
    }
}
