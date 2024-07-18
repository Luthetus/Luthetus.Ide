using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.TextEditor.Tests.Basis.Lexes.Models;

/// <summary>
/// <see cref="TextEditorTextSpan"/>
/// </summary>
public class TextEditorTextSpanTests
{
    /// <summary>
    /// <see cref="TextEditorTextSpan(int, int, byte, ResourceUri, string)"/>
	/// <br/>----<br/>
    /// <see cref="TextEditorTextSpan.StartingIndexInclusive"/>
    /// <see cref="TextEditorTextSpan.EndingIndexExclusive"/>
    /// <see cref="TextEditorTextSpan.DecorationByte"/>
    /// <see cref="TextEditorTextSpan.ResourceUri"/>
    /// <see cref="TextEditorTextSpan.SourceText"/>
    /// <see cref="TextEditorTextSpan.Length"/>
    /// <see cref="TextEditorTextSpan.GetText()"/>
    /// </summary>
    [Fact]
	public void Constructor_A()
	{
        var resourceUri = new ResourceUri("/unitTesting.txt");
		var functionIdentifier = "MyFunction";
		var sourceText = $"Abc123 {functionIdentifier} AppleSoup";
		var indexOfFunctionIdentifier = sourceText.IndexOf(functionIdentifier);
		var startingIndexInclusive = indexOfFunctionIdentifier;
		var endingIndexExclusive = indexOfFunctionIdentifier + functionIdentifier.Length;
		var decorationByte = (byte)GenericDecorationKind.Function;

        var textSpan = new TextEditorTextSpan(
            startingIndexInclusive,
            endingIndexExclusive,
            decorationByte,
            resourceUri,
			sourceText);

		Assert.Equal(startingIndexInclusive, textSpan.StartingIndexInclusive);
		Assert.Equal(endingIndexExclusive, textSpan.EndingIndexExclusive);
		Assert.Equal(decorationByte, textSpan.DecorationByte);
		Assert.Equal(resourceUri, textSpan.ResourceUri);
		Assert.Equal(sourceText, textSpan.SourceText);
		Assert.Equal(functionIdentifier.Length, textSpan.Length);
		Assert.Equal(textSpan.EndingIndexExclusive - textSpan.StartingIndexInclusive, textSpan.Length);
		Assert.Equal(functionIdentifier, textSpan.GetText());
	}

    /// <summary>
    /// <see cref="TextEditorTextSpan(int, StringWalker, byte)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorTextSpan.StartingIndexInclusive"/>
    /// <see cref="TextEditorTextSpan.EndingIndexExclusive"/>
    /// <see cref="TextEditorTextSpan.DecorationByte"/>
    /// <see cref="TextEditorTextSpan.ResourceUri"/>
    /// <see cref="TextEditorTextSpan.SourceText"/>
    /// <see cref="TextEditorTextSpan.Length"/>
    /// <see cref="TextEditorTextSpan.GetText()"/>
    /// </summary>
    [Fact]
	public void Constructor_B()
	{
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var functionIdentifier = "MyFunction";
        var sourceText = $"Abc123 {functionIdentifier} AppleSoup";
        var decorationByte = (byte)GenericDecorationKind.Function;

		var stringWalker = new StringWalker(resourceUri, sourceText);
		
		var firstCharacterOfFunctionIdentifier = functionIdentifier.First();

        while (!stringWalker.IsEof)
		{
			if (stringWalker.CurrentCharacter == firstCharacterOfFunctionIdentifier)
			{
				if (stringWalker.PeekForSubstring(functionIdentifier))
					break;
            }

			_ = stringWalker.ReadCharacter();
		}

		var startingIndexInclusive = stringWalker.PositionIndex;
        var endingIndexExclusive = startingIndexInclusive + functionIdentifier.Length;

        _ = stringWalker.ReadRange(functionIdentifier.Length);

        var textSpan = new TextEditorTextSpan(
            startingIndexInclusive,
            stringWalker,
            decorationByte);

        Assert.Equal(startingIndexInclusive, textSpan.StartingIndexInclusive);
        Assert.Equal(endingIndexExclusive, textSpan.EndingIndexExclusive);
        Assert.Equal(decorationByte, textSpan.DecorationByte);
        Assert.Equal(resourceUri, textSpan.ResourceUri);
        Assert.Equal(sourceText, textSpan.SourceText);
        Assert.Equal(functionIdentifier.Length, textSpan.Length);
        Assert.Equal(textSpan.EndingIndexExclusive - textSpan.StartingIndexInclusive, textSpan.Length);
        Assert.Equal(functionIdentifier, textSpan.GetText());
	}

	/// <summary>
	/// <see cref="TextEditorTextSpan.FabricateTextSpan(string)"/>
	/// </summary>
	[Fact]
	public void FabricateTextSpan()
	{
		throw new NotImplementedException();
	}
}