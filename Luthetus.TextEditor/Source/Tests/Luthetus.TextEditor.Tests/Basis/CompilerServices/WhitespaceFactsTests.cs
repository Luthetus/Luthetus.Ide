using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="WhitespaceFacts"/>
/// </summary>
public class WhitespaceFactsTests
{
	/// <summary>
	/// <see cref="WhitespaceFacts.SPACE"/>
	/// </summary>
	[Fact]
	public void SPACE()
	{
		Assert.Equal(' ', WhitespaceFacts.SPACE);
	}

	/// <summary>
	/// <see cref="WhitespaceFacts.TAB"/>
	/// </summary>
	[Fact]
	public void TAB()
	{
		Assert.Equal('\t', WhitespaceFacts.TAB);
	}

	/// <summary>
	/// <see cref="WhitespaceFacts.CARRIAGE_RETURN"/>
	/// </summary>
	[Fact]
	public void CARRIAGE_RETURN()
	{
		Assert.Equal('\r', WhitespaceFacts.CARRIAGE_RETURN);
	}

	/// <summary>
	/// <see cref="WhitespaceFacts.LINE_FEED"/>
	/// </summary>
	[Fact]
	public void LINE_FEED()
	{
		Assert.Equal('\n', WhitespaceFacts.LINE_FEED);
    }

	/// <summary>
	/// <see cref="WhitespaceFacts.ALL_LIST"/>
	/// </summary>
	[Fact]
	public void ALL_LIST()
	{
        Assert.Contains(WhitespaceFacts.SPACE, WhitespaceFacts.ALL_LIST);
        Assert.Contains(WhitespaceFacts.TAB, WhitespaceFacts.ALL_LIST);
        Assert.Contains(WhitespaceFacts.CARRIAGE_RETURN, WhitespaceFacts.ALL_LIST);
        Assert.Contains(WhitespaceFacts.LINE_FEED, WhitespaceFacts.ALL_LIST);
	}

	/// <summary>
	/// <see cref="WhitespaceFacts.LINE_ENDING_CHARACTER_LIST"/>
	/// </summary>
	[Fact]
	public void LINE_ENDING_CHARACTER_LIST()
	{
        Assert.Contains(WhitespaceFacts.CARRIAGE_RETURN, WhitespaceFacts.LINE_ENDING_CHARACTER_LIST);
        Assert.Contains(WhitespaceFacts.LINE_FEED, WhitespaceFacts.LINE_ENDING_CHARACTER_LIST);
	}
}