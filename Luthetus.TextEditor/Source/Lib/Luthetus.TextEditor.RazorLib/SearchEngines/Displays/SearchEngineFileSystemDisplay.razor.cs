using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;

namespace Luthetus.TextEditor.RazorLib.SearchEngines.Displays;

public partial class SearchEngineFileSystemDisplay : ComponentBase, IDisposable
{
	[Parameter, EditorRequired]
	public SearchEngineFileSystem SearchEngineFileSystem { get; set; } = null!;

	protected override void OnInitialized()
	{
		SearchEngineFileSystem.ProgressOccurred += On_SearchEngineFileSystem_ProgressOccurred;
		base.OnInitialized();
	}

	private async void On_SearchEngineFileSystem_ProgressOccurred()
	{
		await InvokeAsync(StateHasChanged);
	}

	public void Dispose()
	{
		SearchEngineFileSystem.ProgressOccurred -= On_SearchEngineFileSystem_ProgressOccurred;
	}
}