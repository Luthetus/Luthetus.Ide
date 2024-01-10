using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public interface IBinder
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; }
    public ImmutableArray<ITextEditorSymbol> SymbolsList { get; }

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan);
    public IBoundScope? GetBoundScope(TextEditorTextSpan textSpan);
}
