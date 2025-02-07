using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public record struct SymbolReference
{
    public SymbolReference(Symbol symbol, int scopeIndexKey)
    {
        Symbol = symbol;
        ScopeIndexKey = scopeIndexKey;
    }

    public Symbol Symbol { get; }
    public int ScopeIndexKey { get; }
}