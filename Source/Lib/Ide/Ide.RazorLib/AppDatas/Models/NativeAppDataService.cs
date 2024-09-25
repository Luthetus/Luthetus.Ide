using System.Text.Json;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.AppDatas.Models;

public class NativeAppDataService : IAppDataService
{
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IFileSystemProvider _fileSystemProvider;

	public NativeAppDataService(IEnvironmentProvider environmentProvider, IFileSystemProvider fileSystemProvider)
	{
		_environmentProvider = environmentProvider;
		_fileSystemProvider = fileSystemProvider;
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
		        _environmentProvider.JoinPaths(
		        	_environmentProvider.SafeRoamingApplicationDataDirectoryAbsolutePath.Value,
		        	$"{appData.AssemblyNameFullyQualified}.json"),
		        JsonSerializer.Serialize(appData, options))
	        .ConfigureAwait(false);
	}
	
	public Task<AppData?> ReadAppDataAsync<AppData>(string assemblyNameFullyQualified, bool refreshCache)
		where AppData : IAppData
	{
		return Task.FromResult(default(AppData));
	}
}
