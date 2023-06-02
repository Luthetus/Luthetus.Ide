using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;

public sealed class SymbolReference
{
    public SymbolReference(
        ISymbol symbol,
        BoundScopeKey boundScopeKey)
    {
        Symbol = symbol;
        BoundScopeKey = boundScopeKey;
    }

    public ISymbol Symbol { get; }
    public BoundScopeKey BoundScopeKey { get; }
}

