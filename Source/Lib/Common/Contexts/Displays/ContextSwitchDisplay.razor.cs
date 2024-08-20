using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.Widgets.States;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextSwitchDisplay : ComponentBase
{
	[Inject]
	private IState<ContextSwitchState> ContextSwitchStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	
	[CascadingParameter]
    public WidgetModel Widget { get; set; } = null!;
    
    private readonly List<(string ContextSwitchGroupTitle, MenuRecord Menu)> _groupMenuTupleList = new();
    private bool hasCalculatedGroupMenuTupleList = false;
	
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			var contextSwitchState = ContextSwitchStateWrap.Value;
			
			foreach (var contextSwitchGroup in contextSwitchState.ContextSwitchGroupList)
			{
				_groupMenuTupleList.Add(
					(contextSwitchGroup.Title, await contextSwitchGroup.GetMenuFunc()));
			}
			
			hasCalculatedGroupMenuTupleList = true;
		}

		base.OnAfterRenderAsync(firstRender);
	}
}