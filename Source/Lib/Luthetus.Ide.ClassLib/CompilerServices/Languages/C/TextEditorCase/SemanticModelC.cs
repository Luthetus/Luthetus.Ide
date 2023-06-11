using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.Semantics;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.C.TextEditorCase;

public class SemanticModelC : ISemanticModel
{
    private SemanticModelResultC? _recentSemanticModelResult;
    private SemanticResult _semanticResult = new();

    public ISemanticResult? SemanticResult => _semanticResult;

    public TextEditorSymbolDefinition? GoToDefinition(
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

    public SemanticModelResultC? ParseWithResult(
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

                var textSpan = x.TextSpan with
                {
                    DecorationByte = (byte)textEditorDecorationKind
                };

                return (x, textSpan);
            }).ToImmutableList()
        };

        _semanticResult = _semanticResult with 
        {
            SymbolMessageTextSpanTuples = parserSession.Binder.Symbols
                .Select(x => ($"({x.SyntaxKind.GetType().Name}) {x.TextSpan.GetText()}", x.TextSpan))
                .ToImmutableList()
        };

        var semanticModelResult = new SemanticModelResultC(
            text,
            parserSession,
            compilationUnit);

        _recentSemanticModelResult = semanticModelResult;

        return semanticModelResult;
    }
}
