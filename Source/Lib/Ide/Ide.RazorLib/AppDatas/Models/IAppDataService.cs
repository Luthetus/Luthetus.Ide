namespace Luthetus.Ide.RazorLib.AppDatas.Models;

public interface IAppDataService
{
	public Task WriteAppDataAsync<AppData>(AppData appData) where AppData : IAppData;
	
	public Task<AppData?> ReadAppDataAsync<AppData>(
			string assemblyName,
			string typeName,
			string? uniqueIdentifier,
			bool forceRefreshCache)
		where AppData : IAppData;
}
