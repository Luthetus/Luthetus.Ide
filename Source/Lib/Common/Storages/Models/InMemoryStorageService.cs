namespace Luthetus.Common.RazorLib.Storages.Models;

public class InMemoryStorageService : IStorageService
{
	private Dictionary<string, object?> _map;

    public ValueTask SetValue(string key, object? value)
    {
		if (_map.ContainsKey(key))
			_map[key] = value;
		else
			_map.Add(key, value);

		return ValueTask.CompletedTask;
    }

    public ValueTask<object?> GetValue(string key)
    {
		object? value = null;

		if (_map.ContainsKey(key))
			value = _map[key];
			
        return ValueTask.FromResult(value);
    }
}
