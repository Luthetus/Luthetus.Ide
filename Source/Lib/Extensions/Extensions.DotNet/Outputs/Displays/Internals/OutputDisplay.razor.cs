using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.Outputs.States;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.Outputs.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Displays.Internals;

public partial class OutputDisplay : ComponentBase, IDisposable
{
    [Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
    [Inject]
    private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
    private IState<OutputState> OutputStateWrap { get; set; } = null!;
	[Inject]
	private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
    
    private readonly Throttle _eventThrottle = new Throttle(TimeSpan.FromMilliseconds(333));
    
    private OutputTreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
	private OutputTreeViewMouseEventHandler _treeViewMouseEventHandler = null!;

	private int OffsetPerDepthInPixels => (int)Math.Ceiling(
		AppOptionsStateWrap.Value.Options.IconSizeInPixels * (2.0 / 3.0));
    
    protected override void OnInitialized()
    {
    	_treeViewKeyboardEventHandler = new OutputTreeViewKeyboardEventHandler(
			TextEditorService,
			TextEditorConfig,
			ServiceProvider,
			TreeViewService,
			BackgroundTaskService);

		_treeViewMouseEventHandler = new OutputTreeViewMouseEventHandler(
			TextEditorService,
			TextEditorConfig,
			ServiceProvider,
			TreeViewService,
			BackgroundTaskService);
    
    	DotNetCliOutputParser.StateChanged += DotNetCliOutputParser_StateChanged;
    	OutputStateWrap.StateChanged += OutputStateWrap_StateChanged;
    	
    	if (OutputStateWrap.Value.DotNetRunParseResultId != DotNetCliOutputParser.GetDotNetRunParseResult().Id)
    		DotNetCliOutputParser_StateChanged();
    	
        base.OnInitialized();
    }
    
    public void DotNetCliOutputParser_StateChanged()
    {
    	_eventThrottle.Run(_ =>
    	{
    		if (OutputStateWrap.Value.DotNetRunParseResultId == DotNetCliOutputParser.GetDotNetRunParseResult().Id)
    			return Task.CompletedTask;
    			
    		DotNetBackgroundTaskApi.Output.Enqueue_ConstructTreeView();
    		return Task.CompletedTask;
    	});
    }
    
    public async void OutputStateWrap_StateChanged(object? sender, EventArgs e)
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

		Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
		return Task.CompletedTask;
	}
    
    public void Dispose()
    {
    	DotNetCliOutputParser.StateChanged -= DotNetCliOutputParser_StateChanged;
    	OutputStateWrap.StateChanged -= OutputStateWrap_StateChanged;
    }
}