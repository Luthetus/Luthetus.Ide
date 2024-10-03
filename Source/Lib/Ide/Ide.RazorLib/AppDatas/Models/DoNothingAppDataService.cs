namespace Luthetus.Ide.RazorLib.AppDatas.Models;

public class DoNothingAppDataService : IAppDataService
{
	public Task WriteAppDataAsync<AppData>(AppData appData)
		where AppData : IAppData
	{
		return Task.CompletedTask;
	}
	
	public Task<AppData?> ReadAppDataAsync<AppData>(
			string assemblyName,
			string typeName,
			string? uniqueIdentifier,
			bool forceRefreshCache)
		where AppData : IAppData
	{
		return Task.FromResult(default(AppData));
	}
}
