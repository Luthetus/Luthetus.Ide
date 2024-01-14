using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Displays;

public partial class DropdownDisplay : FluxorComponent
{
    [Inject]
    private IState<DropdownState> DropdownStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<DropdownRecord> DropdownKey { get; set; } = Key<DropdownRecord>.Empty;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    [Parameter]
    public DropdownPositionKind DropdownPositionKind { get; set; } = DropdownPositionKind.Vertical;
    [Parameter]
    public bool ShouldDisplayOutOfBoundsClickDisplay { get; set; } = true;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    private bool _disposed;

    private bool ShouldDisplay => DropdownStateWrap.Value.ActiveKeyList.Contains(DropdownKey);

    private string DropdownPositionKindStyleCss => DropdownPositionKind switch
    {
        DropdownPositionKind.Vertical => "position: absolute; left: 0; top: 100%;",
        DropdownPositionKind.Horizontal => "position: absolute; left: 100%; top: 0;",
        DropdownPositionKind.Unset => string.Empty,
        _ => throw new ApplicationException($"The {nameof(DropdownPositionKind)}: {DropdownPositionKind} was unrecognized.")
    };

    /// <summary>
    /// The unused parameter "mouseEventArgs" is here because
    /// <see cref="OutOfBoundsClicks.Displays.OutOfBoundsClickDisplay"/>
    /// requires an <see cref="Action{MouseEventArgs}"/>
    /// </summary>
    private void ClearActiveKeyList(MouseEventArgs mouseEventArgs)
    {
        Dispatcher.Dispatch(new DropdownState.ClearActivesAction());
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _disposed = true;

            if (ShouldDisplay)
                Dispatcher.Dispatch(new DropdownState.RemoveActiveAction(DropdownKey));
        }

        base.Dispose(disposing);
    }
}