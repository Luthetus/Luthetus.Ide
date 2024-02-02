using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.Displays;

public partial class SearchEngineFileSystemDisplay : ComponentBase, IDisposable
{
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;

	[Parameter, EditorRequired]
	public SearchEngineFileSystem SearchEngineFileSystem { get; set; } = null!;

	protected override void OnInitialized()
	{
		SearchEngineFileSystem.ProgressOccurred += On_SearchEngineFileSystem_ProgressOccurred;
		base.OnInitialized();
	}

	private async Task OpenInEditorOnClick(string filePath)
	{
		if (TextEditorConfig.OpenInEditorAsyncFunc is null)
			return;

		await TextEditorConfig.OpenInEditorAsyncFunc
			.Invoke(filePath, ServiceProvider)
            .ConfigureAwait(false);
	}

	private async void On_SearchEngineFileSystem_ProgressOccurred()
	{
		await InvokeAsync(StateHasChanged).ConfigureAwait(false);
	}

	public void Dispose()
	{
		SearchEngineFileSystem.ProgressOccurred -= On_SearchEngineFileSystem_ProgressOccurred;
	}
}