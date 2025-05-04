using System.Text;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.RazorLib.Lexers.Models;

/// <summary>Provides common API that can be used when implementing an <see cref="ITextEditorLexer" /> for the <see cref="TextEditorModel" />.<br /><br />The marker for an out of bounds read is <see cref="ParserFacts.END_OF_FILE" />.</summary>
public struct StringWalkerStruct
{
	/// <summary>Pass in the <see cref="ResourceUri"/> of a file, and its text. One can pass in <see cref="string.Empty"/> for the <see cref="ResourceUri"/> if they are only working with the text itself.</summary>
	public StringWalkerStruct(ResourceUri resourceUri, string sourceText)
	{
		ResourceUri = resourceUri;
		SourceText = sourceText;
	}

	/// <summary>The character index within the <see cref="SourceText" />.</summary>
	public int PositionIndex { get; private set; }

	/// <summary>The file that the <see cref="SourceText"/> came from.</summary>
	public ResourceUri ResourceUri { get; }

	/// <summary>The text which is to be stepped through.</summary>
	public string SourceText { get; }

	/// <summary>Returns <see cref="PeekCharacter" /> invoked with the value of zero</summary>
	public char CurrentCharacter => PeekCharacter(0);

	/// <summary>Returns <see cref="PeekCharacter" /> invoked with the value of one</summary>
	public char NextCharacter => PeekCharacter(1);

	/// <summary>Starting with <see cref="PeekCharacter" /> evaluated at 0 return that and the rest of the <see cref="SourceText" /><br /><br /><see cref="RemainingText" /> => SourceText.Substring(PositionIndex);</summary>
	public string RemainingText => SourceText[PositionIndex..];

	/// <summary>Returns if the current character is the end of file character</summary>
	public bool IsEof => CurrentCharacter == ParserFacts.END_OF_FILE;

	/// <summary>If <see cref="PositionIndex" /> is within bounds of the <see cref="SourceText" />.<br /><br />Then the character within the string <see cref="SourceText" /> at index of <see cref="PositionIndex" /> is returned and <see cref="PositionIndex" /> is incremented by one.<br /><br />Otherwise, <see cref="ParserFacts.END_OF_FILE" /> is returned and the value of <see cref="PositionIndex" /> is unchanged.</summary>
	public char ReadCharacter()
	{
		if (PositionIndex >= SourceText.Length)
			return ParserFacts.END_OF_FILE;

		return SourceText[PositionIndex++];
	}

	/// <summary>If (<see cref="PositionIndex" /> + <see cref="offset" />) is within bounds of the <see cref="SourceText" />.<br /><br />Then the character within the string <see cref="SourceText" /> at index of (<see cref="PositionIndex" /> + <see cref="offset" />) is returned and <see cref="PositionIndex" /> is unchanged.<br /><br />Otherwise, <see cref="ParserFacts.END_OF_FILE" /> is returned and the value of <see cref="PositionIndex" /> is unchanged.<br /><br />offset must be > -1</summary>
	public char PeekCharacter(int offset)
	{
		if (offset <= -1)
			throw new LuthetusTextEditorException($"{nameof(offset)} must be > -1");

		if (PositionIndex + offset >= SourceText.Length)
			return ParserFacts.END_OF_FILE;

		return SourceText[PositionIndex + offset];
	}

	/// <summary>If <see cref="PositionIndex" /> being decremented by 1 would result in <see cref="PositionIndex" /> being less than 0.<br /><br />Then <see cref="ParserFacts.END_OF_FILE" /> will be returned and <see cref="PositionIndex" /> will be left unchanged.<br /><br />Otherwise, <see cref="PositionIndex" /> will be decremented by one and the character within the string <see cref="SourceText" /> at index of <see cref="PositionIndex" /> is returned.</summary>
	public char BacktrackCharacter()
	{
		if (PositionIndex == 0)
			return ParserFacts.END_OF_FILE;

		PositionIndex--;

		return PeekCharacter(0);
	}

	/// <summary>Iterates a counter from 0 until the counter is equal to <see cref="length" />.<br /><br />Each iteration <see cref="ReadCharacter" /> will be invoked.<br /><br />If an iteration's invocation of <see cref="ReadCharacter" /> returned <see cref="ParserFacts.END_OF_FILE" /> then the method will short circuit and return regardless of whether it finished iterating to <see cref="length" /> or not.</summary>
	public string ReadRange(int length)
	{
		var consumeBuilder = new StringBuilder();

		for (var i = 0; i < length; i++)
		{
			var currentCharacter = ReadCharacter();

			consumeBuilder.Append(currentCharacter);

			if (currentCharacter == ParserFacts.END_OF_FILE)
				break;
		}

		return consumeBuilder.ToString();
	}

	/// <summary>Iterates a counter from 0 until the counter is equal to <see cref="length" />.<br /><br />Each iteration <see cref="PeekCharacter" /> will be invoked using the (<see cref="offset" /> + counter).<br /><br />If an iteration's invocation of <see cref="PeekCharacter" /> returned <see cref="ParserFacts.END_OF_FILE" /> then the method will short circuit and return regardless of whether it finished iterating to <see cref="length" /> or not.</summary>
	public string PeekRange(int offset, int length)
	{
		var peekBuilder = new StringBuilder();

		for (var i = 0; i < length; i++)
		{
			var currentCharacter = PeekCharacter(offset + i);

			peekBuilder.Append(currentCharacter);

			if (currentCharacter == ParserFacts.END_OF_FILE)
				break;
		}

		return peekBuilder.ToString();
	}

	/// <summary>Iterates a counter from 0 until the counter is equal to <see cref="length" />.<br /><br />Each iteration <see cref="BacktrackCharacter" /> will be invoked using the.<br /><br />If an iteration's invocation of <see cref="BacktrackCharacter" /> returned <see cref="ParserFacts.END_OF_FILE" /> then the method will short circuit and return regardless of whether it finished iterating to <see cref="length" /> or not.</summary>
	public string BacktrackRange(int length)
	{
		var backtrackBuilder = new StringBuilder();

		for (var i = 0; i < length; i++)
		{
			if (PositionIndex == 0)
			{
				backtrackBuilder.Append(ParserFacts.END_OF_FILE);
				return backtrackBuilder.ToString();
			}

			var currentCharacter = BacktrackCharacter();

			backtrackBuilder.Append(currentCharacter);

			if (currentCharacter == ParserFacts.END_OF_FILE)
				break;
		}

		return backtrackBuilder.ToString();
	}

	public string PeekNextWord()
	{
		var nextWordBuilder = new StringBuilder();

		var i = 0;

		char peekedChar;

		do
		{
			peekedChar = PeekCharacter(i++);

			if (WhitespaceFacts.ALL_LIST.Contains(peekedChar) ||
				KeyboardKeyFacts.IsPunctuationCharacter(peekedChar))
			{
				break;
			}

			nextWordBuilder.Append(peekedChar);
		} while (peekedChar != ParserFacts.END_OF_FILE);

		return nextWordBuilder.ToString();
	}

	/// <summary>Form a substring of the <see cref="SourceText" /> that starts inclusively at the index <see cref="PositionIndex" /> and has a maximum length of <see cref="substring" />.Length.<br /><br />This method uses <see cref="PeekRange" /> internally and therefore will return a string that ends with <see cref="ParserFacts.END_OF_FILE" /> if an index out of bounds read was performed on <see cref="SourceText" /></summary>
	public bool PeekForSubstring(string substring)
	{
		return PeekRange(0, substring.Length) == substring;
	}

	public bool PeekForSubstringRange(List<string> substringsList, out string? matchedOn)
	{
		foreach (var substring in substringsList)
		{
			if (PeekForSubstring(substring))
			{
				matchedOn = substring;
				return true;
			}
		}

		matchedOn = null;
		return false;
	}

	/// <summary>
	/// Provide <see cref="whitespaceOverrideList"/> to override the
	/// default of what qualifies as whitespace.
	/// The default whitespace chars are: <see cref="WhitespaceFacts.ALL_LIST"/>
	/// </summary>
	public string ReadWhitespace(IEnumerable<char>? whitespaceOverrideList = null)
	{
		var whitespaceCharacterList = whitespaceOverrideList ?? WhitespaceFacts.ALL_LIST;

		var whitespaceBuilder = new StringBuilder();

		while (whitespaceCharacterList.Contains(CurrentCharacter))
		{
			var currentCharacter = ReadCharacter();

			whitespaceBuilder.Append(currentCharacter);

			if (currentCharacter == ParserFacts.END_OF_FILE)
				break;
		}

		return whitespaceBuilder.ToString();
	}

	/// <Summary>
	/// Ex: '1.73', positive only.<br/>
	/// { 0, ..., 1, ..., 2, ...}
	/// </Summary>
	public TextEditorTextSpan ReadUnsignedNumericLiteral()
	{
		var startingPosition = PositionIndex;
		var seenPeriod = false;

		while (!IsEof)
		{
			if (!char.IsDigit(CurrentCharacter))
			{
				if (CurrentCharacter == '.' && !seenPeriod)
					seenPeriod = true;
				else
					break;
			}

			_ = ReadCharacter();
		}

		return new TextEditorTextSpan(startingPosition, ref this, 0);
	}

	public string ReadUntil(char deliminator)
	{
		var textBuilder = new StringBuilder();

		while (!IsEof)
		{
			if (CurrentCharacter == deliminator)
				break;

			textBuilder.Append(ReadCharacter());
		}

		return textBuilder.ToString();
	}

	/// <summary>
	/// The line ending is NOT included in the returned string
	/// </summary>
	public string ReadLine()
	{
		var textBuilder = new StringBuilder();

		while (!IsEof)
		{
			if (WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(CurrentCharacter))
				break;

			textBuilder.Append(ReadCharacter());
		}

		return textBuilder.ToString();
	}

	/// <summary>
	/// This method will return immediately upon encountering whitespace.
	/// Returns a text span with its <see cref="TextEditorTextSpan.StartInclusiveIndex"/> equal to '-1' if no word was found.
	/// </summary>
	public (TextEditorTextSpan textSpan, string value) ReadWordTuple(char[]? additionalCharactersToBreakOnList = null)
	{
		additionalCharactersToBreakOnList ??= Array.Empty<char>();

		// The wordBuilder is appended to everytime a character is consumed.
		var wordBuilder = new StringBuilder();

		// wordBuilderStartInclusiveIndex == -1 is to mean that wordBuilder is empty.
		var wordBuilderStartInclusiveIndex = -1;

		while (!IsEof)
		{
			if (WhitespaceFacts.ALL_LIST.Contains(CurrentCharacter) ||
				additionalCharactersToBreakOnList.Contains(CurrentCharacter))
			{
				break;
			}

			if (wordBuilderStartInclusiveIndex == -1)
			{
				// This is the start of a word as opposed to the continuation of a word
				wordBuilderStartInclusiveIndex = PositionIndex;
			}

			wordBuilder.Append(CurrentCharacter);

			_ = ReadCharacter();
		}

		return (new TextEditorTextSpan(
				wordBuilderStartInclusiveIndex,
				PositionIndex,
				0,
				ResourceUri,
				SourceText),
			wordBuilder.ToString());
	}
}