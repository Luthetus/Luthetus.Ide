using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public sealed class SymbolReference
{
    public SymbolReference(ISymbol symbol, Key<IScope> scopeKey)
    {
        Symbol = symbol;
        ScopeKey = scopeKey;
    }

    public ISymbol Symbol { get; }
    public Key<IScope> ScopeKey { get; }
}