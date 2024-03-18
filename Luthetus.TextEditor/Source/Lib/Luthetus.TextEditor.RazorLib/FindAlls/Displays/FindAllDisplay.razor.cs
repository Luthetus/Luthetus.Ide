using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Microsoft.AspNetCore.Components;

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

	private CancellationTokenSource _doSearchCancellationTokenSource = new();
    private bool _isSearching;
    private bool _disposed;

	public SearchEngineFileSystem SearchEngineFileSystem => (SearchEngineFileSystem)
		TextEditorFindAllStateWrap.Value.SearchEngineList
			.FirstOrDefault(x => x.DisplayName == "FileSystem");

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

    private bool MatchCase
    {
        get => TextEditorFindAllStateWrap.Value.Options.MatchCase.Value;
        set
        {
            TextEditorFindAllStateWrap.Value.Options.MatchCase.Value = value;
        }
    }

    private bool MatchWholeWord
    {
        get => TextEditorFindAllStateWrap.Value.Options.MatchWholeWord.Value;
        set
        {
            TextEditorFindAllStateWrap.Value.Options.MatchWholeWord.Value = value;
        }
    }

    private bool UseRegularExpressions
    {
        get => TextEditorFindAllStateWrap.Value.Options.UseRegularExpressions.Value;
        set
        {
            TextEditorFindAllStateWrap.Value.Options.UseRegularExpressions.Value = value;
        }
    }

    private bool IncludeExternalItems
    {
        get => TextEditorFindAllStateWrap.Value.Options.IncludeExternalItems.Value;
        set
        {
            TextEditorFindAllStateWrap.Value.Options.IncludeExternalItems.Value = value;
        }
    }

    private bool IncludeMiscellaneousFiles
    {
        get => TextEditorFindAllStateWrap.Value.Options.IncludeMiscellaneousFiles.Value;
        set
        {
            TextEditorFindAllStateWrap.Value.Options.IncludeMiscellaneousFiles.Value = value;
        }
    }

    private bool AppendResults
    {
        get => TextEditorFindAllStateWrap.Value.Options.AppendResults.Value;
        set
        {
            TextEditorFindAllStateWrap.Value.Options.AppendResults.Value = value;
        }
    }

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

	private async Task DoSearchOnClickAsync(
        TextEditorFindAllState findAllState,
        ITextEditorSearchEngine activeSearchEngine)
    {
        try
        {
            _isSearching = true;
            await InvokeAsync(StateHasChanged);

            _doSearchCancellationTokenSource.Cancel();
            _doSearchCancellationTokenSource = new();

            var cancellationToken = _doSearchCancellationTokenSource.Token;

            await activeSearchEngine
                .SearchAsync(findAllState.SearchQuery, cancellationToken);
        }
        finally
        {
            _isSearching = false;
            await InvokeAsync(StateHasChanged);
        }
    }

	private async void On_SearchEngineFileSystem_ProgressOccurred()
	{
		await InvokeAsync(StateHasChanged);
	}

	protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _disposed = true;

            _doSearchCancellationTokenSource.Cancel();
        	SearchEngineFileSystem.ProgressOccurred -= On_SearchEngineFileSystem_ProgressOccurred;
		}

        base.Dispose(disposing);
    }
}