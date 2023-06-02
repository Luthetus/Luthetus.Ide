using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;

public sealed record SymbolDefinition
{
    public SymbolDefinition(
        BoundScopeKey boundScopeKey,
        ISymbol symbol)
    {
        BoundScopeKey = boundScopeKey;
        Symbol = symbol;
    }
    
    public SymbolDefinition(
            BoundScopeKey boundScopeKey,
            ISymbol symbol,
            List<SymbolReference> symbolReferences)
        : this(boundScopeKey, symbol)
    {
        SymbolReferences = symbolReferences;
    }

    internal List<SymbolReference> SymbolReferences { get; } = new();

    public BoundScopeKey BoundScopeKey { get; }
    public ISymbol Symbol { get; }
    /// <summary>If a reference to a symbol is parsed prior to the definition, <see cref="IsFabricated"/> will be set to true. If later the definition is found, then this instance of <see cref="SymbolDefinition"/> will be overwritten to have this property with value of false.</summary>
    public bool IsFabricated { get; init; }

    public ImmutableArray<SymbolReference> GetSymbolReferences() => SymbolReferences.ToImmutableArray();
}

