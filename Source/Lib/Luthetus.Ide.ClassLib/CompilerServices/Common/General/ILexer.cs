using Luthetus.TextEditor.RazorLib.Analysis;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public interface ILexer
{
    public ImmutableArray<TextEditorDiagnostic> Diagnostics { get; }
}
