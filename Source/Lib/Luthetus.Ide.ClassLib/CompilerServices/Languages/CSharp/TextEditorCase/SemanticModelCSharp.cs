using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
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

        var sameFileBoundScopes = semanticModelResult.ParserSession.Binder.BoundScopes
            .Where(bs => bs.ResourceUri == model.ResourceUri)
            .ToArray();

        var boundScope = sameFileBoundScopes
            .Where(bs => bs.StartingIndexInclusive <= textSpan.StartingIndexInclusive &&
                         (bs.EndingIndexExclusive ?? int.MaxValue) >= textSpan.EndingIndexExclusive)
            // Get the closest scope
            .OrderBy(bs => textSpan.StartingIndexInclusive - bs.StartingIndexInclusive)
            .FirstOrDefault();

        if (boundScope is null)
            return null;

        var textSpanText = textSpan.GetText();

        while (boundScope.Parent is not null &&
               !boundScope.VariableDeclarationMap.ContainsKey(textSpanText) &&
               !boundScope.ClassDeclarationMap.ContainsKey(textSpanText) &&
               !boundScope.FunctionDeclarationMap.ContainsKey(textSpanText) &&
               !boundScope.TypeMap.ContainsKey(textSpanText))
        {
            boundScope = boundScope.Parent;
        }

        if (!boundScope.VariableDeclarationMap.ContainsKey(textSpanText) &&
            !boundScope.ClassDeclarationMap.ContainsKey(textSpanText) &&
            !boundScope.FunctionDeclarationMap.ContainsKey(textSpanText) &&
            !boundScope.TypeMap.ContainsKey(textSpanText))
        {
            return null;
        }

        // (2023-06-03) Symbols don't understand scope right? I was using symbols to find goto-definition across files and wasn't getting anywhere. Going to try and use the bound scope which contains the definition directly.
        {
            if (boundScope.ClassDeclarationMap.TryGetValue(
                textSpanText,
                out var boundClassDeclarationNode))
            {
                return new TextEditorSymbolDefinition(
                    boundClassDeclarationNode.IdentifierToken.TextSpan.ResourceUri,
                    boundClassDeclarationNode.IdentifierToken.TextSpan.StartingIndexInclusive);
            }
        }

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

        var textEditorLexerCSharp = (IdeCSharpLexer)model.Lexer;
        var recentLexSession = textEditorLexerCSharp.RecentLexSession;

        if (recentLexSession is null)
            return null;

        var parserSession = new Parser(
            recentLexSession.SyntaxTokens,
            recentLexSession.Diagnostics);

        CompilationUnit compilationUnit;

        compilationUnit = parserSession.Parse(
            _sharedBinder,
            model.ResourceUri);


        var localSemanticResult = new SemanticResultCSharp(
            text,
            parserSession,
            compilationUnit);

        localSemanticResult = localSemanticResult with 
        {
            DiagnosticTextSpanTuples = compilationUnit.Diagnostics
                .Where(x => x.TextSpan.ResourceUri == model.ResourceUri)
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

                    var textSpan = x.TextSpan with
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
