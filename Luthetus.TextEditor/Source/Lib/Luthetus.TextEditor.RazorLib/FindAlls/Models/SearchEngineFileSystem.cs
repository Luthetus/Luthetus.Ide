using Fluxor;
using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.States;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public class SearchEngineFileSystem : ITextEditorSearchEngine
{
	private readonly IFileSystemProvider _fileSystemProvider;
	private readonly IState<TextEditorFindAllState> _textEditorFindAllStateWrap;
	
	private int _runCount;

	public SearchEngineFileSystem(
		IFileSystemProvider fileSystemProvider,
		IState<TextEditorFindAllState> textEditorFindAllStateWrap)
	{
		_fileSystemProvider = fileSystemProvider;
		_textEditorFindAllStateWrap = textEditorFindAllStateWrap;
	}

    public Key<ITextEditorSearchEngine> Key { get; } =
        new Key<ITextEditorSearchEngine>(Guid.Parse("dd38073d-b2fb-478c-844f-8af5e61855e7"));

    public Type IconComponentRendererType { get; } = typeof(IconCopy);
    public string DisplayName { get; } = "FileSystem";
	public List<string> FilePathList = new List<string>();
	public event Action? ProgressOccurred;
	public bool IsSearching;

    public async Task SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
		if (_runCount != 0)
			return;

		_runCount++;

		_ =  Task.Run(async () => 
		{
			var textEditorFindAllState = _textEditorFindAllStateWrap.Value;

			FilePathList.Clear();
			IsSearching = true;
			ProgressOccurred?.Invoke();
			
			await RecursiveSearchAsync(textEditorFindAllState.StartingDirectoryPath, searchQuery, cancellationToken).ConfigureAwait(false);
			IsSearching = false;
			ProgressOccurred?.Invoke();
			_runCount--;
		}).ConfigureAwait(false);
    }

	private async Task RecursiveSearchAsync(string directoryPath, string searchQuery, CancellationToken cancellationToken = default)
	{
		// Considering the use a breadth first algorithm

		// Search Files
		{
			var childFileList = await _fileSystemProvider.Directory
				.GetFilesAsync(directoryPath)
				.ConfigureAwait(false);

			foreach (var childFile in childFileList)
			{
				await PerformSearchFileAsync(childFile, searchQuery, cancellationToken).ConfigureAwait(false);
			}
		}

		// Update UI with progress
		{
			ProgressOccurred?.Invoke();
		}

		// Recurse into subdirectories
		{
			var subdirectoryList = await _fileSystemProvider.Directory
				.GetDirectoriesAsync(directoryPath)
				.ConfigureAwait(false);

			foreach (var subdirectory in subdirectoryList)
			{
				if (subdirectory.Contains(".git")  || subdirectory.Contains("bin") || subdirectory.Contains("obj"))
					continue;

				await RecursiveSearchAsync(subdirectory, searchQuery, cancellationToken).ConfigureAwait(false);
			}
		}
	}

	private async Task PerformSearchFileAsync(string filePath, string searchQuery, CancellationToken cancellationToken = default)
	{
		var contents = await _fileSystemProvider.File.ReadAllTextAsync(filePath).ConfigureAwait(false);

		if (contents.Contains(searchQuery))
			FilePathList.Add(filePath);
	}
}
