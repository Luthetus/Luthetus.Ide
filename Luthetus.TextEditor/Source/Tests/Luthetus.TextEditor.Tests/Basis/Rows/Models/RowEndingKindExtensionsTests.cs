using Xunit;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.Tests.Basis.Rows.Models;

/// <summary>
/// <see cref="RowEndingKindExtensions"/>
/// </summary>
public class RowEndingKindExtensionsTests
{
	/// <summary>
	/// <see cref="RowEndingKindExtensions.AsCharacters(RowEndingKind)"/>
	/// </summary>
	[Fact]
	public void AsCharacters()
	{
		Assert.Equal("\r", RowEndingKind.CarriageReturn.AsCharacters());
		Assert.Equal("\n", RowEndingKind.Linefeed.AsCharacters());
		Assert.Equal("\r\n", RowEndingKind.CarriageReturnLinefeed.AsCharacters());
		Assert.Equal(string.Empty, RowEndingKind.StartOfFile.AsCharacters());
		Assert.Equal(string.Empty, RowEndingKind.EndOfFile.AsCharacters());
		Assert.Equal(string.Empty, RowEndingKind.Unset.AsCharacters());
	}

	/// <summary>
	/// <see cref="RowEndingKindExtensions.AsCharactersHtmlEscaped(RowEndingKind)"/>
	/// </summary>
	[Fact]
	public void AsCharactersHtmlEscaped()
	{
		Assert.Equal("\\r", RowEndingKind.CarriageReturn.AsCharactersHtmlEscaped());
		Assert.Equal("\\n", RowEndingKind.Linefeed.AsCharactersHtmlEscaped());
		Assert.Equal("\\r\\n", RowEndingKind.CarriageReturnLinefeed.AsCharactersHtmlEscaped());
		Assert.Equal("SOF", RowEndingKind.StartOfFile.AsCharactersHtmlEscaped());
		Assert.Equal("EOF", RowEndingKind.EndOfFile.AsCharactersHtmlEscaped());
	}

	/// <summary>
	/// <see cref="RowEndingKindExtensions.AsFriendlyName(RowEndingKind)"/>
	/// </summary>
	[Fact]
	public void AsFriendlyName()
	{
		Assert.Equal("CR", RowEndingKind.CarriageReturn.AsFriendlyName());
		Assert.Equal("LF", RowEndingKind.Linefeed.AsFriendlyName());
		Assert.Equal("CRLF", RowEndingKind.CarriageReturnLinefeed.AsFriendlyName());
		Assert.Equal("Unset", RowEndingKind.Unset.AsFriendlyName());
		Assert.Equal("SOF", RowEndingKind.StartOfFile.AsFriendlyName());
		Assert.Equal("EOF", RowEndingKind.EndOfFile.AsFriendlyName());
	}

	/// <summary>
	/// <see cref="RowEndingKindExtensions.GetRowEndingsUserAllowedToUse(RowEndingKind)"/>
	/// </summary>
	[Fact]
	public void GetRowEndingsUserAllowedToUse()
	{
		var rowEndingsUserAllowedToUse = RowEndingKind.Unset.GetRowEndingsUserAllowedToUse();

		Assert.Equal(3, rowEndingsUserAllowedToUse.Length);
		Assert.Contains(rowEndingsUserAllowedToUse, x => x == RowEndingKind.CarriageReturn);
		Assert.Contains(rowEndingsUserAllowedToUse, x => x == RowEndingKind.Linefeed);
		Assert.Contains(rowEndingsUserAllowedToUse, x => x == RowEndingKind.CarriageReturnLinefeed);
	}
}