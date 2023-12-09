using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

public class TokenWalkerTests
{
	[Fact]
	public void TokenWalker()
	{
		//public TokenWalker(ImmutableArray<ISyntaxToken> tokens, LuthetusDiagnosticBag diagnosticBag)
		throw new NotImplementedException();
	}

	[Fact]
	public void Tokens()
	{
		//public ImmutableArray<ISyntaxToken> Tokens => _tokenBag;
		throw new NotImplementedException();
	}

	[Fact]
	public void Current()
	{
		//public ISyntaxToken Current => Peek(0);
		throw new NotImplementedException();
	}

	[Fact]
	public void Next()
	{
		//public ISyntaxToken Next => Peek(1);
		throw new NotImplementedException();
	}

	[Fact]
	public void Previous()
	{
		//public ISyntaxToken Previous => Peek(-1);
		throw new NotImplementedException();
	}

	[Fact]
	public void IsEof()
	{
		//public bool IsEof => Current.SyntaxKind == SyntaxKind.EndOfFileToken;
		throw new NotImplementedException();
	}

	[Fact]
	public void EOF()
	{
		//private ISyntaxToken EOF => _tokenBag.Length > 0
	 //       ? _tokenBag[_tokenBag.Length - 1]
	 //       : new EndOfFileToken(new(0, 0, 0, new(string.Empty), string.Empty));
		throw new NotImplementedException();
	}

	[Fact]
	public void Peek()
	{
		//public ISyntaxToken Peek(int offset)
		throw new NotImplementedException();
	}

	[Fact]
	public void Consume()
	{
		//public ISyntaxToken Consume()
		throw new NotImplementedException();
	}

	[Fact]
	public void Backtrack()
	{
		//public ISyntaxToken Backtrack()
		throw new NotImplementedException();
	}

	[Fact]
	public void Match()
	{
		//public ISyntaxToken Match(SyntaxKind expectedSyntaxKind)
		throw new NotImplementedException();
	}

	[Fact]
	public void MatchRange()
	{
		//public ISyntaxToken MatchRange(IEnumerable<SyntaxKind> validSyntaxKinds, SyntaxKind fabricationKind)
		throw new NotImplementedException();
	}

	[Fact]
	public void GetBadToken()
	{
		//private BadToken GetBadToken() => new BadToken(new(0, 0, 0, new(string.Empty), string.Empty));
		throw new NotImplementedException();
	}
}