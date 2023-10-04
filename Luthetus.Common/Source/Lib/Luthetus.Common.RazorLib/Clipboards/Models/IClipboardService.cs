using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.Clipboards.Models;

public interface IClipboardService : ILuthetusCommonService
{
    public Task<string> ReadClipboard();
    public Task SetClipboard(string value);
}