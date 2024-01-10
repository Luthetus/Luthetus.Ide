using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class TextEditorDefaultBinder : IBinder
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; } = ImmutableArray<TextEditorDiagnostic>.Empty;
    public ImmutableArray<ITextEditorSymbol> SymbolsList { get; } = ImmutableArray<ITextEditorSymbol>.Empty;

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan)
    {
        return null;
    }

    public IBoundScope? GetBoundScope(TextEditorTextSpan textSpan)
    {
        return null;
    }
}