using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="ArraySyntaxToken"/>
/// </summary>
public class ArraySyntaxTokenTests
{
    /// <summary>
    /// <see cref="ArraySyntaxToken(TextEditorTextSpan)"/>
	/// <br/>----<br/>
    /// <see cref="ArraySyntaxToken.TextSpan"/>
	/// <see cref="ArraySyntaxToken.SyntaxKind"/>
	/// <see cref="ArraySyntaxToken.IsFabricated"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var text = @"public class MyClass
{
	private int[] _integers = Array.Empty<int>();
}";
		var targetSubstring = "[]";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

		var arraySyntaxToken = new ArraySyntaxToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
			0,
			new ResourceUri("/unitTesting.txt"),
			text));

		Assert.Equal(targetSubstring, arraySyntaxToken.TextSpan.GetText());
		Assert.Equal(SyntaxKind.ArraySyntaxToken, arraySyntaxToken.SyntaxKind);
		Assert.False(arraySyntaxToken.IsFabricated);
	}
}