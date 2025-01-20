using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.Installations.Displays;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: when one first starts interacting with this project,
/// 	this type might be one of the first types they interact with. So, the redundancy of namespace
/// 	and type containing 'Luthetus' feels reasonable here.
/// </remarks>
public partial class LuthetusCommonInitializer : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonConfig CommonConfig { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    private IState<ContextState> ContextStateWrap { get; set; } = null!;
    [Inject]
    private IState<ContextSwitchState> ContextSwitchStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelState> PanelStateWrap { get; set; } = null!;
	[Inject]
    private IState<DialogState> DialogStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private BrowserResizeInterop BrowserResizeInterop { get; set; } = null!;
    [Inject]
    private LuthetusHostingInformation LuthetusHostingInformation { get; set; } = null!;
    
    public static Key<ContextSwitchGroup> ContextSwitchGroupKey { get; } = Key<ContextSwitchGroup>.NewKey();
    
    private LuthetusCommonJavaScriptInteropApi? _jsRuntimeCommonApi;
    
    private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi =>
    	_jsRuntimeCommonApi ??= JsRuntime.GetLuthetusCommonApi();
    
    /// <summary>
    /// This is to say that the order of the <Luthetus...Initializer/> components
    /// in the markup matters?
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource = new();
    
    private bool _hasStartedContinuousWorker = false;
    private bool _hasStartedIndefiniteWorker = false;

	protected override void OnInitialized()
	{
		BackgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskService.ContinuousTaskWorker.Queue.Key,
            nameof(LuthetusCommonInitializer),
            async () =>
            {
                AppOptionsService.SetActiveThemeRecordKey(CommonConfig.InitialThemeKey, false);

                await AppOptionsService
                    .SetFromLocalStorageAsync()
                    .ConfigureAwait(false);

				ContextSwitchStateWrap.Value.FocusInitiallyContextSwitchGroupKey = ContextSwitchGroupKey;                    
                Dispatcher.Dispatch(new ContextSwitchState.RegisterContextSwitchGroupAction(
                	new ContextSwitchGroup(
                		ContextSwitchGroupKey,
						"Contexts",
						() =>
						{
							var contextState = ContextStateWrap.Value;
							var panelState = PanelStateWrap.Value;
							var dialogState = DialogStateWrap.Value;
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
											Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(panelGroup.Key, panel.Key));
											
											var contextRecord = ContextFacts.AllContextsList.FirstOrDefault(x => x.ContextKey == panel.ContextRecordKey);
											
											if (contextRecord is not null)
											{
												var command = ContextHelper.ConstructFocusContextElementCommand(
											        contextRecord,
											        nameof(ContextHelper.ConstructFocusContextElementCommand),
											        nameof(ContextHelper.ConstructFocusContextElementCommand),
											        JsRuntimeCommonApi,
											        Dispatcher);
											        
											    await command.CommandFunc.Invoke(null).ConfigureAwait(false);
											}
										}
										else
										{
											var existingDialog = dialogState.DialogList.FirstOrDefault(
												x => x.DynamicViewModelKey == panel.DynamicViewModelKey);
											
											if (existingDialog is not null)
											{
												Dispatcher.Dispatch(new DialogState.SetActiveDialogKeyAction(existingDialog.DynamicViewModelKey));
												
												await JsRuntimeCommonApi
									                .FocusHtmlElementById(existingDialog.DialogFocusPointHtmlElementId)
									                .ConfigureAwait(false);
											}
											else
											{
												Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(PanelFacts.LeftPanelGroupKey, panel, true));
												Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(PanelFacts.LeftPanelGroupKey, panel.Key));
												
												var contextRecord = ContextFacts.AllContextsList.FirstOrDefault(x => x.ContextKey == panel.ContextRecordKey);
											
												if (contextRecord is not null)
												{
													var command = ContextHelper.ConstructFocusContextElementCommand(
												        contextRecord,
												        nameof(ContextHelper.ConstructFocusContextElementCommand),
												        nameof(ContextHelper.ConstructFocusContextElementCommand),
												        JsRuntimeCommonApi,
												        Dispatcher);
												        
												    await command.CommandFunc.Invoke(null).ConfigureAwait(false);
												}
											}
										}
									});
						
						        menuOptionList.Add(menuOptionPanel);
							}
						
							var menu = menuOptionList.Count == 0
								? MenuRecord.Empty
								: new MenuRecord(menuOptionList.ToImmutableArray());
								
							return Task.FromResult(menu);
						})));
            });
	
		base.OnInitialized();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			var token = _cancellationTokenSource.Token;

			if (BackgroundTaskService.ContinuousTaskWorker.StartAsyncTask is null)
			{
				_hasStartedContinuousWorker = true;

				BackgroundTaskService.ContinuousTaskWorker.StartAsyncTask = Task.Run(
					() => BackgroundTaskService.ContinuousTaskWorker.StartAsync(token),
					token);
			}

			if (LuthetusHostingInformation.LuthetusPurposeKind == LuthetusPurposeKind.Ide)
			{
				if (BackgroundTaskService.IndefiniteTaskWorker.StartAsyncTask is null)
				{
					_hasStartedIndefiniteWorker = true;

					BackgroundTaskService.IndefiniteTaskWorker.StartAsyncTask = Task.Run(
						() => BackgroundTaskService.IndefiniteTaskWorker.StartAsync(token),
						token);
				}
			}

			BrowserResizeInterop.SubscribeWindowSizeChanged(JsRuntimeCommonApi);
		}

		base.OnAfterRender(firstRender);
	}
    
    public void Dispose()
    {
    	BrowserResizeInterop.DisposeWindowSizeChanged(JsRuntimeCommonApi);
    	_cancellationTokenSource.Cancel();
    	_cancellationTokenSource.Dispose();
    	
    	if (_hasStartedContinuousWorker)
    		BackgroundTaskService.ContinuousTaskWorker.StartAsyncTask = null;
    		
    	if (_hasStartedIndefiniteWorker)
    		BackgroundTaskService.IndefiniteTaskWorker.StartAsyncTask = null;
    }
}