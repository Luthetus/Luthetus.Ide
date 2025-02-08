using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;

namespace Luthetus.Common.RazorLib.Menus.Displays;

public partial class MenuDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter(Name="ReturnFocusToParentFuncAsync")]
    public Func<Task>? ReturnFocusToParentFuncAsync { get; set; }
	[CascadingParameter]
    public DropdownRecord? Dropdown { get; set; }

    [Parameter, EditorRequired]
    public MenuRecord MenuRecord { get; set; } = null!;

    [Parameter]
    public int InitialActiveMenuOptionRecordIndex { get; set; } = -1;
    [Parameter]
    public bool GroupByMenuOptionKind { get; set; } = true;
    [Parameter]
    public bool FocusOnAfterRenderAsync { get; set; } = true;
    [Parameter]
    public RenderFragment<MenuOptionRecord>? IconRenderFragment { get; set; }

    private ElementReference? _menuDisplayElementReference;

    /// <summary>
    /// First time the MenuDisplay opens the _activeMenuOptionRecordIndex == -1
    /// </summary>
    private int _activeMenuOptionRecordIndex = -1;

	/// <summary>
	/// The <see cref="MenuRecord"/>, which is provided to this component, is most often asynchronously calculated.
	///
	/// i.e.: The initial <see cref="MenuRecord"/> parameter is <see cref="MenuRecord.Empty"/>,
	///       then the actual is passed in afterwards when the async task completes.
	/// </summary>
	private MenuRecord? _previousMenuRecord;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _activeMenuOptionRecordIndex = InitialActiveMenuOptionRecordIndex;

            if (FocusOnAfterRenderAsync &&
                _activeMenuOptionRecordIndex == -1 &&
                _menuDisplayElementReference is not null)
            {
                try
                {
                    await _menuDisplayElementReference.Value
                        .FocusAsync()
                        .ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                    //             This bug is seemingly happening randomly. I have a suspicion
                    //             that there are race-condition exceptions occurring with "FocusAsync"
                    //             on an ElementReference.
                }
            }
            else
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task SetFocusToFirstOptionInMenuAsync()
    {
        _activeMenuOptionRecordIndex = 0;

        await InvokeAsync(StateHasChanged);
    }

    private async Task RestoreFocusToThisMenuAsync()
    {
        if (_activeMenuOptionRecordIndex == -1)
        {
            try
            {
				var localMenuDisplayElementReference = _menuDisplayElementReference;
                if (localMenuDisplayElementReference is not null)
                {
                    await localMenuDisplayElementReference.Value
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

            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (MenuRecord.MenuOptionList.Count == 0)
        {
            _activeMenuOptionRecordIndex = -1;
            return;
        }

        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT:
            	if (Dropdown is not null)
	                Dispatcher.Dispatch(new DropdownState.DisposeAction(Dropdown.Key));

                if (ReturnFocusToParentFuncAsync is not null)
                    await ReturnFocusToParentFuncAsync.Invoke().ConfigureAwait(false);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN:
                if (_activeMenuOptionRecordIndex >= MenuRecord.MenuOptionList.Count - 1)
                    _activeMenuOptionRecordIndex = 0;
                else
                    _activeMenuOptionRecordIndex++;
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP:
                if (_activeMenuOptionRecordIndex <= 0)
                    _activeMenuOptionRecordIndex = MenuRecord.MenuOptionList.Count - 1;
                else
                    _activeMenuOptionRecordIndex--;
                break;
            case KeyboardKeyFacts.MovementKeys.HOME:
                _activeMenuOptionRecordIndex = 0;
                break;
            case KeyboardKeyFacts.MovementKeys.END:
                _activeMenuOptionRecordIndex = MenuRecord.MenuOptionList.Count - 1;
                break;
            case KeyboardKeyFacts.MetaKeys.ESCAPE:
            	if (Dropdown is not null)
	                Dispatcher.Dispatch(new DropdownState.DisposeAction(Dropdown.Key));

                if (ReturnFocusToParentFuncAsync is not null)
                    await ReturnFocusToParentFuncAsync.Invoke().ConfigureAwait(false);

                break;
        }
    }
}