using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.Models;

public class SearchEngineFileSystem : ITextEditorSearchEngine
{
	private readonly IFileSystemProvider _fileSystemProvider;

	public SearchEngineFileSystem(IFileSystemProvider fileSystemProvider)
	{
		_fileSystemProvider = fileSystemProvider;
	}

    public Key<ITextEditorSearchEngine> SearchEngineKey { get; } =
        new Key<ITextEditorSearchEngine>(Guid.Parse("dd38073d-b2fb-478c-844f-8af5e61855e7"));

    public Type IconComponentRendererType { get; } = typeof(IconCopy);
    public string DisplayName { get; } = "FileSystem";

    public async Task SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        await Task.Delay(5_000);
    }
}