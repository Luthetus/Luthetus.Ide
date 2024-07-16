using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.Lexers.Models;

/// <summary>
/// <see cref="ResourceUri"/>
/// </summary>
public record ResourceUriTests
{
    /// <summary>
    /// <see cref="ResourceUri(string)"/>
	/// <br/>----<br/>
    /// <see cref="ResourceUri.Value"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var value = "/unitTesting.txt";
        var resourceUri = new ResourceUri(value);
		Assert.Equal(value, resourceUri.Value);
	}
}