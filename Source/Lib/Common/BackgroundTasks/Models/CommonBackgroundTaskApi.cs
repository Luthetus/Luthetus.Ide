using System.Collections.Concurrent;
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
    
    private readonly ConcurrentQueue<CommonWorkArgs> _workQueue = new();

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
    
    public bool __TaskCompletionSourceWasCreated { get; set; }

    public void Enqueue(CommonWorkArgs commonWorkArgs)
    {
		_workQueue.Enqueue(commonWorkArgs);
        _backgroundTaskService.Continuous_EnqueueGroup(this);
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

    public async ValueTask Do_WriteToLocalStorage(string key, object value)
    {
        var valueJson = System.Text.Json.JsonSerializer.Serialize(value);
        await _storageService.SetValue(key, valueJson).ConfigureAwait(false);
    }

    public async ValueTask Do_Tab_ManuallyPropagateOnContextMenu(
        Func<TabContextMenuEventArgs, Task> localHandleTabButtonOnContextMenu, TabContextMenuEventArgs tabContextMenuEventArgs)
    {
        await localHandleTabButtonOnContextMenu.Invoke(tabContextMenuEventArgs).ConfigureAwait(false);
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

    public async ValueTask Do_TreeView_HandleExpansionChevronOnMouseDown(TreeViewNoType localTreeViewNoType, TreeViewContainer treeViewContainer)
    {
        await localTreeViewNoType.LoadChildListAsync().ConfigureAwait(false);
        _treeViewService.ReduceReRenderNodeAction(treeViewContainer.Key, localTreeViewNoType);
    }

    public async ValueTask Do_TreeView_ManuallyPropagateOnContextMenu(Func<MouseEventArgs?, Key<TreeViewContainer>, TreeViewNoType?, Task> handleTreeViewOnContextMenu, MouseEventArgs mouseEventArgs, Key<TreeViewContainer> key, TreeViewNoType treeViewNoType)
    {
        await handleTreeViewOnContextMenu.Invoke(
                mouseEventArgs,
                key,
                treeViewNoType)
            .ConfigureAwait(false);
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
    
    public ValueTask HandleEvent()
	{
		if (!_workQueue.TryDequeue(out CommonWorkArgs workArgs))
			return ValueTask.CompletedTask;
			
		switch (workArgs.WorkKind)
		{
			case CommonWorkKind.LuthetusCommonInitializerWork:
				return Do_LuthetusCommonInitializer(LuthetusCommonInitializer.ContextSwitchGroupKey);
			case CommonWorkKind.WriteToLocalStorage:
				return Do_WriteToLocalStorage(workArgs.WriteToLocalStorage_Key, workArgs.WriteToLocalStorage_Value);
			case CommonWorkKind.Tab_ManuallyPropagateOnContextMenu:
				return Do_Tab_ManuallyPropagateOnContextMenu(workArgs.HandleTabButtonOnContextMenu, workArgs.TabContextMenuEventArgs);
			case CommonWorkKind.TreeView_HandleTreeViewOnContextMenu:
				return Do_TreeView_HandleTreeViewOnContextMenu(workArgs.OnContextMenuFunc, workArgs.TreeViewContextMenuCommandArgs);
            case CommonWorkKind.TreeView_HandleExpansionChevronOnMouseDown:
				return Do_TreeView_HandleExpansionChevronOnMouseDown(workArgs.TreeViewNoType, workArgs.TreeViewContainer);
            case CommonWorkKind.TreeView_ManuallyPropagateOnContextMenu:
				return Do_TreeView_ManuallyPropagateOnContextMenu(workArgs.HandleTreeViewOnContextMenu, workArgs.MouseEventArgs, workArgs.ContainerKey, workArgs.TreeViewNoType);
            case CommonWorkKind.TreeViewService_LoadChildList:
				return Do_TreeViewService_LoadChildList(workArgs.ContainerKey, workArgs.TreeViewNoType);
			default:
				Console.WriteLine($"{nameof(CommonBackgroundTaskApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
		}
	}
}
