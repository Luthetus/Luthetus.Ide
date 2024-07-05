using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Models;

public class DropdownService : IDropdownService
{
    private readonly IDispatcher _dispatcher;

    public DropdownService(
        IDispatcher dispatcher,
        IState<DropdownState> dropdownStateWrap)
    {
        _dispatcher = dispatcher;
        DropdownStateWrap = dropdownStateWrap;
    }

    public IState<DropdownState> DropdownStateWrap { get; }
}