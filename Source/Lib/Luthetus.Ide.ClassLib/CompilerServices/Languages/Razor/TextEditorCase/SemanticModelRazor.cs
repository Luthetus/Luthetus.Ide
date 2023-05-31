using Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
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

    public SymbolDefinition? GoToDefinition(
        TextEditorModel model,
        TextEditorTextSpan textSpan)
    {
        var semanticModelResult = ParseWithResult(model);

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
        var text = model.GetAllText();

        model.Lexer.Lex(
            text,
            model.RenderStateKey);

        var overriteTextEditorRazorLexer = (OverriteTextEditorRazorLexer)model.Lexer;

        if (overriteTextEditorRazorLexer.TEST_RazorSyntaxTree is null)
            return null;

        var recentResult = overriteTextEditorRazorLexer
            .TEST_RazorSyntaxTree
            .RecentResult;

        var parserSession = recentResult
            .Parser;

        var compilationUnit = recentResult
            .CompilationUnit;

        // Testing
        var resultingSymbols = new List<ISymbol>();

        foreach (var adhocSymbol in parserSession.Binder.Symbols)
        {
            var adhocTextInsertion = recentResult.AdhocClassInsertions
                .SingleOrDefault(x =>
                    adhocSymbol.TextSpan.StartingIndexInclusive >= x.InsertionStartingIndexInclusive &&
                    adhocSymbol.TextSpan.EndingIndexExclusive < x.InsertionEndingIndexExclusive);

            // TODO: Fix for spans that go 2 adhocTextInsertions worth of length?
            if (adhocTextInsertion is null)
            {
                adhocTextInsertion = recentResult.AdhocRenderFunctionInsertions
                    .SingleOrDefault(x =>
                        adhocSymbol.TextSpan.StartingIndexInclusive >= x.InsertionStartingIndexInclusive &&
                        adhocSymbol.TextSpan.EndingIndexExclusive < x.InsertionEndingIndexExclusive);
            }

            if (adhocTextInsertion is not null)
            {
                ISymbol? symbolToAdd = null;

                var symbolSourceTextStartingIndexInclusive =
                    adhocTextInsertion.SourceTextStartingIndexInclusive +
                    (adhocSymbol.TextSpan.StartingIndexInclusive - adhocTextInsertion.InsertionStartingIndexInclusive);

                var symbolSourceTextEndingIndexExclusive =
                    symbolSourceTextStartingIndexInclusive +
                    (adhocSymbol.TextSpan.EndingIndexExclusive - adhocSymbol.TextSpan.StartingIndexInclusive);

                switch (adhocSymbol.SyntaxKind)
                {
                    case SyntaxKind.TypeSymbol:
                        var sourceTextSpan = adhocSymbol.TextSpan with
                        {
                            ResourceUri = model.ResourceUri,
                            SourceText = text,
                            DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageType,
                            StartingIndexInclusive = symbolSourceTextStartingIndexInclusive,
                            EndingIndexExclusive = symbolSourceTextEndingIndexExclusive,
                        };

                        symbolToAdd = new TypeSymbol(sourceTextSpan);
                        break;
                    case SyntaxKind.FunctionSymbol:
                        break;
                    case SyntaxKind.VariableSymbol:
                        break;
                }

                if (symbolToAdd is not null)
                    resultingSymbols.Add(symbolToAdd);
            }
        }

        //DiagnosticTextSpanTuples = compilationUnit.Diagnostics.Select(x =>
        //{
        //    var textEditorDecorationKind = x.DiagnosticLevel switch
        //    {
        //        TextEditorDiagnosticLevel.Hint => TextEditorSemanticDecorationKind.DiagnosticHint,
        //        TextEditorDiagnosticLevel.Suggestion => TextEditorSemanticDecorationKind.DiagnosticSuggestion,
        //        TextEditorDiagnosticLevel.Warning => TextEditorSemanticDecorationKind.DiagnosticWarning,
        //        TextEditorDiagnosticLevel.Error => TextEditorSemanticDecorationKind.DiagnosticError,
        //        TextEditorDiagnosticLevel.Other => TextEditorSemanticDecorationKind.DiagnosticOther,
        //        _ => throw new NotImplementedException(),
        //    };

        //    var textSpan = x.TextEditorTextSpan with
        //    {
        //        DecorationByte = (byte)textEditorDecorationKind
        //    };

        //    return (x, textSpan);
        //}).ToImmutableList();

        SymbolMessageTextSpanTuples = resultingSymbols
            .Select(x => ($"({x.GetType().Name}){x.TextSpan.GetText()}", x.TextSpan))
            .ToImmutableList();

        var semanticModelResult = new SemanticModelResultRazor(
            text,
            parserSession,
            compilationUnit);

        _recentSemanticModelResult = semanticModelResult;

        return semanticModelResult;
    }
}
