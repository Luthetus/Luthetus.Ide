using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

public sealed record SymbolDefinitionTests
{
	[Fact]
	public void SymbolDefinition_A()
	{
		//public SymbolDefinition(BoundScopeKey boundScopeKey, ISymbol symbol)
	}

	[Fact]
	public void SymbolDefinition_B()
	{
		//public SymbolDefinition(
	 //           BoundScopeKey boundScopeKey,
	 //           ISymbol symbol,
	 //           List<SymbolReference> symbolReferences)
	 //       : this(boundScopeKey, symbol)
	}

	[Fact]
	public void SymbolReferences()
	{
		//public List<SymbolReference> SymbolReferences { get; init; } = new();
	}

	[Fact]
	public void BoundScopeKey()
	{
		//public BoundScopeKey BoundScopeKey { get; }
	}

	[Fact]
	public void Symbol()
	{
		//public ISymbol Symbol { get; }
	}

	[Fact]
	public void IsFabricated()
	{
		//public bool IsFabricated { get; init; }
	}

	[Fact]
	public void GetSymbolReferences()
	{
		//public ImmutableArray<SymbolReference> GetSymbolReferences() => SymbolReferences.ToImmutableArray();
	}
}