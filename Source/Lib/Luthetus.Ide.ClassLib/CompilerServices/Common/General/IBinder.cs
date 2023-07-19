using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public interface IBinder
{
    public ImmutableArray<TextEditorDiagnostic> Diagnostics { get; }
    public ImmutableArray<ITextEditorSymbol> Symbols { get; }
}
