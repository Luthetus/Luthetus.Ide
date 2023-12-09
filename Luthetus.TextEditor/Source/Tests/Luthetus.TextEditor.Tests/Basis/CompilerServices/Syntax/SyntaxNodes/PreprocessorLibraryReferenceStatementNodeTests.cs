using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record PreprocessorLibraryReferenceStatementNodeTests
{
	[Fact]
	public void PreprocessorLibraryReferenceStatementNode()
	{
		//public PreprocessorLibraryReferenceStatementNode(
	 //       ISyntaxToken includeDirectiveSyntaxToken,
	 //       ISyntaxToken libraryReferenceSyntaxToken)
	}

	[Fact]
	public void IncludeDirectiveSyntaxToken()
	{
		//public ISyntaxToken IncludeDirectiveSyntaxToken { get; }
	}

	[Fact]
	public void LibraryReferenceSyntaxToken()
	{
		//public ISyntaxToken LibraryReferenceSyntaxToken { get; }
	}

	[Fact]
	public void ChildBag()
	{
		//public ImmutableArray<ISyntax> ChildBag { get; }
	}

	[Fact]
	public void IsFabricated()
	{
		//public bool IsFabricated { get; init; }
	}

	[Fact]
	public void SyntaxKind()
	{
		//public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorLibraryReferenceStatementNode;
	}
}