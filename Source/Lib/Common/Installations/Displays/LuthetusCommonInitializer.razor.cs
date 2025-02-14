using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
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
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    
    public static Key<ContextSwitchGroup> ContextSwitchGroupKey { get; } = Key<ContextSwitchGroup>.NewKey();
    
    /// <summary>
    /// This is to say that the order of the <Luthetus...Initializer/> components
    /// in the markup matters?
    /// </summary>
    private CancellationTokenSource _cancellationTokenSource = new();
    
    private bool _hasStartedContinuousWorker = false;
    private bool _hasStartedIndefiniteWorker = false;

	protected override void OnInitialized()
	{
        CommonApi.BackgroundTaskApi.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            CommonApi.BackgroundTaskApi.ContinuousTaskWorker.Queue.Key,
            nameof(LuthetusCommonInitializer),
            async () =>
            {
                CommonApi.AppOptionApi.SetActiveThemeRecordKey(CommonApi.CommonConfigApi.InitialThemeKey, false);

                await CommonApi.AppOptionApi
                    .SetFromLocalStorageAsync()
                    .ConfigureAwait(false);

                CommonApi.ContextApi.GetContextSwitchState().FocusInitiallyContextSwitchGroupKey = ContextSwitchGroupKey;
                CommonApi.ContextApi.ReduceRegisterContextSwitchGroupAction(
                	new ContextSwitchGroup(
                		ContextSwitchGroupKey,
						"Contexts",
						() =>
						{
							var contextState = CommonApi.ContextApi.GetContextState();
							var panelState = CommonApi.PanelApi.GetPanelState();
							var dialogState = CommonApi.DialogApi.GetDialogState();
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
                                            CommonApi.PanelApi.ReduceSetActivePanelTabAction(panelGroup.Key, panel.Key);
											
											var contextRecord = ContextFacts.AllContextsList.FirstOrDefault(x => x.ContextKey == panel.ContextRecordKey);
											
											if (contextRecord != default)
											{
												var command = ContextHelper.ConstructFocusContextElementCommand(
											        contextRecord,
											        nameof(ContextHelper.ConstructFocusContextElementCommand),
											        nameof(ContextHelper.ConstructFocusContextElementCommand),
                                                    CommonApi.LuthetusCommonJavaScriptInteropApi,
                                                    CommonApi.PanelApi);
											        
											    await command.CommandFunc.Invoke(null).ConfigureAwait(false);
											}
										}
										else
										{
											var existingDialog = dialogState.DialogList.FirstOrDefault(
												x => x.DynamicViewModelKey == panel.DynamicViewModelKey);
											
											if (existingDialog is not null)
											{
                                                CommonApi.DialogApi.ReduceSetActiveDialogKeyAction(existingDialog.DynamicViewModelKey);
												
												await CommonApi.LuthetusCommonJavaScriptInteropApi
                                                    .FocusHtmlElementById(existingDialog.DialogFocusPointHtmlElementId)
									                .ConfigureAwait(false);
											}
											else
											{
                                                CommonApi.PanelApi.ReduceRegisterPanelTabAction(PanelFacts.LeftPanelGroupKey, panel, true);
                                                CommonApi.PanelApi.ReduceSetActivePanelTabAction(PanelFacts.LeftPanelGroupKey, panel.Key);
												
												var contextRecord = ContextFacts.AllContextsList.FirstOrDefault(x => x.ContextKey == panel.ContextRecordKey);
											
												if (contextRecord != default)
												{
													var command = ContextHelper.ConstructFocusContextElementCommand(
												        contextRecord,
												        nameof(ContextHelper.ConstructFocusContextElementCommand),
												        nameof(ContextHelper.ConstructFocusContextElementCommand),
                                                        CommonApi.LuthetusCommonJavaScriptInteropApi,
                                                        CommonApi.PanelApi);
												        
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
            });
	
		base.OnInitialized();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			var token = _cancellationTokenSource.Token;

			if (CommonApi.BackgroundTaskApi.ContinuousTaskWorker.StartAsyncTask is null)
			{
				_hasStartedContinuousWorker = true;

                CommonApi.BackgroundTaskApi.ContinuousTaskWorker.StartAsyncTask = Task.Run(
					() => CommonApi.BackgroundTaskApi.ContinuousTaskWorker.StartAsync(token),
					token);
			}

			if (CommonApi.HostingInformationApi.LuthetusPurposeKind == LuthetusPurposeKind.Ide)
			{
				if (CommonApi.BackgroundTaskApi.IndefiniteTaskWorker.StartAsyncTask is null)
				{
					_hasStartedIndefiniteWorker = true;

                    CommonApi.BackgroundTaskApi.IndefiniteTaskWorker.StartAsyncTask = Task.Run(
						() => CommonApi.BackgroundTaskApi.IndefiniteTaskWorker.StartAsync(token),
						token);
				}
			}

            CommonApi.BrowserResizeInteropApi.SubscribeWindowSizeChanged(CommonApi.LuthetusCommonJavaScriptInteropApi);
		}

		base.OnAfterRender(firstRender);
	}
    
    public void Dispose()
    {
        CommonApi.BrowserResizeInteropApi.DisposeWindowSizeChanged(CommonApi.LuthetusCommonJavaScriptInteropApi);
    	_cancellationTokenSource.Cancel();
    	_cancellationTokenSource.Dispose();
    	
    	if (_hasStartedContinuousWorker)
            CommonApi.BackgroundTaskApi.ContinuousTaskWorker.StartAsyncTask = null;
    		
    	if (_hasStartedIndefiniteWorker)
            CommonApi.BackgroundTaskApi.IndefiniteTaskWorker.StartAsyncTask = null;
    }
}