namespace Luthetus.Common.RazorLib.Clipboards.Models;

public class InMemoryClipboardService : IClipboardService
{
    private string _clipboard = string.Empty;

    public Task<string> ReadClipboard()
    {
        return Task.FromResult(_clipboard);
    }

    public Task SetClipboard(string value)
    {
        _clipboard = value;
        return Task.CompletedTask;
    }
}