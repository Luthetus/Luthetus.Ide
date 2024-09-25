namespace Luthetus.Ide.RazorLib.AppDatas.Models;

public class DoNothingAppDataService : IAppDataService
{
	public Task WriteAppDataAsync<AppData>(AppData appData)
		where AppData : IAppData
	{
		return Task.CompletedTask;
	}
	
	public Task<AppData?> ReadAppDataAsync<AppData>(string assemblyNameFullyQualified, bool refreshCache)
		where AppData : IAppData
	{
		return Task.FromResult(default(AppData));
	}
}
