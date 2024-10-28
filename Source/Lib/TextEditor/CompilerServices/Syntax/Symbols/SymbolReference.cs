using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public sealed class SymbolReference
{
    public SymbolReference(ISymbol symbol, int scopeIndexKey)
    {
        Symbol = symbol;
        ScopeIndexKey = scopeIndexKey;
    }

    public ISymbol Symbol { get; }
    public int ScopeIndexKey { get; }
}