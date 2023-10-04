using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Models;

public interface IDropdownService : ILuthetusCommonService
{
    public IState<DropdownState> DropdownStateWrap { get; }

    public void AddActiveDropdownKey(Key<DropdownRecord> dialogRecord);
    public void RemoveActiveDropdownKey(Key<DropdownRecord> dropdownKey);
    public void ClearActiveDropdownKeysAction();
}