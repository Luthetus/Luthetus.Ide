using Microsoft.JSInterop;
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

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class CommonBackgroundTaskApi : IBackgroundTaskGroup
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;
    private readonly IAppOptionsService _appOptionsService;
    private readonly IContextService _contextService;
    private readonly IPanelService _panelService;
    private readonly IDialogService _dialogService;
    private readonly LuthetusCommonConfig _commonConfig;

    public CommonBackgroundTaskApi(
		IBackgroundTaskService backgroundTaskService,
		IStorageService storageService,
		IAppOptionsService appOptionsService,
		IContextService contextService,
		IPanelService panelService,
        IDialogService dialogService,
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
        _commonConfig = commonConfig;

        Storage = new StorageCommonApi(
            _backgroundTaskService,
            _storageService);
            
        JsRuntimeCommonApi = jsRuntime.GetLuthetusCommonApi();
    }

    public StorageCommonApi Storage { get; }
    public LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi { get; }
    
    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(CommonBackgroundTaskApi);
    public bool EarlyBatchEnabled { get; } = false;
    
    public bool __TaskCompletionSourceWasCreated { get; set; }
    
    public void EnqueueLuthetusCommonInitializer()
    {
    	_backgroundTaskService.EnqueueGroup(this);
    }

	public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
	{
		return null;
	}
	
	public ValueTask HandleEvent(CancellationToken cancellationToken)
	{
		return LuthetusCommonInitializerWork(LuthetusCommonInitializer.ContextSwitchGroupKey);
	}
	
	private async ValueTask LuthetusCommonInitializerWork(Key<ContextSwitchGroup> contextSwitchGroupKey)
	{
        _appOptionsService.SetActiveThemeRecordKey(_commonConfig.InitialThemeKey, false);

        await _appOptionsService
            .SetFromLocalStorageAsync()
            .ConfigureAwait(false);

		_contextService.GetContextSwitchState().FocusInitiallyContextSwitchGroupKey = contextSwitchGroupKey;                    
        _contextService.ReduceRegisterContextSwitchGroupAction(
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
									_panelService.ReduceSetActivePanelTabAction(panelGroup.Key, panel.Key);
									
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
										_panelService.ReduceRegisterPanelTabAction(PanelFacts.LeftPanelGroupKey, panel, true);
										_panelService.ReduceSetActivePanelTabAction(PanelFacts.LeftPanelGroupKey, panel.Key);
										
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
						? MenuRecord.GetEmpty()
						: new MenuRecord(menuOptionList);
						
					return Task.FromResult(menu);
				}));
		
	}
}
