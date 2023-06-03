using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Semantics;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.TextEditorCase;

public record SemanticResultCSharp : ISemanticResult
{
    public SemanticResultCSharp(
        string text,
        Parser parserSession,
        CompilationUnit compilationUnit)
    {
        Text = text;
        ParserSession = parserSession;
        CompilationUnit = compilationUnit;
    }

    public string Text { get; }
    public Parser ParserSession { get; }
    public CompilationUnit CompilationUnit { get; }

    public ImmutableList<(TextEditorDiagnostic diagnostic, TextEditorTextSpan textSpan)> DiagnosticTextSpanTuples { get; init; } = ImmutableList<(TextEditorDiagnostic diagnostic, TextEditorTextSpan textSpan)>.Empty;
    public ImmutableList<(string message, TextEditorTextSpan textSpan)> SymbolMessageTextSpanTuples { get; init; } = ImmutableList<(string message, TextEditorTextSpan textSpan)>.Empty;
}