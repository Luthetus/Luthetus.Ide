using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

public sealed record SymbolDefinitionTests
{
	[Fact]
	public void SymbolDefinition_A()
	{
		//public SymbolDefinition(BoundScopeKey boundScopeKey, ISymbol symbol)
		throw new NotImplementedException();
	}

	[Fact]
	public void SymbolDefinition_B()
	{
		//public SymbolDefinition(
		//           BoundScopeKey boundScopeKey,
		//           ISymbol symbol,
		//           List<SymbolReference> symbolReferences)
		//       : this(boundScopeKey, symbol)
		throw new NotImplementedException();
	}

	[Fact]
	public void SymbolReferences()
	{
		//public List<SymbolReference> SymbolReferences { get; init; } = new();
		throw new NotImplementedException();
	}

	[Fact]
	public void BoundScopeKey()
	{
		//public BoundScopeKey BoundScopeKey { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void Symbol()
	{
		//public ISymbol Symbol { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void IsFabricated()
	{
		//public bool IsFabricated { get; init; }
		throw new NotImplementedException();
	}

	[Fact]
	public void GetSymbolReferences()
	{
		//public ImmutableArray<SymbolReference> GetSymbolReferences() => SymbolReferences.ToImmutableArray();
		throw new NotImplementedException();
	}
}