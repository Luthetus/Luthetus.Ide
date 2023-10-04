namespace Luthetus.Common.RazorLib.Storages.Models;

public class DoNothingStorageService : IStorageService
{
    public DoNothingStorageService(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    public bool IsEnabled { get; }

    public ValueTask SetValue(string key, object? value)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<object?> GetValue(string key)
    {
        return new ValueTask<object?>(ValueTask.CompletedTask);
    }
}