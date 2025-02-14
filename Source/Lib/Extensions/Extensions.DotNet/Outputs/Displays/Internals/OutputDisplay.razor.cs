using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.Outputs.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Displays.Internals;

public partial class OutputDisplay : ComponentBase, IDisposable
{
	[Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    [Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
    [Inject]
    private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;
    
    private readonly Throttle _eventThrottle = new Throttle(TimeSpan.FromMilliseconds(333));
    
    private OutputTreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
	private OutputTreeViewMouseEventHandler _treeViewMouseEventHandler = null!;

	private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        CommonApi.AppOptionApi.GetAppOptionsState().Options.IconSizeInPixels * (2.0 / 3.0));
    
    protected override void OnInitialized()
    {
    	_treeViewKeyboardEventHandler = new OutputTreeViewKeyboardEventHandler(
            CommonApi,
			TextEditorService,
			TextEditorConfig,
			ServiceProvider);

		_treeViewMouseEventHandler = new OutputTreeViewMouseEventHandler(
            CommonApi,
			TextEditorService,
			TextEditorConfig,
			ServiceProvider);
    
    	DotNetCliOutputParser.StateChanged += DotNetCliOutputParser_StateChanged;
    	DotNetBackgroundTaskApi.OutputService.OutputStateChanged += OnOutputStateChanged;
    	
    	if (DotNetBackgroundTaskApi.OutputService.GetOutputState().DotNetRunParseResultId != DotNetCliOutputParser.GetDotNetRunParseResult().Id)
    		DotNetCliOutputParser_StateChanged();
    	
        base.OnInitialized();
    }
    
    public void DotNetCliOutputParser_StateChanged()
    {
    	_eventThrottle.Run(_ =>
    	{
    		if (DotNetBackgroundTaskApi.OutputService.GetOutputState().DotNetRunParseResultId == DotNetCliOutputParser.GetDotNetRunParseResult().Id)
    			return Task.CompletedTask;
    			
    		DotNetBackgroundTaskApi.Output.Enqueue_ConstructTreeView();
    		return Task.CompletedTask;
    	});
    }
    
    public async void OnOutputStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    private Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
	{
		var dropdownRecord = new DropdownRecord(
			OutputContextMenu.ContextMenuEventDropdownKey,
			treeViewCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels,
			treeViewCommandArgs.ContextMenuFixedPosition.TopPositionInPixels,
			typeof(OutputContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(OutputContextMenu.TreeViewCommandArgs),
					treeViewCommandArgs
				}
			},
			restoreFocusOnClose: null);

		CommonApi.DropdownApi.ReduceRegisterAction(dropdownRecord);
		return Task.CompletedTask;
	}
    
    public void Dispose()
    {
    	DotNetCliOutputParser.StateChanged -= DotNetCliOutputParser_StateChanged;
    	DotNetBackgroundTaskApi.OutputService.OutputStateChanged -= OnOutputStateChanged;
    }
}