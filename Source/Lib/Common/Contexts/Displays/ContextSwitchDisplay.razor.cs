using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.Contexts.States;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.Widgets.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextSwitchDisplay : ComponentBase
{
	[Inject]
	private IState<ContextSwitchState> ContextSwitchStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	
	[CascadingParameter]
    public WidgetModel Widget { get; set; } = null!;
    
    private readonly List<ContextSwitchGroupMarker> _groupMenuTupleList = new();
    private readonly List<MenuOptionRecord> _flatMenuOptionList = new();
    
    private bool _hasCalculatedGroupMenuTupleList = false;
    private int _activeIndex = 0;
    private ElementReference? _contextSwitchHtmlElement;
    
    /// <summary>
    /// Track where within the <see cref="_flatMenuOptionList"/> do various groups start.
    /// </summary>
    private class ContextSwitchGroupMarker
    {
		public ContextSwitchGroupMarker(string title, MenuRecord menu, int startInclusiveIndex, int menuOptionListLength)
		{
			Title = title;
			Menu = menu;
			StartInclusiveIndex = startInclusiveIndex;
			MenuOptionListLength = menuOptionListLength;
		}

    	public string Title { get; }
    	public MenuRecord Menu { get; }
    	public int StartInclusiveIndex { get; }
    	public int MenuOptionListLength { get; }
    }
	
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			var contextSwitchState = ContextSwitchStateWrap.Value;
			
			foreach (var contextSwitchGroup in contextSwitchState.ContextSwitchGroupList)
			{
				var menu = await contextSwitchGroup.GetMenuFunc();
				
				_groupMenuTupleList.Add(new(
					contextSwitchGroup.Title,
					menu,
					_flatMenuOptionList.Count,
					menu.MenuOptionList.Length));
					
				_flatMenuOptionList.AddRange(menu.MenuOptionList);
			}
			
			_hasCalculatedGroupMenuTupleList = true;
			await InvokeAsync(StateHasChanged);
			
			try
            {
                await _contextSwitchHtmlElement.Value
                    .FocusAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
				// Eat this exception
            }
		}

		base.OnAfterRenderAsync(firstRender);
	}
	
	private int GetActiveGroupIndex()
	{
		var localActiveIndex = _activeIndex;
	
		if (localActiveIndex < 0 || !_hasCalculatedGroupMenuTupleList)
			return -1;
	
		var activeGroupMarker = (ContextSwitchGroupMarker?)null;
		
		int index = 0;
		for (; index < _groupMenuTupleList.Count; index++)
		{
			var groupMarker = _groupMenuTupleList[index];
			if (groupMarker.StartInclusiveIndex + groupMarker.MenuOptionListLength > _activeIndex)
			{
				activeGroupMarker = groupMarker;
				break;
			}
		}
		
		if (activeGroupMarker is null)
			return -1;
			
		return index;
	}
	
	private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (_flatMenuOptionList.Count == 0)
        {
            _activeIndex = -1;
            return;
        }
        
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT:
            	// TODO: Determine current column, ...
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN:
                if (_activeIndex >= _flatMenuOptionList.Count - 1)
                    _activeIndex = 0;
                else
                    _activeIndex++;
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP:
                if (_activeIndex <= 0)
                    _activeIndex = _flatMenuOptionList.Count - 1;
                else
                    _activeIndex--;
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT:
            	// TODO: Determine current column, ...
                break;
            case KeyboardKeyFacts.MovementKeys.HOME:
                _activeIndex = 0;
                break;
            case KeyboardKeyFacts.MovementKeys.END:
                _activeIndex = _flatMenuOptionList.Count - 1;
                break;
            case KeyboardKeyFacts.MetaKeys.ESCAPE:
                break;
        }
    }
}