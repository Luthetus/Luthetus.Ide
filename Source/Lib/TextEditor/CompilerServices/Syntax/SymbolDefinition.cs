namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public record struct SymbolDefinition
{
    public SymbolDefinition(int scopeIndexKey, Symbol symbol)
    {
        ScopeIndexKey = scopeIndexKey;
        Symbol = symbol;
    }

    public List<SymbolReference> SymbolReferences { get; init; } = new();

    public int ScopeIndexKey { get; }
    public Symbol Symbol { get; }
    /// <summary>If a reference to a symbol is parsed prior to the definition, <see cref="IsFabricated"/> will be set to true. If later the definition is found, then this instance of <see cref="SymbolDefinition"/> will be overwritten to have this property with value of false.</summary>
    public bool IsFabricated { get; init; }

    public IReadOnlyList<SymbolReference> GetSymbolReferences() => SymbolReferences;
}