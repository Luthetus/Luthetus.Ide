using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.Models;

public class SearchEngineFileSystem : ITextEditorSearchEngine
{
	private readonly IFileSystemProvider _fileSystemProvider;
	
	private int _runCount;

	public SearchEngineFileSystem(IFileSystemProvider fileSystemProvider)
	{
		_fileSystemProvider = fileSystemProvider;
	}

    public Key<ITextEditorSearchEngine> SearchEngineKey { get; } =
        new Key<ITextEditorSearchEngine>(Guid.Parse("dd38073d-b2fb-478c-844f-8af5e61855e7"));

    public Type IconComponentRendererType { get; } = typeof(IconCopy);
    public string DisplayName { get; } = "FileSystem";
	public string StartingDirectoryPath = "C:\\Users\\hunte\\Repos\\Luthetus.Ide_Fork\\";
	public List<string> FilePathBag = new List<string>();
	public event Action? ProgressOccurred;
	public bool IsSearching;

    public async Task SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
		if (_runCount != 0)
			return;

		_runCount++;

		_ =  Task.Run(async () => 
		{
			FilePathBag.Clear();
			IsSearching = true;
			ProgressOccurred?.Invoke();
			
			await RecursiveSearchAsync(StartingDirectoryPath, searchQuery, cancellationToken);
			IsSearching = false;
			ProgressOccurred?.Invoke();
			_runCount--;
		});
    }

	private async Task RecursiveSearchAsync(string directoryPath, string searchQuery, CancellationToken cancellationToken = default)
	{
		// Considering the use a breadth first algorithm

		// Search Files
		{
			var childFileBag = await _fileSystemProvider.Directory.GetFilesAsync(
				directoryPath);

			foreach (var childFile in childFileBag)
			{
				await PerformSearchFileAsync(childFile, searchQuery, cancellationToken);
			}
		}

		// Update UI with progress
		{
			ProgressOccurred?.Invoke();
		}

		// Recurse into subdirectories
		{
			var subdirectoryBag = await _fileSystemProvider.Directory.GetDirectoriesAsync(
				directoryPath);

			foreach (var subdirectory in subdirectoryBag)
			{
				if (subdirectory.Contains(".git")  || subdirectory.Contains("bin") || subdirectory.Contains("obj"))
					continue;

				await RecursiveSearchAsync(subdirectory, searchQuery, cancellationToken);
			}
		}
	}

	private async Task PerformSearchFileAsync(string filePath, string searchQuery, CancellationToken cancellationToken = default)
	{
		var contents = await _fileSystemProvider.File.ReadAllTextAsync(filePath);

		if (contents.Contains(searchQuery))
			FilePathBag.Add(filePath);
	}
}
