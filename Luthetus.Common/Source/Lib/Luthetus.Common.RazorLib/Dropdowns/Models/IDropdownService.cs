using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Models;

public interface IDropdownService
{
    public IState<DropdownState> DropdownStateWrap { get; }

    public void AddActiveDropdownKey(Key<DropdownRecord> dialogRecord);
    public void RemoveActiveDropdownKey(Key<DropdownRecord> dropdownKey);
    public void ClearActiveDropdownKeysAction();
}