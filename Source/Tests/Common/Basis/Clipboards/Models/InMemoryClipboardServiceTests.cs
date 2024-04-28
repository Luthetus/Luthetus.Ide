using Luthetus.Common.RazorLib.Clipboards.Models;

namespace Luthetus.Common.Tests.Basis.Clipboards.Models;

/// <summary>
/// <see cref="InMemoryClipboardService"/>
/// </summary>
public class InMemoryClipboardServiceTests
{
    /// <summary>
    /// <see cref="InMemoryClipboardService()"/>
    /// <br/>----<br/>
    /// <see cref="InMemoryClipboardService.SetClipboard(string)"/>
    /// <see cref="InMemoryClipboardService.ReadClipboard()"/>
    /// </summary>
    [Fact]
    public async Task Constructor()
    {
        var inMemoryClipboardService = new InMemoryClipboardService();

        var text = "Hello World!";

        await inMemoryClipboardService.SetClipboard(text);
        Assert.Equal(text, await inMemoryClipboardService.ReadClipboard());
    }
}