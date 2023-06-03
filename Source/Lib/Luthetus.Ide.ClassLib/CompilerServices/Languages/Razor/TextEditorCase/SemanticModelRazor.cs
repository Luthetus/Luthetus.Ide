using Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.BinderCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Analysis.Html.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.Semantics;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.TextEditorCase;

public class SemanticModelRazor : ISemanticModel
{
    private SemanticResultRazor? _semanticResult;

    public ISemanticResult? SemanticResult => _semanticResult;

    public TextEditorSymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan)
    {
        var localSemanticResult = ParseWithResult(model);

        if (localSemanticResult is null)
            return null;

        var boundScope = localSemanticResult.Parser.Binder.BoundScopes
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

        if (localSemanticResult.Parser.Binder.SymbolDefinitions.TryGetValue(
                symbolDefinitionId,
                out var symbolDefinition))
        {
            var sourceTextSpan = localSemanticResult.MapAdhocCSharpTextSpanToSource(
                model.ResourceUri,
                model.GetAllText(),
                symbolDefinition.Symbol.TextSpan);

            if (sourceTextSpan is null)
                return null;

            return new TextEditorSymbolDefinition(
                sourceTextSpan.ResourceUri,
                sourceTextSpan.StartingIndexInclusive);
        }

        return null;
    }

    public void Parse(
        TextEditorModel model)
    {
        _ = ParseWithResult(model);
    }

    public SemanticResultRazor? ParseWithResult(
        TextEditorModel model)
    {
        var modelText = model.GetAllText();

        model.Lexer.Lex(
            modelText,
            model.RenderStateKey);

        var overriteTextEditorRazorLexer = (IdeRazorLexer)model.Lexer;

        if (overriteTextEditorRazorLexer.IdeRazorSyntaxTree is null)
            return null;

        overriteTextEditorRazorLexer.IdeRazorSyntaxTree.ParseAdhocCSharpClass();

        var localSemanticResult = overriteTextEditorRazorLexer
            .IdeRazorSyntaxTree
            .RecentResult;

        if (localSemanticResult is null)
            return null;

        var parserSession = localSemanticResult
            .Parser;

        var compilationUnit = localSemanticResult
            .CompilationUnit;

        // Testing
        var resultingSymbols = new List<ISymbol>();

        foreach (var adhocSymbol in parserSession.Binder.Symbols)
        {
            var sourceTextSpan = localSemanticResult.MapAdhocCSharpTextSpanToSource(
                model.ResourceUri,
                modelText,
                adhocSymbol.TextSpan);

            if (sourceTextSpan is null)
                continue;
            
            ISymbol? symbolToAdd = null;

            switch (adhocSymbol.SyntaxKind)
            {
                case SyntaxKind.TypeSymbol:
                    sourceTextSpan = sourceTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageType,
                    };

                    symbolToAdd = new TypeSymbol(sourceTextSpan);
                    break;
                case SyntaxKind.FunctionSymbol:
                    sourceTextSpan = sourceTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageMethod,
                    };

                    symbolToAdd = new FunctionSymbol(sourceTextSpan);
                    break;
                case SyntaxKind.VariableSymbol:
                    sourceTextSpan = sourceTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageVariable,
                    };

                    symbolToAdd = new VariableSymbol(sourceTextSpan);
                    break;
            }

            if (symbolToAdd is not null)
                resultingSymbols.Add(symbolToAdd);
        }

        localSemanticResult = localSemanticResult with 
        {
            SymbolMessageTextSpanTuples = resultingSymbols
                .Select(x => ($"({x.GetType().Name}){x.TextSpan.GetText()}", x.TextSpan))
                .ToImmutableList()
        };

        _semanticResult = localSemanticResult;
        return localSemanticResult;
    }
}
