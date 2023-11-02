using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class TextEditorDefaultParser : IParser
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag { get; } = ImmutableArray<TextEditorDiagnostic>.Empty;
}