using Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Analysis.Html.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.Semantics;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.TextEditorCase;

public class SemanticModelRazor : ISemanticModel
{
    private SemanticModelResultRazor? _recentSemanticModelResult;

    public ImmutableList<(TextEditorDiagnostic diagnostic, TextEditorTextSpan textSpan)> DiagnosticTextSpanTuples { get; private set; } = ImmutableList<(TextEditorDiagnostic diagnostic, TextEditorTextSpan textSpan)>.Empty;
    public ImmutableList<(string message, TextEditorTextSpan textSpan)> SymbolMessageTextSpanTuples { get; private set; } = ImmutableList<(string message, TextEditorTextSpan textSpan)>.Empty;

    public TextEditorSymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan)
    {
        _ = ParseWithResult(model);

        if (_recentSemanticModelResult is null)
            return null;

        var boundScope = _recentSemanticModelResult.Parser.Binder.BoundScopes
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

        if (_recentSemanticModelResult.Parser.Binder.SymbolDefinitions.TryGetValue(
                symbolDefinitionId,
                out var symbolDefinition))
        {
            var sourceTextSpan = _recentSemanticModelResult.MapAdhocCSharpTextSpanToSource(
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

    public SemanticModelResultRazor? ParseWithResult(
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

        _recentSemanticModelResult = overriteTextEditorRazorLexer
            .IdeRazorSyntaxTree
            .RecentResult;

        if (_recentSemanticModelResult is null)
            return null;

        var parserSession = _recentSemanticModelResult
            .Parser;

        var compilationUnit = _recentSemanticModelResult
            .CompilationUnit;

        // Testing
        var resultingSymbols = new List<ISymbol>();

        foreach (var adhocSymbol in parserSession.Binder.Symbols)
        {
            var sourceTextSpan = _recentSemanticModelResult.MapAdhocCSharpTextSpanToSource(
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

        SymbolMessageTextSpanTuples = resultingSymbols
            .Select(x => ($"({x.GetType().Name}){x.TextSpan.GetText()}", x.TextSpan))
            .ToImmutableList();

        return null;
    }
}
