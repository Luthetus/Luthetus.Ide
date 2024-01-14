using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public interface ILexer
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticList { get; }
}
