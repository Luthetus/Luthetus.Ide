using Luthetus.TextEditor.RazorLib.Analysis;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public interface IParser
{
    public ImmutableArray<TextEditorDiagnostic> Diagnostics { get; }
}
