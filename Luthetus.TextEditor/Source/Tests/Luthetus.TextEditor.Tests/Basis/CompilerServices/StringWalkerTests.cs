using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;

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
	/// <see cref="StringWalker.PeekForSubstring(string)"/>
	/// <see cref="StringWalker.PeekForSubstringRange(ImmutableArray{string}, out string?)"/>
	/// <see cref="StringWalker.ReadWhitespace(IEnumerable{char}?)"/>
	/// <see cref="StringWalker.ReadUnsignedNumericLiteral()"/>
	/// <see cref="StringWalker.ReadUntil(char)"/>
    /// <see cref="StringWalker.ReadLine()"/>
    /// <see cref="StringWalker.ReadWordTuple(ImmutableArray{char}?)"/>
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

		// Peek for substring
		{
			Assert.False(stringWalker.PeekForSubstring("AlphabetSoup"));
			Assert.True(stringWalker.PeekForSubstring("Hello"));

            // Assert values of Properties.
            // The values should not have changed from 'Peek the next word' since a peek was performed.
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

		// Peek for a range of substrings
		{
			var substringsBag = new List<string> 
			{
                "AlphabetSoup",
				"Abc123",
				"Apple Sauce"
            };

			// A false result
			{
                Assert.False(stringWalker.PeekForSubstringRange(
					substringsBag.ToImmutableArray(),
					out var matchedOn));

                Assert.Null(matchedOn);
            }

			// A true result
			{
				substringsBag.Add("Hello");

                Assert.True(stringWalker.PeekForSubstringRange(
                    substringsBag.ToImmutableArray(),
                    out var matchedOn));

                Assert.Equal("Hello", matchedOn);
            }

            // Assert values of Properties.
            // The values should not have changed from 'Peek for substring' since a peek was performed.
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

        // Read until a character
        {
            var expectedOutput = "Hello" + ' ';
            var actualOutput = stringWalker.ReadUntil('W');

            Assert.Equal(expectedOutput, actualOutput);

            // Assert values of Properties.
            {
                Assert.Equal(expectedOutput.Length, stringWalker.PositionIndex);
                Assert.Equal(resourceUri, stringWalker.ResourceUri);
                Assert.Equal(sourceText, stringWalker.SourceText);
                Assert.Equal('W', stringWalker.CurrentCharacter);
                Assert.Equal('o', stringWalker.NextCharacter);
                Assert.Equal("World!\n7 Pillows\n \n,abc123", stringWalker.RemainingText);
                Assert.False(stringWalker.IsEof);
            }
        }

        // Read a line
        {
            var expectedOutput = "World!";
            var actualOutput = stringWalker.ReadLine();
            
            Assert.Equal(expectedOutput, actualOutput);

            // Assert values of Properties.
            {
                Assert.Equal(12, stringWalker.PositionIndex);
                Assert.Equal(resourceUri, stringWalker.ResourceUri);
                Assert.Equal(sourceText, stringWalker.SourceText);
                Assert.Equal('\n', stringWalker.CurrentCharacter);
                Assert.Equal('7', stringWalker.NextCharacter);
                Assert.Equal("\n7 Pillows\n \n,abc123", stringWalker.RemainingText);
                Assert.False(stringWalker.IsEof);
            }
        }

        // Read a word-tuple
        {
            // Read whitespace as to position the cursor at a word.
            _ = stringWalker.ReadWhitespace();

            var textSpan = new TextEditorTextSpan(
                13,
                14,
                (byte)GenericDecorationKind.None,
                resourceUri,
                sourceText);

            var expectedOutput = (textSpan, "7");
            var actualOutput = stringWalker.ReadWordTuple();

            Assert.Equal(expectedOutput, actualOutput);
        }
	}
    
    /// <summary>
    /// <see cref="StringWalker.ReadUnsignedNumericLiteral()"/>
    /// </summary>
    [Fact]
    public void ReadUnsignedNumericLiteral()
    {
        // No decimal place
        {
            var resourceUri = new ResourceUri("/unitTesting.txt");
            var sourceText = "123abc";

            var stringWalker = new StringWalker(
                resourceUri,
                sourceText);

            var numeric = stringWalker.ReadUnsignedNumericLiteral();
            Assert.Equal("123", numeric.TextSpan.GetText());
        }
        
        // With decimal place
        {
            var resourceUri = new ResourceUri("/unitTesting.txt");
            var sourceText = "123.456abc";

            var stringWalker = new StringWalker(
                resourceUri,
                sourceText);

            var numeric = stringWalker.ReadUnsignedNumericLiteral();
            Assert.Equal("123.456", numeric.TextSpan.GetText());
        }
        
        // With decimal place AND a period character
        {
            var resourceUri = new ResourceUri("/unitTesting.txt");
            var sourceText = "123.456.abc";

            var stringWalker = new StringWalker(
                resourceUri,
                sourceText);

            var numeric = stringWalker.ReadUnsignedNumericLiteral();
            Assert.Equal("123.456", numeric.TextSpan.GetText());
        }
    }

    /// <summary>
    /// <see cref="StringWalker.ReadWhitespace(IEnumerable{char}?)"/>
    /// </summary>
    [Fact]
	public void ReadWhitespace()
	{
        // No whitespaceOverrideBag
        {
            var resourceUri = new ResourceUri("/unitTesting.txt");
            var sourceText = ' ' + "\t\r\n\n\rApple";

            var stringWalker = new StringWalker(
                resourceUri,
                sourceText);

            var whitespace = stringWalker.ReadWhitespace();
            Assert.Equal(' ' + "\t\r\n\n\r", whitespace);
            Assert.Equal('A', stringWalker.CurrentCharacter);
        }

        // With whitespaceOverrideBag
        {
            var resourceUri = new ResourceUri("/unitTesting.txt");
            var sourceText = ' ' + "\t\r\n\n\rApple";

            var stringWalker = new StringWalker(
                resourceUri,
                sourceText);

            List<char> whitespaceOverrideBag = new List<char>
            {
                WhitespaceFacts.SPACE,
            };

            var whitespace = stringWalker.ReadWhitespace(whitespaceOverrideBag);
            Assert.Equal(" ", whitespace);
            Assert.Equal('\t', stringWalker.CurrentCharacter);
        }
	}
}