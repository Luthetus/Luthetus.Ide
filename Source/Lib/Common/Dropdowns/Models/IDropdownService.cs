using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.States;

namespace Luthetus.Common.RazorLib.Dropdowns.Models;

public interface IDropdownService
{
    public IState<DropdownState> DropdownStateWrap { get; }
}