namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public sealed class SymbolReferenceTests
{
    public SymbolReference(ISymbol symbol, BoundScopeKey boundScopeKey)
    {
        Symbol = symbol;
        BoundScopeKey = boundScopeKey;
    }

    public ISymbol Symbol { get; }
    public BoundScopeKey BoundScopeKey { get; }
}