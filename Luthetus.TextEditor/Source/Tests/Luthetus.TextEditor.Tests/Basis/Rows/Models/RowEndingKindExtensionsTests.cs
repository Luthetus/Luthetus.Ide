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
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="RowEndingKindExtensions.AsFriendlyName(RowEndingKind)"/>
	/// </summary>
	[Fact]
	public void AsFriendlyName()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="RowEndingKindExtensions.GetRowEndingsUserAllowedToUse(RowEndingKind)"/>
	/// </summary>
	[Fact]
	public void GetRowEndingsUserAllowedToUse()
	{
		throw new NotImplementedException();
	}
}