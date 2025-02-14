using System.Text.Json;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.AppDatas.Models;

public class NativeAppDataService : IAppDataService
{
	private readonly object _cacheLock = new();
	
	/// <summary>
	/// Key is the assembly name fully qualified.
	/// </summary>
	private readonly Dictionary<string, IAppData> _appDataMap = new();
	
	private readonly LuthetusCommonApi _commonApi;

	public NativeAppDataService(LuthetusCommonApi commonApi)
	{
        _commonApi = commonApi;
	}
	
	private bool _isInitialized;

	public async Task WriteAppDataAsync<AppData>(AppData appData)
		where AppData : IAppData
	{
		if (!_isInitialized)
		{
			_isInitialized = true;
			
			var directoryPath = _commonApi.EnvironmentProviderApi.SafeRoamingApplicationDataDirectoryAbsolutePath.Value;
			
			var directoryExists = await _commonApi.FileSystemProviderApi.Directory
				.ExistsAsync(directoryPath)
				.ConfigureAwait(false);
			
			if (!directoryExists)
			{
				await _commonApi.FileSystemProviderApi.Directory
					.CreateDirectoryAsync(directoryPath)
					.ConfigureAwait(false);
			}

            _commonApi.EnvironmentProviderApi.DeletionPermittedRegister(
				new SimplePath(directoryPath, true));
		}
		
		var options = new JsonSerializerOptions { WriteIndented = true };
	
		await _commonApi.FileSystemProviderApi.File.WriteAllTextAsync(
		        GetFilePath(appData.AssemblyName, appData.TypeName, appData.UniqueIdentifier),
		        JsonSerializer.Serialize(appData, options))
	        .ConfigureAwait(false);
	    
	    // TODO: This will update cache, but in a bad, round-about way. This is bad it reads for no reason we already have it.    
	    await ReadAppDataAsync<AppData>(appData.AssemblyName, appData.TypeName, appData.UniqueIdentifier, forceRefreshCache: true);
	}
	
	public async Task<AppData?> ReadAppDataAsync<AppData>(
			string assemblyName,
			string typeName,
			string? uniqueIdentifier,
			bool forceRefreshCache)
		where AppData : IAppData
	{
		var appData = default(AppData);
		var path = GetFilePath(assemblyName, typeName, uniqueIdentifier);
		
		try
		{
			var success = false;
	
			lock (_cacheLock)
			{
				success = _appDataMap.TryGetValue(assemblyName, out var interfaceAppData);
				appData = (AppData)interfaceAppData;
			}
			
			if (!success || forceRefreshCache)
			{
				var appDataJson = await _commonApi.FileSystemProviderApi.File
					.ReadAllTextAsync(path)
					.ConfigureAwait(false);
				
				appData = JsonSerializer.Deserialize<AppData>(appDataJson);
				
				lock (_cacheLock)
				{
					if (_appDataMap.ContainsKey(assemblyName))
						_appDataMap[assemblyName] = appData;
					else
						_appDataMap.Add(assemblyName, appData);
				}
			}
		}
		catch (Exception e)
		{
			/*NotificationHelper.DispatchError(
		        $"{nameof(NativeAppDataService)}.{nameof(ReadAppDataAsync)}",
		        e.ToString(),
		        _commonComponentRenderers,
		        _dispatcher,
		        TimeSpan.FromSeconds(5));*/
		        
		    Console.WriteLine(e);
		}
	
		return appData;
	}
	
	public string GetFilePath(string assemblyName, string typeName, string uniqueIdentifier)
	{
		var relativePath = IAppData.CreateRelativePathNoLeadingDelimiter(
			assemblyName,
			typeName,
			uniqueIdentifier);
		
		return _commonApi.EnvironmentProviderApi.JoinPaths(
            _commonApi.EnvironmentProviderApi.SafeRoamingApplicationDataDirectoryAbsolutePath.Value,
	    	relativePath);
	}
}
