using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Models;

public class DropdownService : IDropdownService
{
    private readonly IDispatcher _dispatcher;

    public DropdownService(
        bool isEnabled,
        IDispatcher dispatcher,
        IState<DropdownState> dropdownStateWrap)
    {
        _dispatcher = dispatcher;
        IsEnabled = isEnabled;
        DropdownStateWrap = dropdownStateWrap;
    }

    public bool IsEnabled { get; }
    public IState<DropdownState> DropdownStateWrap { get; }

    public void AddActiveDropdownKey(Key<DropdownRecord> dialogRecord)
    {
        _dispatcher.Dispatch(new DropdownState.AddActiveAction(
            dialogRecord));
    }

    public void RemoveActiveDropdownKey(Key<DropdownRecord> dropdownKey)
    {
        _dispatcher.Dispatch(new DropdownState.RemoveActiveAction(
            dropdownKey));
    }

    public void ClearActiveDropdownKeysAction()
    {
        _dispatcher.Dispatch(new DropdownState.ClearActivesAction());
    }
}