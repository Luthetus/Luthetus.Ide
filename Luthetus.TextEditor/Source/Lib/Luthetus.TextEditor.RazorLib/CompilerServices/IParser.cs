using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public interface IParser
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; }
}
