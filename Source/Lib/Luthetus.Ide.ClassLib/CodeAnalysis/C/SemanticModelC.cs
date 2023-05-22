using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.Semantics;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C;

public class SemanticModelC : ISemanticModel
{
    private SemanticModelResult? _recentSemanticModelResult;

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

    public SemanticModelResult? ParseWithResult(
        TextEditorModel model)
    {
        var text = model.GetAllText();

        model.Lexer.Lex(
            text,
            model.RenderStateKey);

        var textEditorLexerC = (TextEditorLexerC)model.Lexer;
        var recentLexSession = textEditorLexerC.RecentLexSession;

        if (recentLexSession is null)
            return null;

        var parserSession = new ParserSession(
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
                (byte)textEditorDecorationKind);
        }).ToImmutableList();

        SymbolTextSpans = parserSession.Binder.Symbols
            .Select(x => x.TextSpan)
            .ToImmutableList();

        var semanticModelResult = new SemanticModelResult(
            text,
            parserSession,
            compilationUnit);

        _recentSemanticModelResult = semanticModelResult;

        return semanticModelResult;
    }
}
