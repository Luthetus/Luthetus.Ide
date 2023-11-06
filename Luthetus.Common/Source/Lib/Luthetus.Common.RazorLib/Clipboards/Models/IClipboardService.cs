namespace Luthetus.Common.RazorLib.Clipboards.Models;

public interface IClipboardService
{
    public Task<string> ReadClipboard();
    public Task SetClipboard(string value);
}