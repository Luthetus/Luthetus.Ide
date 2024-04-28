using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.Tests.Basis.Rows.Models;

/// <summary>
/// <see cref="RowEndingKindExtensions"/>
/// </summary>
public class RowEndingKindExtensionsTests
{
	/// <summary>
	/// <see cref="RowEndingKindExtensions.AsCharacters(LineEndKind)"/>
	/// </summary>
	[Fact]
	public void AsCharacters()
	{
		Assert.Equal("\r", LineEndKind.CarriageReturn.AsCharacters());
		Assert.Equal("\n", LineEndKind.LineFeed.AsCharacters());
		Assert.Equal("\r\n", LineEndKind.CarriageReturnLineFeed.AsCharacters());
		Assert.Equal(string.Empty, LineEndKind.StartOfFile.AsCharacters());
		Assert.Equal(string.Empty, LineEndKind.EndOfFile.AsCharacters());
		Assert.Equal(string.Empty, LineEndKind.Unset.AsCharacters());
	}

	/// <summary>
	/// <see cref="RowEndingKindExtensions.AsCharactersHtmlEscaped(LineEndKind)"/>
	/// </summary>
	[Fact]
	public void AsCharactersHtmlEscaped()
	{
		Assert.Equal("\\r", LineEndKind.CarriageReturn.AsCharactersHtmlEscaped());
		Assert.Equal("\\n", LineEndKind.LineFeed.AsCharactersHtmlEscaped());
		Assert.Equal("\\r\\n", LineEndKind.CarriageReturnLineFeed.AsCharactersHtmlEscaped());
		Assert.Equal("SOF", LineEndKind.StartOfFile.AsCharactersHtmlEscaped());
		Assert.Equal("EOF", LineEndKind.EndOfFile.AsCharactersHtmlEscaped());
	}

	/// <summary>
	/// <see cref="RowEndingKindExtensions.AsFriendlyName(LineEndKind)"/>
	/// </summary>
	[Fact]
	public void AsFriendlyName()
	{
		Assert.Equal("CR", LineEndKind.CarriageReturn.AsFriendlyName());
		Assert.Equal("LF", LineEndKind.LineFeed.AsFriendlyName());
		Assert.Equal("CRLF", LineEndKind.CarriageReturnLineFeed.AsFriendlyName());
		Assert.Equal("Unset", LineEndKind.Unset.AsFriendlyName());
		Assert.Equal("SOF", LineEndKind.StartOfFile.AsFriendlyName());
		Assert.Equal("EOF", LineEndKind.EndOfFile.AsFriendlyName());
	}

	/// <summary>
	/// <see cref="RowEndingKindExtensions.GetRowEndingsUserAllowedToUse(LineEndKind)"/>
	/// </summary>
	[Fact]
	public void GetRowEndingsUserAllowedToUse()
	{
		var rowEndingsUserAllowedToUse = LineEndKind.Unset.GetRowEndingsUserAllowedToUse();

		Assert.Equal(3, rowEndingsUserAllowedToUse.Length);
		Assert.Contains(rowEndingsUserAllowedToUse, x => x == LineEndKind.CarriageReturn);
		Assert.Contains(rowEndingsUserAllowedToUse, x => x == LineEndKind.LineFeed);
		Assert.Contains(rowEndingsUserAllowedToUse, x => x == LineEndKind.CarriageReturnLineFeed);
	}
}