namespace Luthetus.Common.RazorLib.Clipboards.Models;

public class InMemoryClipboardServiceTests
{
    [Fact]
    public async Task Constructor()
    {
        /*
        public InMemoryClipboardService()
         */

        var inMemoryClipboardService = new InMemoryClipboardService();

        var text = "Hello World!";

        // [Fact]
        // public void ReadClipboard()
        await inMemoryClipboardService.SetClipboard(text);

        // [Fact]
        // public void SetClipboard()
        Assert.Equal(text, await inMemoryClipboardService.ReadClipboard());
    }
}