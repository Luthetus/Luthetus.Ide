using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="StringWalker"/>
/// </summary>
public class StringWalkerTests
{
    /// <summary>
    /// <see cref="StringWalker(ResourceUri, string)"/>
	/// <br/>----<br/>
    /// <see cref="StringWalker.PositionIndex"/>
    /// <see cref="StringWalker.ResourceUri"/>
    /// <see cref="StringWalker.SourceText"/>
    /// <see cref="StringWalker.CurrentCharacter"/>
    /// <see cref="StringWalker.NextCharacter"/>
    /// <see cref="StringWalker.RemainingText"/>
    /// <see cref="StringWalker.IsEof"/>
	/// <see cref="StringWalker.ReadCharacter()"/>
	/// <see cref="StringWalker.PeekCharacter(int)"/>
	/// <see cref="StringWalker.BacktrackCharacter()"/>
	/// <see cref="StringWalker.ReadRange(int)"/>
	/// <see cref="StringWalker.PeekRange(int, int)"/>
	/// <see cref="StringWalker.BacktrackRange(int)"/>
	/// <see cref="StringWalker.PeekNextWord()"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var resourceUri = new ResourceUri("/unitTesting.txt");
		var sourceText = TestConstants.SOURCE_TEXT;

        var stringWalker = new StringWalker(resourceUri, sourceText);

		// Assert initial values of Properties
		{
			Assert.Equal(0, stringWalker.PositionIndex);
			Assert.Equal(resourceUri, stringWalker.ResourceUri);
			Assert.Equal(sourceText, stringWalker.SourceText);
			Assert.Equal('H', stringWalker.CurrentCharacter);
			Assert.Equal('e', stringWalker.NextCharacter);
			Assert.Equal("Hello World!\n7 Pillows\n \n,abc123", stringWalker.RemainingText);
			Assert.False(stringWalker.IsEof);
		}

		// Read a character
		{
			Assert.Equal('H', stringWalker.ReadCharacter());

			// Assert values of Properties after having read a character
            Assert.Equal(1, stringWalker.PositionIndex);
            Assert.Equal(resourceUri, stringWalker.ResourceUri);
            Assert.Equal(sourceText, stringWalker.SourceText);
            Assert.Equal('e', stringWalker.CurrentCharacter);
            Assert.Equal('l', stringWalker.NextCharacter);
            Assert.Equal("ello World!\n7 Pillows\n \n,abc123", stringWalker.RemainingText);
            Assert.False(stringWalker.IsEof);
		}

		// Peek a character
		{
			// Peek current character
            Assert.Equal('e', stringWalker.PeekCharacter(0));
            // Peek next character
            Assert.Equal('l', stringWalker.PeekCharacter(1));
            // Peek the character two indices higher
            Assert.Equal('l', stringWalker.PeekCharacter(2));
            // Peek the character three indices higher
            Assert.Equal('o', stringWalker.PeekCharacter(3));
			
			// Assert values of Properties after having peeked a character
			// These values should not have changed since the peek method was used.
			Assert.Equal(1, stringWalker.PositionIndex);
			Assert.Equal(resourceUri, stringWalker.ResourceUri);
			Assert.Equal(sourceText, stringWalker.SourceText);
			Assert.Equal('e', stringWalker.CurrentCharacter);
			Assert.Equal('l', stringWalker.NextCharacter);
			Assert.Equal("ello World!\n7 Pillows\n \n,abc123", stringWalker.RemainingText);
			Assert.False(stringWalker.IsEof);
        }

		// Backtrack a character
		{
			Assert.Equal('H', stringWalker.BacktrackCharacter());

			// Assert values of Properties after having backtracked a character
			// These values should be equal to the initial state.
			Assert.Equal(0, stringWalker.PositionIndex);
			Assert.Equal(resourceUri, stringWalker.ResourceUri);
			Assert.Equal(sourceText, stringWalker.SourceText);
			Assert.Equal('H', stringWalker.CurrentCharacter);
			Assert.Equal('e', stringWalker.NextCharacter);
			Assert.Equal("Hello World!\n7 Pillows\n \n,abc123", stringWalker.RemainingText);
			Assert.False(stringWalker.IsEof);
		}

		// Read a range of characters
		{
			var goalString = "Hello";
            var charsToRead = goalString.Length;
            Assert.Equal(goalString, stringWalker.ReadRange(charsToRead));

            // Assert values of Properties after having read a range of characters
            Assert.Equal(goalString.Length, stringWalker.PositionIndex);
            Assert.Equal(resourceUri, stringWalker.ResourceUri);
            Assert.Equal(sourceText, stringWalker.SourceText);
            Assert.Equal(' ', stringWalker.CurrentCharacter);
            Assert.Equal('W', stringWalker.NextCharacter);
            Assert.Equal(" World!\n7 Pillows\n \n,abc123", stringWalker.RemainingText);
            Assert.False(stringWalker.IsEof);
        }

		// Peek a range of characters
		{
            var goalString = "World";
            var charsToRead = goalString.Length;

			// The cursor is one character away from reading the goal string.
			// So another assertion can test the 'offset' argument of 'PeekRange'
            Assert.Equal(" Worl", stringWalker.PeekRange(0, charsToRead));

			// Increase index to begin reading by 1. This puts the cursor
			// at the correct position to read the goalString.
            Assert.Equal(goalString, stringWalker.PeekRange(1, charsToRead));

            // Assert values of Properties after having peeked a range of characters
			// These values should be unchanged from the 'read range' assertions
            Assert.Equal(goalString.Length, stringWalker.PositionIndex);
            Assert.Equal(resourceUri, stringWalker.ResourceUri);
            Assert.Equal(sourceText, stringWalker.SourceText);
            Assert.Equal(' ', stringWalker.CurrentCharacter);
            Assert.Equal('W', stringWalker.NextCharacter);
            Assert.Equal(" World!\n7 Pillows\n \n,abc123", stringWalker.RemainingText);
            Assert.False(stringWalker.IsEof);
        }

		// Backtrack a range of characters
		{
			// By backtracking a length equal to the current position index,
			// one returns to the initial state
			var result = stringWalker.BacktrackRange(stringWalker.PositionIndex);
            Assert.Equal(new string("Hello".Reverse().ToArray()), result);

            // Assert initial values of Properties
            {
                Assert.Equal(0, stringWalker.PositionIndex);
                Assert.Equal(resourceUri, stringWalker.ResourceUri);
                Assert.Equal(sourceText, stringWalker.SourceText);
                Assert.Equal('H', stringWalker.CurrentCharacter);
                Assert.Equal('e', stringWalker.NextCharacter);
                Assert.Equal("Hello World!\n7 Pillows\n \n,abc123", stringWalker.RemainingText);
                Assert.False(stringWalker.IsEof);
            }
        }

		// Peek the next word
		{
			Assert.Equal("Hello", stringWalker.PeekNextWord());

            // Assert values of Properties.
            // The values should not have changed from 'Backtrack' since a peek was performed.
            {
                Assert.Equal(0, stringWalker.PositionIndex);
                Assert.Equal(resourceUri, stringWalker.ResourceUri);
                Assert.Equal(sourceText, stringWalker.SourceText);
                Assert.Equal('H', stringWalker.CurrentCharacter);
                Assert.Equal('e', stringWalker.NextCharacter);
                Assert.Equal("Hello World!\n7 Pillows\n \n,abc123", stringWalker.RemainingText);
                Assert.False(stringWalker.IsEof);
            }
        }

        throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="StringWalker.CheckForSubstring(string)"/>
	/// </summary>
	[Fact]
	public void CheckForSubstring()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="StringWalker.CheckForSubstringRange(System.Collections.Immutable.ImmutableArray{string}, out string?)"/>
	/// </summary>
	[Fact]
	public void CheckForSubstringRange()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="StringWalker.ReadWhitespace(IEnumerable{char}?)"/>
	/// </summary>
	[Fact]
	public void ReadWhitespace()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="StringWalker.ReadUnsignedNumericLiteral()"/>
	/// </summary>
	[Fact]
	public void ReadUnsignedNumericLiteral()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="StringWalker.ReadUntil(char)"/>
	/// </summary>
	[Fact]
	public void ReadUntil()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="StringWalker.ReadLine()"/>
	/// </summary>
	[Fact]
	public void ReadLine()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="StringWalker.ReadWordTuple(System.Collections.Immutable.ImmutableArray{char}?)"/>
	/// </summary>
	[Fact]
	public void ReadWordTuple()
	{
		throw new NotImplementedException();
	}
}