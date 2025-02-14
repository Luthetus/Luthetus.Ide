using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Storages.Models;

public interface IStorageService
{
    public ValueTask SetValue(string key, object? value);
    public ValueTask<object?> GetValue(string key);

	public static void WriteToLocalStorage(IBackgroundTaskService backgroundTaskService, IStorageService storageService, string key, object value)
	{
		backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
			"WriteToStorage",
			async () =>
			{
				var valueJson = System.Text.Json.JsonSerializer.Serialize(value);
				await storageService.SetValue(key, valueJson).ConfigureAwait(false);
			});
	}
}