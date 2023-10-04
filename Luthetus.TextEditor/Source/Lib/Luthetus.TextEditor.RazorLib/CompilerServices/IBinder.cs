using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public interface IBinder
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag { get; }
    public ImmutableArray<ITextEditorSymbol> SymbolsBag { get; }
}