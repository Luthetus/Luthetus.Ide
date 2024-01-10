using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class TextEditorDefaultLexer : ILexer
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticList { get; } = ImmutableArray<TextEditorDiagnostic>.Empty;
}