using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Displays;

public partial class FindAllDisplay : ComponentBase, IDisposable
{
	[Inject]
    private IFindAllService FindAllService { get; set; } = null!;
    [Inject]
	private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;	
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private IDropdownService DropdownService { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private TextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    
    private FindAllTreeViewKeyboardEventHandler _treeViewKeymap = null!;
	private FindAllTreeViewMouseEventHandler _treeViewMouseEventHandler = null!;
    
    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
		AppOptionsService.GetAppOptionsState().Options.IconSizeInPixels * (2.0 / 3.0));

	private string SearchQuery
    {
        get => FindAllService.GetFindAllState().SearchQuery;
        set
        {
            if (value is not null)
                FindAllService.SetSearchQuery(value);
        }
    }

	private string StartingDirectoryPath
    {
        get => FindAllService.GetFindAllState().StartingDirectoryPath;
        set
        {
            if (value is not null)
                FindAllService.SetStartingDirectoryPath(value);
        }
    }
    
    protected override void OnInitialized()
	{
		FindAllService.FindAllStateChanged += OnFindAllStateChanged;
	
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
			FindAllContextMenu.ContextMenuEventDropdownKey,
			treeViewCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels,
			treeViewCommandArgs.ContextMenuFixedPosition.TopPositionInPixels,
			typeof(FindAllContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(FindAllContextMenu.TreeViewCommandArgs),
					treeViewCommandArgs
				}
			},
			null);

		DropdownService.ReduceRegisterAction(dropdownRecord);
		return Task.CompletedTask;
	}

	private void DoSearchOnClick()
    {
    	FindAllService.HandleStartSearchAction();
    }

	private void CancelSearchOnClick()
    {
    	FindAllService.CancelSearch();
    }
    
    public async void OnFindAllStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	FindAllService.FindAllStateChanged -= OnFindAllStateChanged;
    }
}