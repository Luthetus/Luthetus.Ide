using System.Collections.Immutable;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer;

public class GenericPreprocessorDefinitionTests
{
	[Fact]
	public void GenericPreprocessorDefinition()
	{
		//public GenericPreprocessorDefinition(
		//	string transitionSubstring,
		//	ImmutableArray<DeliminationExtendedSyntaxDefinition> deliminationExtendedSyntaxBag)
	}

	[Fact]
	public void TransitionSubstring()
	{
		//public string TransitionSubstring { get; }
	}

	[Fact]
	public void DeliminationExtendedSyntaxBag()
	{
		//public ImmutableArray<DeliminationExtendedSyntaxDefinition> DeliminationExtendedSyntaxBag { get; }
	}
}