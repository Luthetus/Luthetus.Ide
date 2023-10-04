using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.States;

public partial record DropdownState
{
    public record AddActiveAction(Key<DropdownRecord> Key);
    public record RemoveActiveAction(Key<DropdownRecord> Key);
    public record ClearActivesAction;
}