using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Widgets.States;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Contexts.States;

namespace Luthetus.Common.RazorLib.Installations.Displays;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: when one first starts interacting with this project,
/// 	this type might be one of the first types they interact with. So, the redundancy of namespace
/// 	and type containing 'Luthetus' feels reasonable here.
/// </remarks>
public partial class LuthetusCommonInitializer : ComponentBase
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
    private IDispatcher Dispatcher { get; set; } = null!;
    
    public static Key<ContextSwitchGroup> ContextSwitchGroupKey { get; } = Key<ContextSwitchGroup>.NewKey();

	protected override void OnInitialized()
	{
		BackgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            nameof(LuthetusCommonInitializer),
            async () =>
            {
                AppOptionsService.SetActiveThemeRecordKey(CommonConfig.InitialThemeKey, false);

                await AppOptionsService
                    .SetFromLocalStorageAsync()
                    .ConfigureAwait(false);
                    
                Dispatcher.Dispatch(new ContextSwitchState.RegisterContextSwitchGroupAction(
                	new ContextSwitchGroup(
                		ContextSwitchGroupKey,
						"Contexts",
						() =>
						{
							var contextState = ContextStateWrap.Value;
							var menuOptionList = new List<MenuOptionRecord>();
										
							foreach (var context in contextState.AllContextsList)
					        {
					        	menuOptionList.Add(new MenuOptionRecord(
					        		context.DisplayNameFriendly,
					        		MenuOptionKind.Other,
					        		OnClickFunc: () =>
					        		{
					        			Dispatcher.Dispatch(new WidgetState.SetWidgetAction(null));
					        			return Task.CompletedTask;
					        		}));
					        }
							
							var menu = menuOptionList.Count == 0
								? MenuRecord.Empty
								: new MenuRecord(menuOptionList.ToImmutableArray());
								
							return Task.FromResult(menu);
						})));
            });
	
		base.OnInitialized();
	}
}