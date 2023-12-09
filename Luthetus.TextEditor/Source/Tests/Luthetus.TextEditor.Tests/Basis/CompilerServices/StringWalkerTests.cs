using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

public class StringWalkerTests
{
	[Fact]
	public void StringWalker()
	{
		//public StringWalker(ResourceUri resourceUri, string sourceText)
		throw new NotImplementedException();
	}

	[Fact]
	public void PositionIndex()
	{
		//public int PositionIndex { get; private set; }
		throw new NotImplementedException();
	}

	[Fact]
	public void ResourceUri()
	{
		//public ResourceUri ResourceUri { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void SourceText()
	{
		//public string SourceText { get; }
		throw new NotImplementedException();
	}

	[Fact]
	public void CurrentCharacter()
	{
		//public char CurrentCharacter => PeekCharacter(0);
		throw new NotImplementedException();
	}

	[Fact]
	public void NextCharacter()
	{
		//public char NextCharacter => PeekCharacter(1);
		throw new NotImplementedException();
	}

	[Fact]
	public void RemainingText()
	{
		//public string RemainingText => SourceText[PositionIndex..];
		throw new NotImplementedException();
	}

	[Fact]
	public void IsEof()
	{
		//public bool IsEof => CurrentCharacter == ParserFacts.END_OF_FILE;
		throw new NotImplementedException();
	}

	[Fact]
	public void ReadCharacter()
	{
		//public char ReadCharacter()
		throw new NotImplementedException();
	}

	[Fact]
	public void PeekCharacter()
	{
		//public char PeekCharacter(int offset)
		throw new NotImplementedException();
	}

	[Fact]
	public void BacktrackCharacter()
	{
		//public char BacktrackCharacter()
		throw new NotImplementedException();
	}

	[Fact]
	public void ReadRange()
	{
		//public string ReadRange(int length)
		throw new NotImplementedException();
	}

	[Fact]
	public void PeekRange()
	{
		//public string PeekRange(int offset, int length)
		throw new NotImplementedException();
	}

	[Fact]
	public void BacktrackRange()
	{
		//public string BacktrackRange(int length)
		throw new NotImplementedException();
	}

	[Fact]
	public void PeekNextWord()
	{
		//public string PeekNextWord()
		throw new NotImplementedException();
	}

	[Fact]
	public void CheckForSubstring()
	{
		//public bool CheckForSubstring(string substring)
		throw new NotImplementedException();
	}


	[Fact]
	public void CheckForSubstringRange()
	{
		//public bool CheckForSubstringRange(ImmutableArray<string> substringsBag, out string? matchedOn)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReadWhitespace()
	{
		//public string ReadWhitespace(IEnumerable<char>? whitespaceOverrideBag = null)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReadUnsignedNumericLiteral()
	{
		//public NumericLiteralToken ReadUnsignedNumericLiteral()
		throw new NotImplementedException();
	}

	[Fact]
	public void ReadUntil()
	{
		//public string ReadUntil(char deliminator)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReadLine()
	{
		//public string ReadLine()
		throw new NotImplementedException();
	}

	[Fact]
	public void ReadWordTuple()
	{
		//public (TextEditorTextSpan textSpan, string value) ReadWordTuple(ImmutableArray<char>? additionalCharactersToBreakOnBag = null)
		throw new NotImplementedException();
	}
}