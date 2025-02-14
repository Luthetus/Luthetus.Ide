using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="AmbiguousIdentifierNode"/>
/// </summary>
public class AmbiguousIdentifierNodeTests
{
    /// <summary>
    /// <see cref="AmbiguousIdentifierNode(IdentifierToken)"/>
    /// <br/>----<br/>
    /// <see cref="AmbiguousIdentifierNode.IdentifierToken"/>
	/// <see cref="AmbiguousIdentifierNode.ChildList"/>
	/// <see cref="AmbiguousIdentifierNode.IsFabricated"/>
	/// <see cref="AmbiguousIdentifierNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var typeIdentifier = "SomeUndefinedType";
        var sourceText = $@"{typeIdentifier} MyMethod()
{{
}}";

		var indexOfTypeIdentifierInclusive = sourceText.IndexOf(typeIdentifier);

		var identifierToken = new IdentifierToken(new TextEditorTextSpan(
            indexOfTypeIdentifierInclusive,
            indexOfTypeIdentifierInclusive + typeIdentifier.Length,
			0,
			new ResourceUri("/unitTesting.txt"),
			sourceText));

		var ambiguousIdentifierNode = new AmbiguousIdentifierNode(identifierToken);

		Assert.Equal(identifierToken, ambiguousIdentifierNode.IdentifierToken);
		
		Assert.Single(ambiguousIdentifierNode.ChildList);
		Assert.Equal(identifierToken, ambiguousIdentifierNode.ChildList.Single());

		Assert.False(ambiguousIdentifierNode.IsFabricated);

		Assert.Equal(
			RazorLib.CompilerServices.Syntax.SyntaxKind.AmbiguousIdentifierNode,
			ambiguousIdentifierNode.SyntaxKind);
	}
}
