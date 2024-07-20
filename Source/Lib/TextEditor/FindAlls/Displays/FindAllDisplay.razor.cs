using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.TextEditor.RazorLib;
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
	private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
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
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    
    private FindAllTreeViewKeyboardEventHandler _treeViewKeymap = null!;
	private FindAllTreeViewMouseEventHandler _treeViewMouseEventHandler = null!;
    
    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
		AppOptionsStateWrap.Value.Options.IconSizeInPixels * (2.0 / 3.0));

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
    
    protected override void OnInitialized()
	{
		_treeViewKeymap = new FindAllTreeViewKeyboardEventHandler(
			TextEditorService,
			TextEditorConfig,
			ServiceProvider,
			TreeViewService,
			BackgroundTaskService);

		_treeViewMouseEventHandler = new FindAllTreeViewMouseEventHandler(
			TextEditorService,
			TextEditorConfig,
			ServiceProvider,
			TreeViewService,
			BackgroundTaskService);

		base.OnInitialized();
	}
	
	private Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
	{
		var dropdownRecord = new DropdownRecord(
			FindAllDisplayContextMenu.ContextMenuEventDropdownKey,
			treeViewCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels,
			treeViewCommandArgs.ContextMenuFixedPosition.TopPositionInPixels,
			typeof(FindAllDisplayContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(FindAllDisplayContextMenu.TreeViewCommandArgs),
					treeViewCommandArgs
				}
			},
			null);

		Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
		return Task.CompletedTask;
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