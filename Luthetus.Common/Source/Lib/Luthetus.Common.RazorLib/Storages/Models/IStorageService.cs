namespace Luthetus.Common.RazorLib.Storages.Models;

public interface IStorageService
{
    public ValueTask SetValue(string key, object? value);
    public ValueTask<object?> GetValue(string key);
}