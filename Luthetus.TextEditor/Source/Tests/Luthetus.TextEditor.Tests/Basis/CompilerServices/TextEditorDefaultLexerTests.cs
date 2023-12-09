using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class TextEditorDefaultLexerTests
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag { get; } = ImmutableArray<TextEditorDiagnostic>.Empty;
}