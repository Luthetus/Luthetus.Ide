using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public interface IBinder
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag { get; }
    public ImmutableArray<ITextEditorSymbol> SymbolsBag { get; }

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan);
    public IBoundScope? GetBoundScope(TextEditorTextSpan textSpan);
}
