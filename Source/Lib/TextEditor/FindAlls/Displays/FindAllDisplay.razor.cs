using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Displays;

public partial class FindAllDisplay : FluxorComponent
{
	[Inject]
    private IState<TextEditorFindAllState> TextEditorFindAllStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;	
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
	[Inject]
	private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

	private string SearchQuery
    {
        get => TextEditorFindAllStateWrap.Value.SearchQuery;
        set
        {
            if (value is not null)
                Dispatcher.Dispatch(new TextEditorFindAllState.SetSearchQueryAction(value));
        }
    }

	private string StartingDirectoryPath
    {
        get => TextEditorFindAllStateWrap.Value.StartingDirectoryPath;
        set
        {
            if (value is not null)
                Dispatcher.Dispatch(new TextEditorFindAllState.SetStartingDirectoryPathAction(value));
        }
    }

	private async Task OpenInEditorOnClick(string filePath)
	{
		var resourceUri = new ResourceUri(filePath);

        if (TextEditorConfig.RegisterModelFunc is null)
			return;

        await TextEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
                resourceUri,
                ServiceProvider))
            .ConfigureAwait(false);

        if (TextEditorConfig.TryRegisterViewModelFunc is not null)
		{
			var viewModelKey = await TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
				    Key<TextEditorViewModel>.NewKey(),
                    resourceUri,
                    new Category("main"),
				    false,
				    ServiceProvider))
                .ConfigureAwait(false);

            if (viewModelKey != Key<TextEditorViewModel>.Empty &&
				TextEditorConfig.TryShowViewModelFunc is not null)
            {
				await TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
					    viewModelKey,
					    Key<TextEditorGroup>.Empty,
					    ServiceProvider))
                    .ConfigureAwait(false);
            }
        }
	}

	private void DoSearchOnClick()
    {
    	Dispatcher.Dispatch(new TextEditorFindAllState.StartSearchAction());
    }

	private void CancelSearchOnClick()
    {
    	Dispatcher.Dispatch(new TextEditorFindAllState.CancelSearchAction());
    }
}