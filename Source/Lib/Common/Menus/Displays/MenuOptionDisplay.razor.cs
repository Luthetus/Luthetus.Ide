using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.Menus.Displays;

public partial class MenuOptionDisplay : ComponentBase
{
    [Inject]
    private IState<DropdownState> DropdownStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

	[CascadingParameter]
	public DropdownRecord? Dropdown { get; set; }

    [Parameter, EditorRequired]
    public MenuOptionRecord MenuOptionRecord { get; set; } = null!;
    [Parameter, EditorRequired]
    public int Index { get; set; }
    [Parameter, EditorRequired]
    public int ActiveMenuOptionRecordIndex { get; set; }

    [Parameter]
    public RenderFragment<MenuOptionRecord>? IconRenderFragment { get; set; }

	private string _menuOptionHtmlElementId => $"luth_menu-option-display_{_htmlElementIdSalt}";

    private readonly Key<DropdownRecord> _subMenuDropdownKey = Key<DropdownRecord>.NewKey();
    private readonly Guid _htmlElementIdSalt = Guid.NewGuid();

    private ElementReference? _topmostElementReference;
    private bool _shouldDisplayWidget;

    private bool IsActive => Index == ActiveMenuOptionRecordIndex;

    private bool HasSubmenuActive => DropdownStateWrap.Value.DropdownList.Any(
        x => x.Key == _subMenuDropdownKey);

    private string IsActiveCssClass => IsActive ? "luth_active" : string.Empty;

    private string HasWidgetActiveCssClass =>
        _shouldDisplayWidget && MenuOptionRecord.WidgetRendererType is not null
            ? "luth_active"
            : string.Empty;

    private MenuOptionCallbacks MenuOptionCallbacks => new(
        () => HideWidgetAsync(null),
        HideWidgetAsync);

    private bool DisplayWidget => _shouldDisplayWidget && MenuOptionRecord.WidgetRendererType is not null;

    protected override async Task OnParametersSetAsync()
    {
        var localHasSubmenuActive = HasSubmenuActive;

        // The following if(s) are not working. They result
        // in one never being able to open a submenu
        //
        // // Close submenu for non active menu option
        // if (!IsActive && localHasSubmenuActive)
        //     Dispatcher.Dispatch(new RemoveActiveDropdownKeyAction(_subMenuDropdownKey));
        //
        // // Hide widget for non active menu option
        // if (!IsActive && _shouldDisplayWidget)
        // {
        //     _shouldDisplayWidget = false;
        // }

        // Set focus to active menu option
        if (IsActive && !localHasSubmenuActive && !DisplayWidget)
        {
            FocusElementReference();
        }

        await base.OnParametersSetAsync();
    }

    private void HandleOnClick()
    {
        if (MenuOptionRecord.OnClickFunc is not null)
        {
            MenuOptionRecord.OnClickFunc.Invoke();
            Dispatcher.Dispatch(new DropdownState.ClearAction());

			var localDropdown = Dropdown;
			if (localDropdown is not null)
            	Dispatcher.Dispatch(new DropdownState.DisposeAction(localDropdown.Key));
        }

		var localSubMenu = MenuOptionRecord.SubMenu;
        if (localSubMenu is not null)
        {
            if (HasSubmenuActive)
                Dispatcher.Dispatch(new DropdownState.DisposeAction(_subMenuDropdownKey));
            else
				RenderDropdownOnClick(localSubMenu);
        }

        if (MenuOptionRecord.WidgetRendererType is not null)
        {
            _shouldDisplayWidget = !_shouldDisplayWidget;
        }
    }

	private async Task RenderDropdownOnClick(MenuRecord localSubMenu)
	{
		var jsRuntimeCommonApi = JsRuntime.GetLuthetusCommonApi();

		var buttonDimensions = await jsRuntimeCommonApi
			.MeasureElementById(_menuOptionHtmlElementId)
			.ConfigureAwait(false);

		var dropdownRecord = new DropdownRecord(
			_subMenuDropdownKey,
			buttonDimensions.LeftInPixels + buttonDimensions.WidthInPixels,
			buttonDimensions.TopInPixels,
			typeof(MenuDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(MenuDisplay.MenuRecord),
					localSubMenu
				}
			},
			FocusElementReference);

        Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
	}

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT:
				var localSubMenu = MenuOptionRecord.SubMenu;
                if (localSubMenu is not null)
                    RenderDropdownOnClick(localSubMenu);
                break;
        }

        switch (keyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                HandleOnClick();
                break;
        }
    }

	private async Task FocusElementReference()
	{
		var localTopmostElementReference = _topmostElementReference;
	    try
        {
            if (localTopmostElementReference.HasValue)
            {
                await localTopmostElementReference.Value
                    .FocusAsync()
                    .ConfigureAwait(false);
            }
        }
        catch (Exception)
        {
            // TODO: Capture specifically the exception that is fired when the JsRuntime...
			//       ...tries to set focus to an HTML element, but that HTML element
			//       was not found.
        }
	}

    private async Task HideWidgetAsync(Action? onAfterWidgetHidden)
    {
        _shouldDisplayWidget = false;
        await InvokeAsync(StateHasChanged);

        if (onAfterWidgetHidden is null) // Only hide the widget
        {
			await FocusElementReference();
        }
        else // Hide the widget AND dispose the menu
        {
            onAfterWidgetHidden.Invoke();
            Dispatcher.Dispatch(new DropdownState.ClearAction());

			var localDropdown = Dropdown;
			if (localDropdown is not null)
            	Dispatcher.Dispatch(new DropdownState.DisposeAction(localDropdown.Key));
        }
    }
}