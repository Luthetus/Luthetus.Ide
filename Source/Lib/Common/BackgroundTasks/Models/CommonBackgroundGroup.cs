using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class CommonBackgroundGroup : IBackgroundTaskGroup
{
	private readonly object _workKindQueueLock = new();
	private readonly IBackgroundTaskService _backgroundTaskService;
	
	public CommonBackgroundGroup(IBackgroundTaskService backgroundTaskService)
	{
		_backgroundTaskService = backgroundTaskService;
	}
	
	private bool _taskCompletionSourceWasCreated;

	public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    
    // Nervous about this not being considered an interpolated constant string.
    public string Name { get; } = "TextEditorWorker";
    
    public bool EarlyBatchEnabled { get; } = false;
    
    public bool __TaskCompletionSourceWasCreated
    {
    	get => _taskCompletionSourceWasCreated;
    	set => _ = value;
    }
    
    public Queue<byte> LuthetusCommonInitializerQueue { get; } = new();
    public Queue<byte> Tab_ManuallyPropagateOnContextMenuQueue { get; } = new();
    public Queue<byte> TreeView_HandleTreeViewOnContextMenuQueue { get; } = new();
    public Queue<byte> TreeView_HandleExpansionChevronOnMouseDownQueue { get; } = new();
    public Queue<byte> TreeView_ManuallyPropagateOnContextMenuQueue { get; } = new();
	
	/// <summary>
	/// If multiple EventKind of the same are enqueued one after another then
	/// better to have this Queue be a struct that has the count of contiguous work kind enqueues?
	/// </summary>
	public Queue<CommonWorkKind> WorkKindQueue { get; } = new();
	
	public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
	{
		return null;
	}
	
	public void EnqueueLuthetusCommonInitializer(byte value)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(CommonWorkKind.LuthetusCommonInitializer);
			LuthetusCommonInitializerQueue.Enqueue(value);
		}
		
		_backgroundTaskService.EnqueueGroup(this);
	}
	
	public void EnqueueTab_ManuallyPropagateOnContextMenu(byte value)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(CommonWorkKind.Tab_ManuallyPropagateOnContextMenu);
			Tab_ManuallyPropagateOnContextMenuQueue.Enqueue(value);
		}
		
		_backgroundTaskService.EnqueueGroup(this);
	}
	
	public void EnqueueTreeView_HandleTreeViewOnContextMenu(byte value)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(CommonWorkKind.TreeView_HandleTreeViewOnContextMenu);
			TreeView_HandleTreeViewOnContextMenuQueue.Enqueue(value);
		}
		
		_backgroundTaskService.EnqueueGroup(this);
	}
	
	public void EnqueueTreeView_HandleExpansionChevronOnMouseDownQueue(byte value)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(CommonWorkKind.TreeView_HandleExpansionChevronOnMouseDown);
			TreeView_HandleExpansionChevronOnMouseDownQueue.Enqueue(value);
		}
		
		_backgroundTaskService.EnqueueGroup(this);
	}
	
	public void EnqueueTreeView_ManuallyPropagateOnContextMenu(byte value)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(CommonWorkKind.TreeView_ManuallyPropagateOnContextMenu);
			TreeView_ManuallyPropagateOnContextMenuQueue.Enqueue(value);
		}
		
		_backgroundTaskService.EnqueueGroup(this);
	}
	
	public ValueTask HandleEvent(CancellationToken cancellationToken)
	{
		CommonWorkKind workKind;
	
		// avoid UI infinite loop enqueue dequeue single work item
		// by getting the count prior to starting the yield return deqeue
		// then only dequeueing at most 'count' times.
		
		lock (_workKindQueueLock)
		{
			if (!WorkKindQueue.TryDequeue(out workKind))
				return ValueTask.CompletedTask;
		}
			
		switch (workKind)
		{
			case CommonWorkKind.LuthetusCommonInitializer:
				var luthetusCommonInitializer = LuthetusCommonInitializerQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
                return ValueTask.CompletedTask;
            case CommonWorkKind.Tab_ManuallyPropagateOnContextMenu:
				var tab_ManuallyPropagateOnContextMenu = Tab_ManuallyPropagateOnContextMenuQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
				return ValueTask.CompletedTask;
			case CommonWorkKind.TreeView_HandleTreeViewOnContextMenu:
				var treeView_HandleTreeViewOnContextMenu = TreeView_HandleTreeViewOnContextMenuQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
				return ValueTask.CompletedTask;
            case CommonWorkKind.TreeView_HandleExpansionChevronOnMouseDown:
		    	var treeView_HandleExpansionChevronOnMouseDown = TreeView_HandleExpansionChevronOnMouseDownQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
                return ValueTask.CompletedTask;
            case CommonWorkKind.TreeView_ManuallyPropagateOnContextMenu:
				var treeView_ManuallyPropagateOnContextMenu = TreeView_ManuallyPropagateOnContextMenuQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
                return ValueTask.CompletedTask;
            default:
				return ValueTask.CompletedTask;
		}
	}
	
	// AppOptionsService
	// CommonConfig
	// ContextService
	// ContextFacts
	// ContextSwitchGroup
	// PanelService
	// DialogService
	// JsRuntimeCommonApi
	
	/*public ValueTask LuthetusCommonInitializerWork(CancellationToken cancellationToken)
	{
		AppOptionsService.SetActiveThemeRecordKey(CommonConfig.InitialThemeKey, false);

        await AppOptionsService
            .SetFromLocalStorageAsync()
            .ConfigureAwait(false);

		ContextService.GetContextSwitchState().FocusInitiallyContextSwitchGroupKey = ContextSwitchGroupKey;                    
        ContextService.ReduceRegisterContextSwitchGroupAction(
        	new ContextSwitchGroup(
        		ContextSwitchGroupKey,
				"Contexts",
				() =>
				{
					var contextState = ContextService.GetContextState();
					var panelState = PanelService.GetPanelState();
					var dialogState = DialogService.GetDialogState();
					var menuOptionList = new List<MenuOptionRecord>();
					
					foreach (var panel in panelState.PanelList)
					{
				        var menuOptionPanel = new MenuOptionRecord(
							panel.Title,
				            MenuOptionKind.Delete,
				            async () => 
							{
								var panelGroup = panel.TabGroup as PanelGroup;
				
								if (panelGroup is not null)
								{
									PanelService.ReduceSetActivePanelTabAction(panelGroup.Key, panel.Key);
									
									var contextRecord = ContextFacts.AllContextsList.FirstOrDefault(x => x.ContextKey == panel.ContextRecordKey);
									
									if (contextRecord != default)
									{
										var command = ContextHelper.ConstructFocusContextElementCommand(
									        contextRecord,
									        nameof(ContextHelper.ConstructFocusContextElementCommand),
									        nameof(ContextHelper.ConstructFocusContextElementCommand),
									        JsRuntimeCommonApi,
									        PanelService);
									        
									    await command.CommandFunc.Invoke(null).ConfigureAwait(false);
									}
								}
								else
								{
									var existingDialog = dialogState.DialogList.FirstOrDefault(
										x => x.DynamicViewModelKey == panel.DynamicViewModelKey);
									
									if (existingDialog is not null)
									{
										DialogService.ReduceSetActiveDialogKeyAction(existingDialog.DynamicViewModelKey);
										
										await JsRuntimeCommonApi
							                .FocusHtmlElementById(existingDialog.DialogFocusPointHtmlElementId)
							                .ConfigureAwait(false);
									}
									else
									{
										PanelService.ReduceRegisterPanelTabAction(PanelFacts.LeftPanelGroupKey, panel, true);
										PanelService.ReduceSetActivePanelTabAction(PanelFacts.LeftPanelGroupKey, panel.Key);
										
										var contextRecord = ContextFacts.AllContextsList.FirstOrDefault(x => x.ContextKey == panel.ContextRecordKey);
									
										if (contextRecord != default)
										{
											var command = ContextHelper.ConstructFocusContextElementCommand(
										        contextRecord,
										        nameof(ContextHelper.ConstructFocusContextElementCommand),
										        nameof(ContextHelper.ConstructFocusContextElementCommand),
										        JsRuntimeCommonApi,
										        PanelService);
										        
										    await command.CommandFunc.Invoke(null).ConfigureAwait(false);
										}
									}
								}
							});
				
				        menuOptionList.Add(menuOptionPanel);
					}
				
					var menu = menuOptionList.Count == 0
						? MenuRecord.GetEmpty()
						: new MenuRecord(menuOptionList);
						
					return Task.FromResult(menu);
				}));
	}*/
}



