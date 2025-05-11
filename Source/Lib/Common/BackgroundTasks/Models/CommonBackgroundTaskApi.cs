using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Installations.Displays;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// <summary>
/// This seems to be working.
/// But this code is very experimental.
/// Just feeling things out at the moment.
/// </summary>
public class CommonBackgroundTaskApi : IBackgroundTaskGroup
{
	private readonly BackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;
    private readonly IAppOptionsService _appOptionsService;
    private readonly IContextService _contextService;
    private readonly IPanelService _panelService;
    private readonly IDialogService _dialogService;
    private readonly ITreeViewService _treeViewService;
    private readonly LuthetusCommonConfig _commonConfig;

    public CommonBackgroundTaskApi(
		BackgroundTaskService backgroundTaskService,
		IStorageService storageService,
		IAppOptionsService appOptionsService,
		IContextService contextService,
		IPanelService panelService,
        IDialogService dialogService,
        ITreeViewService treeViewService,
        LuthetusCommonConfig commonConfig,
        IJSRuntime jsRuntime)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;

        _appOptionsService = appOptionsService;
		_appOptionsService.CommonBackgroundTaskApi = this;

        _contextService = contextService;
        _panelService = panelService;
        _dialogService = dialogService;
        _treeViewService = treeViewService;
        _commonConfig = commonConfig;
            
        JsRuntimeCommonApi = jsRuntime.GetLuthetusCommonApi();

        _treeViewService.CommonBackgroundTaskApi = this;
    }

    public LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi { get; }
    
    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(CommonBackgroundTaskApi);
    public bool EarlyBatchEnabled { get; } = false;
    
    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<CommonWorkKind> _workKindQueue = new();
    
    private readonly object _workLock = new();

    public void Enqueue_LuthetusCommonInitializer()
    {
    	lock (_workLock)
    	{
    		_workKindQueue.Enqueue(CommonWorkKind.LuthetusCommonInitializerWork);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    private async ValueTask Do_LuthetusCommonInitializer(Key<ContextSwitchGroup> contextSwitchGroupKey)
    {
        _appOptionsService.SetActiveThemeRecordKey(_commonConfig.InitialThemeKey, false);

        await _appOptionsService
            .SetFromLocalStorageAsync()
            .ConfigureAwait(false);

        _contextService.GetContextSwitchState().FocusInitiallyContextSwitchGroupKey = contextSwitchGroupKey;
        _contextService.RegisterContextSwitchGroup(
            new ContextSwitchGroup(
                contextSwitchGroupKey,
                "Contexts",
                () =>
                {
                    var contextState = _contextService.GetContextState();
                    var panelState = _panelService.GetPanelState();
                    var dialogState = _dialogService.GetDialogState();
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
                                    _panelService.SetActivePanelTab(panelGroup.Key, panel.Key);

                                    var contextRecord = ContextFacts.AllContextsList.FirstOrDefault(x => x.ContextKey == panel.ContextRecordKey);

                                    if (contextRecord != default)
                                    {
                                        var command = ContextHelper.ConstructFocusContextElementCommand(
                                            contextRecord,
                                            nameof(ContextHelper.ConstructFocusContextElementCommand),
                                            nameof(ContextHelper.ConstructFocusContextElementCommand),
                                            JsRuntimeCommonApi,
                                            _panelService);

                                        await command.CommandFunc.Invoke(null).ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    var existingDialog = dialogState.DialogList.FirstOrDefault(
                                        x => x.DynamicViewModelKey == panel.DynamicViewModelKey);

                                    if (existingDialog is not null)
                                    {
                                        _dialogService.ReduceSetActiveDialogKeyAction(existingDialog.DynamicViewModelKey);

                                        await JsRuntimeCommonApi
                                            .FocusHtmlElementById(existingDialog.DialogFocusPointHtmlElementId)
                                            .ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        _panelService.RegisterPanelTab(PanelFacts.LeftPanelGroupKey, panel, true);
                                        _panelService.SetActivePanelTab(PanelFacts.LeftPanelGroupKey, panel.Key);

                                        var contextRecord = ContextFacts.AllContextsList.FirstOrDefault(x => x.ContextKey == panel.ContextRecordKey);

                                        if (contextRecord != default)
                                        {
                                            var command = ContextHelper.ConstructFocusContextElementCommand(
                                                contextRecord,
                                                nameof(ContextHelper.ConstructFocusContextElementCommand),
                                                nameof(ContextHelper.ConstructFocusContextElementCommand),
                                                JsRuntimeCommonApi,
                                                _panelService);

                                            await command.CommandFunc.Invoke(null).ConfigureAwait(false);
                                        }
                                    }
                                }
                            });

                        menuOptionList.Add(menuOptionPanel);
                    }

                    var menu = menuOptionList.Count == 0
                        ? new MenuRecord(MenuRecord.NoMenuOptionsExistList)
                        : new MenuRecord(menuOptionList);

                    return Task.FromResult(menu);
                }));
    }

    private readonly Queue<(string Key, object Value)> _writeToLocalStorageQueue = new();

    public void Enqueue_WriteToLocalStorage(string key, object value)
    {
    	lock (_workLock)
    	{
    		_workKindQueue.Enqueue(CommonWorkKind.WriteToLocalStorage);
			_writeToLocalStorageQueue.Enqueue((key, value));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public async ValueTask Do_WriteToLocalStorage(string key, object value)
    {
        var valueJson = System.Text.Json.JsonSerializer.Serialize(value);
        await _storageService.SetValue(key, valueJson).ConfigureAwait(false);
    }

    private readonly Queue<(Func<TabContextMenuEventArgs, Task> LocalHandleTabButtonOnContextMenu, TabContextMenuEventArgs TabContextMenuEventArgs)> _tab_ManuallyPropagateOnContextMenuQueue = new();

    public void Enqueue_Tab_ManuallyPropagateOnContextMenu(
		Func<TabContextMenuEventArgs, Task> localHandleTabButtonOnContextMenu, TabContextMenuEventArgs tabContextMenuEventArgs)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(CommonWorkKind.Tab_ManuallyPropagateOnContextMenu);
            _tab_ManuallyPropagateOnContextMenuQueue.Enqueue((localHandleTabButtonOnContextMenu, tabContextMenuEventArgs));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public async ValueTask Do_Tab_ManuallyPropagateOnContextMenu(
        Func<TabContextMenuEventArgs, Task> localHandleTabButtonOnContextMenu, TabContextMenuEventArgs tabContextMenuEventArgs)
    {
        await localHandleTabButtonOnContextMenu.Invoke(tabContextMenuEventArgs).ConfigureAwait(false);
    }

    private readonly Queue<(Func<TreeViewCommandArgs, Task>? OnContextMenuFunc, TreeViewCommandArgs TreeViewContextMenuCommandArgs)> _treeView_HandleTreeViewOnContextMenuQueue = new();

    public void Enqueue_TreeView_HandleTreeViewOnContextMenu(
        Func<TreeViewCommandArgs, Task>? onContextMenuFunc, TreeViewCommandArgs treeViewContextMenuCommandArgs)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(CommonWorkKind.TreeView_HandleTreeViewOnContextMenu);
            _treeView_HandleTreeViewOnContextMenuQueue.Enqueue((onContextMenuFunc, treeViewContextMenuCommandArgs));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public async ValueTask Do_TreeView_HandleTreeViewOnContextMenu(
        Func<TreeViewCommandArgs, Task>? onContextMenuFunc, TreeViewCommandArgs treeViewContextMenuCommandArgs)
    {
        if (onContextMenuFunc is not null)
        {
            await onContextMenuFunc
                .Invoke(treeViewContextMenuCommandArgs)
                .ConfigureAwait(false);
        }
    }

    private readonly Queue<(TreeViewNoType LocalTreeViewNoType, TreeViewContainer TreeViewContainer)> _treeView_HandleExpansionChevronOnMouseDownQueue = new();

    public void Enqueue_TreeView_HandleExpansionChevronOnMouseDown(TreeViewNoType localTreeViewNoType, TreeViewContainer treeViewContainer)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(CommonWorkKind.TreeView_HandleExpansionChevronOnMouseDown);
            _treeView_HandleExpansionChevronOnMouseDownQueue.Enqueue((localTreeViewNoType, treeViewContainer));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public async ValueTask Do_TreeView_HandleExpansionChevronOnMouseDown(TreeViewNoType localTreeViewNoType, TreeViewContainer treeViewContainer)
    {
        await localTreeViewNoType.LoadChildListAsync().ConfigureAwait(false);
        _treeViewService.ReduceReRenderNodeAction(treeViewContainer.Key, localTreeViewNoType);
    }

    private readonly Queue<(Func<MouseEventArgs?, Key<TreeViewContainer>, TreeViewNoType?, Task> HandleTreeViewOnContextMenu, MouseEventArgs MouseEventArgs, Key<TreeViewContainer> Key, TreeViewNoType TreeViewNoType)> _treeView_ManuallyPropagateOnContextMenuQueue = new();

    public void Enqueue_TreeView_ManuallyPropagateOnContextMenu(Func<MouseEventArgs?, Key<TreeViewContainer>, TreeViewNoType?, Task> handleTreeViewOnContextMenu, MouseEventArgs mouseEventArgs, Key<TreeViewContainer> key, TreeViewNoType treeViewNoType)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(CommonWorkKind.TreeView_ManuallyPropagateOnContextMenu);
            _treeView_ManuallyPropagateOnContextMenuQueue.Enqueue((handleTreeViewOnContextMenu, mouseEventArgs, key, treeViewNoType));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public async ValueTask Do_TreeView_ManuallyPropagateOnContextMenu(Func<MouseEventArgs?, Key<TreeViewContainer>, TreeViewNoType?, Task> handleTreeViewOnContextMenu, MouseEventArgs mouseEventArgs, Key<TreeViewContainer> key, TreeViewNoType treeViewNoType)
    {
        await handleTreeViewOnContextMenu.Invoke(
                mouseEventArgs,
                key,
                treeViewNoType)
            .ConfigureAwait(false);
    }

    private readonly Queue<(Key<TreeViewContainer> containerKey, TreeViewNoType treeViewNoType)> _queue_TreeViewService_LoadChildList = new();

    public void Enqueue_TreeViewService_LoadChildList(Key<TreeViewContainer> containerKey, TreeViewNoType treeViewNoType)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(CommonWorkKind.TreeViewService_LoadChildList);
            _queue_TreeViewService_LoadChildList.Enqueue((containerKey, treeViewNoType));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public async ValueTask Do_TreeViewService_LoadChildList(Key<TreeViewContainer> containerKey, TreeViewNoType treeViewNoType)
    {
        try
        {
            await treeViewNoType.LoadChildListAsync().ConfigureAwait(false);

            _treeViewService.ReduceReRenderNodeAction(
                containerKey,
                treeViewNoType);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
	{
		return null;
	}
	
	public ValueTask HandleEvent(CancellationToken cancellationToken)
	{
		CommonWorkKind workKind;
		
		lock (_workLock)
		{
			if (!_workKindQueue.TryDequeue(out workKind))
				return ValueTask.CompletedTask;
		}
			
		switch (workKind)
		{
			case CommonWorkKind.LuthetusCommonInitializerWork:
			{
				return Do_LuthetusCommonInitializer(LuthetusCommonInitializer.ContextSwitchGroupKey);
			}
			case CommonWorkKind.WriteToLocalStorage:
			{
				var args = _writeToLocalStorageQueue.Dequeue();
				return Do_WriteToLocalStorage(args.Key, args.Value);
			}
			case CommonWorkKind.Tab_ManuallyPropagateOnContextMenu:
			{
				var args = _tab_ManuallyPropagateOnContextMenuQueue.Dequeue();
				return Do_Tab_ManuallyPropagateOnContextMenu(args.LocalHandleTabButtonOnContextMenu, args.TabContextMenuEventArgs);
			}
			case CommonWorkKind.TreeView_HandleTreeViewOnContextMenu:
			{
				var args = _treeView_HandleTreeViewOnContextMenuQueue.Dequeue();
				return Do_TreeView_HandleTreeViewOnContextMenu(args.OnContextMenuFunc, args.TreeViewContextMenuCommandArgs);
			}
            case CommonWorkKind.TreeView_HandleExpansionChevronOnMouseDown:
			{
				var args = _treeView_HandleExpansionChevronOnMouseDownQueue.Dequeue();
				return Do_TreeView_HandleExpansionChevronOnMouseDown(args.LocalTreeViewNoType, args.TreeViewContainer);
			}
            case CommonWorkKind.TreeView_ManuallyPropagateOnContextMenu:
			{
				var args = _treeView_ManuallyPropagateOnContextMenuQueue.Dequeue();
				return Do_TreeView_ManuallyPropagateOnContextMenu(args.HandleTreeViewOnContextMenu, args.MouseEventArgs, args.Key, args.TreeViewNoType);
			}
            case CommonWorkKind.TreeViewService_LoadChildList:
			{
				var args = _queue_TreeViewService_LoadChildList.Dequeue();
				return Do_TreeViewService_LoadChildList(args.containerKey, args.treeViewNoType);
			}
			default:
			{
				Console.WriteLine($"{nameof(CommonBackgroundTaskApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
			}
		}
	}
}
