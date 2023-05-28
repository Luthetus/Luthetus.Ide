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

    public ImmutableList<TextEditorTextSpan> DiagnosticTextSpans { get; set; } = ImmutableList<TextEditorTextSpan>.Empty;
    public ImmutableList<TextEditorTextSpan> SymbolTextSpans { get; private set; } = ImmutableList<TextEditorTextSpan>.Empty;

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

    public SemanticModelResultCSharp? ParseWithResult(
        TextEditorModel model)
    {
        var text = model.GetAllText();

        model.Lexer.Lex(
            text,
            model.RenderStateKey);

        var textEditorLexerC = (TextEditorLexerCSharp)model.Lexer;
        var recentLexSession = textEditorLexerC.RecentLexSession;

        if (recentLexSession is null)
            return null;

        var parserSession = new Parser(
            recentLexSession.SyntaxTokens,
            text,
            recentLexSession.Diagnostics);

        var compilationUnit = parserSession.Parse();

        DiagnosticTextSpans = compilationUnit.Diagnostics.Select(x =>
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

            return new TextEditorTextSpan(
                x.TextEditorTextSpan.StartingIndexInclusive,
                x.TextEditorTextSpan.EndingIndexExclusive,
                (byte)textEditorDecorationKind,
                model.ResourceUri,
                text);
        }).ToImmutableList();

        SymbolTextSpans = parserSession.Binder.Symbols
            .Select(x => x.TextSpan)
            .ToImmutableList();

        var semanticModelResult = new SemanticModelResultCSharp(
            text,
            parserSession,
            compilationUnit);

        _recentSemanticModelResult = semanticModelResult;

        return semanticModelResult;
    }
}
