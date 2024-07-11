using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextSwitchDisplay : ComponentBase
{
	[Inject]
	private IState<ContextState> ContextStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	
	[CascadingParameter]
    public IDialog DialogRecord { get; set; } = null!;
	
	private MenuRecord _menu;
	
	protected override void OnInitialized()
	{
		InitializeMenu();
		base.OnInitialized();
	}
	
	private void InitializeMenu()
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
        			Dispatcher.Dispatch(new DialogState.DisposeAction(DialogRecord.DynamicViewModelKey));
        			return Task.CompletedTask;
        		}));
        }
		
		if (menuOptionList.Count == 0)
			_menu = MenuRecord.Empty;
		else
			_menu = new MenuRecord(menuOptionList.ToImmutableArray());
	}
}