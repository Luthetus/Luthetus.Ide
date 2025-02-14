using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Displays;

public partial class FindAllDisplay : ComponentBase, IDisposable
{
	[Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
	[Inject]
    private IFindAllService FindAllService { get; set; } = null!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;	
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private FindAllTreeViewKeyboardEventHandler _treeViewKeymap = null!;
	private FindAllTreeViewMouseEventHandler _treeViewMouseEventHandler = null!;
    
    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
		CommonApi.AppOptionApi.GetAppOptionsState().Options.IconSizeInPixels * (2.0 / 3.0));

	private string SearchQuery
    {
        get => FindAllService.GetFindAllState().SearchQuery;
        set
        {
            if (value is not null)
                FindAllService.ReduceSetSearchQueryAction(value);
        }
    }

	private string StartingDirectoryPath
    {
        get => FindAllService.GetFindAllState().StartingDirectoryPath;
        set
        {
            if (value is not null)
                FindAllService.ReduceSetStartingDirectoryPathAction(value);
        }
    }
    
    protected override void OnInitialized()
	{
		FindAllService.FindAllStateChanged += OnFindAllStateChanged;
	
		_treeViewKeymap = new FindAllTreeViewKeyboardEventHandler(
			CommonApi,
			TextEditorService,
			TextEditorConfig,
			ServiceProvider);

		_treeViewMouseEventHandler = new FindAllTreeViewMouseEventHandler(
			CommonApi,
			TextEditorService,
			TextEditorConfig,
			ServiceProvider);

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

		CommonApi.DropdownApi.ReduceRegisterAction(dropdownRecord);
		return Task.CompletedTask;
	}

	private void DoSearchOnClick()
    {
    	FindAllService.HandleStartSearchAction();
    }

	private void CancelSearchOnClick()
    {
    	FindAllService.ReduceCancelSearchAction();
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