using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dropdowns.States;

namespace Luthetus.Common.RazorLib.Dropdowns.Displays;

public partial class DropdownInitializer : FluxorComponent
{
	[Inject]
    private IState<DropdownState> DropdownStateWrap { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    /// <summary>
    /// The unused parameter "mouseEventArgs" is here because
    /// <see cref="OutOfBoundsClicks.Displays.OutOfBoundsClickDisplay"/>
    /// requires an <see cref="Action{MouseEventArgs}"/>
    /// </summary>
    private void ClearActiveKeyList(MouseEventArgs mouseEventArgs)
    {
        Dispatcher.Dispatch(new DropdownState.ClearAction());
    }
}