using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.Storages.Models;

public interface IStorageService : ILuthetusCommonService
{
    public ValueTask SetValue(string key, object? value);
    public ValueTask<object?> GetValue(string key);
}