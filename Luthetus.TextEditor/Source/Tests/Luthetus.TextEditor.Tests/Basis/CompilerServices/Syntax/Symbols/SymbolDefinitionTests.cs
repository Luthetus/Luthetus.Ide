using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

public sealed record SymbolDefinitionTests
{
    public SymbolDefinition(BoundScopeKey boundScopeKey, ISymbol symbol)
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

    public List<SymbolReference> SymbolReferences { get; init; } = new();

    public BoundScopeKey BoundScopeKey { get; }
    public ISymbol Symbol { get; }
    /// <summary>If a reference to a symbol is parsed prior to the definition, <see cref="IsFabricated"/> will be set to true. If later the definition is found, then this instance of <see cref="SymbolDefinition"/> will be overwritten to have this property with value of false.</summary>
    public bool IsFabricated { get; init; }

    public ImmutableArray<SymbolReference> GetSymbolReferences() => SymbolReferences.ToImmutableArray();
}