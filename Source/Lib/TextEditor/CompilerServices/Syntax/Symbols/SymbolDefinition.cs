using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public sealed record SymbolDefinition
{
    public SymbolDefinition(Key<IScope> scopeKey, ISymbol symbol)
    {
        ScopeKey = scopeKey;
        Symbol = symbol;
    }

    public List<SymbolReference> SymbolReferences { get; init; } = new();

    public Key<IScope> ScopeKey { get; }
    public ISymbol Symbol { get; }
    /// <summary>If a reference to a symbol is parsed prior to the definition, <see cref="IsFabricated"/> will be set to true. If later the definition is found, then this instance of <see cref="SymbolDefinition"/> will be overwritten to have this property with value of false.</summary>
    public bool IsFabricated { get; init; }

    public ImmutableArray<SymbolReference> GetSymbolReferences() => SymbolReferences.ToImmutableArray();
}