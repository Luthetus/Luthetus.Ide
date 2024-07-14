using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="TokenWalker"/>
/// </summary>
public class TokenWalkerTests
{
    /// <summary>
    /// <see cref="TokenWalker(ImmutableArray{ISyntaxToken}, LuthDiagnosticBag)"/>
    /// <br/>----<br/>
	/// <see cref="TokenWalker.TokenList"/>
    /// <see cref="TokenWalker.Current"/>
    /// <see cref="TokenWalker.Next"/>
    /// <see cref="TokenWalker.Previous"/>
    /// <see cref="TokenWalker.IsEof"/>
    /// <see cref="TokenWalker.EOF"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TokenWalker.Peek(int)"/>
	/// </summary>
	[Fact]
	public void Peek()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TokenWalker.Consume()"/>
	/// </summary>
	[Fact]
	public void Consume()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TokenWalker.Backtrack()"/>
	/// </summary>
	[Fact]
	public void Backtrack()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TokenWalker.Match(SyntaxKind)"/>
	/// </summary>
	[Fact]
	public void Match()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TokenWalker.MatchRange(IEnumerable{SyntaxKind}, SyntaxKind)"/>
	/// </summary>
	[Fact]
	public void MatchRange()
	{
		throw new NotImplementedException();
	}
}