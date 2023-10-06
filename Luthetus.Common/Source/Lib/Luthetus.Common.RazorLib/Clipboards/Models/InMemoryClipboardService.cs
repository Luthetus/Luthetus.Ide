namespace Luthetus.Common.RazorLib.Clipboards.Models;

public class InMemoryClipboardService : IClipboardService
{
    private string _clipboard = string.Empty;

    public InMemoryClipboardService(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    public bool IsEnabled { get; }

    public Task<string> ReadClipboardAsync()
    {
        return Task.FromResult(_clipboard);
    }

    public Task SetClipboardAsync(string value)
    {
        _clipboard = value;
        return Task.CompletedTask;
    }
}