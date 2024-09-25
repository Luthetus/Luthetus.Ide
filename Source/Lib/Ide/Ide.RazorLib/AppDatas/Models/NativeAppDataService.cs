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

	public Task WriteAppDataAsync<AppData>(AppData appData)
		where AppData : IAppData
	{
		/*_environmentProvider.
	
		_fileSystemProvider.File.WriteAllTextAsync(
	        string absolutePathString,
	        string contents,
	        CancellationToken cancellationToken = default);*/
		
		return Task.CompletedTask;
	}
	
	public Task<AppData?> ReadAppDataAsync<AppData>(string assemblyNameFullyQualified, bool refreshCache)
		where AppData : IAppData
	{
		return Task.FromResult(default(AppData));
	}
}
