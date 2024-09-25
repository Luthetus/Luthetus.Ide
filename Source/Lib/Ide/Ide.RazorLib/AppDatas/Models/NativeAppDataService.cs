using System.Text.Json;
using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Ide.RazorLib.AppDatas.Models;

public class NativeAppDataService : IAppDataService
{
	private readonly object _cacheLock = new();
	
	/// <summary>
	/// Key is the assembly name fully qualified.
	/// </summary>
	private readonly Dictionary<string, IAppData> _appDataMap = new();
	
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IFileSystemProvider _fileSystemProvider;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly IDispatcher _dispatcher;

	public NativeAppDataService(
		IEnvironmentProvider environmentProvider,
		IFileSystemProvider fileSystemProvider,
		ICommonComponentRenderers commonComponentRenderers,
		IDispatcher dispatcher)
	{
		_environmentProvider = environmentProvider;
		_fileSystemProvider = fileSystemProvider;
		_commonComponentRenderers = commonComponentRenderers;
		_dispatcher = dispatcher;
	}
	
	private bool _isInitialized;

	public async Task WriteAppDataAsync<AppData>(AppData appData)
		where AppData : IAppData
	{
		if (!_isInitialized)
		{
			_isInitialized = true;
			
			var directoryPath = _environmentProvider.SafeRoamingApplicationDataDirectoryAbsolutePath.Value;
			
			var directoryExists = await _fileSystemProvider.Directory
				.ExistsAsync(directoryPath)
				.ConfigureAwait(false);
			
			if (!directoryExists)
			{
				await _fileSystemProvider.Directory
					.CreateDirectoryAsync(directoryPath)
					.ConfigureAwait(false);
			}
			
			_environmentProvider.DeletionPermittedRegister(
				new SimplePath(directoryPath, true));
		}
		
		var options = new JsonSerializerOptions { WriteIndented = true };
	
		await _fileSystemProvider.File.WriteAllTextAsync(
		        GetFilePath(appData.AssemblyNameFullyQualified),
		        JsonSerializer.Serialize(appData, options))
	        .ConfigureAwait(false);
	}
	
	public async Task<AppData?> ReadAppDataAsync<AppData>(string assemblyNameFullyQualified, bool refreshCache)
		where AppData : IAppData
	{
		var appData = default(AppData);
		var path = GetFilePath(assemblyNameFullyQualified);
		
		try
		{
			var success = false;
	
			lock (_cacheLock)
			{
				success = _appDataMap.TryGetValue(assemblyNameFullyQualified, out var interfaceAppData);
				appData = (AppData)interfaceAppData;
			}
			
			if (!success || refreshCache)
			{
				var appDataJson = await _fileSystemProvider.File
					.ReadAllTextAsync(path)
					.ConfigureAwait(false);
				
				appData = JsonSerializer.Deserialize<AppData>(appDataJson);
				
				lock (_cacheLock)
				{
					if (_appDataMap.ContainsKey(assemblyNameFullyQualified))
						_appDataMap[assemblyNameFullyQualified] = appData;
					else
						_appDataMap.Add(assemblyNameFullyQualified, appData);
				}
			}
		}
		catch (Exception e)
		{
			NotificationHelper.DispatchError(
		        $"{nameof(NativeAppDataService)}.{nameof(ReadAppDataAsync)}",
		        e.ToString(),
		        _commonComponentRenderers,
		        _dispatcher,
		        TimeSpan.FromSeconds(5));
		}
	
		return appData;
	}
	
	public string GetFilePath(string assemblyNameFullyQualified)
	{
		return _environmentProvider.JoinPaths(
	    	_environmentProvider.SafeRoamingApplicationDataDirectoryAbsolutePath.Value,
	    	$"{assemblyNameFullyQualified}.json");
	}
}
