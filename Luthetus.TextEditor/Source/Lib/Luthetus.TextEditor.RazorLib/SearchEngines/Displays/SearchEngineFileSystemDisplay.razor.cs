using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;

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
		var resourceUri = new ResourceUri(filePath);

        if (TextEditorConfig.RegisterModelFunc is null)
			return;

        await TextEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
                resourceUri,
                ServiceProvider));

        if (TextEditorConfig.TryRegisterViewModelFunc is not null)
		{
			var viewModelKey = await TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
				Key<TextEditorViewModel>.NewKey(),
                resourceUri,
                new TextEditorCategory("main"),
				false,
				ServiceProvider));

            if (viewModelKey != Key<TextEditorViewModel>.Empty &&
				TextEditorConfig.TryShowViewModelFunc is not null)
            {
				await TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
					viewModelKey,
					Key<TextEditorGroup>.Empty,
					ServiceProvider));
            }
        }
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