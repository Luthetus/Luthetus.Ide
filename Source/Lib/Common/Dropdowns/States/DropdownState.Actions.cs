using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.States;

public partial record DropdownState
{
	public record RegisterAction(DropdownRecord Dropdown);
	public record DisposeAction(Key<DropdownRecord> Key);
	public record ClearAction;
	public record FitOnScreenAction(DropdownRecord Dropdown);
    public record AddActiveAction(Key<DropdownRecord> Key);
    public record RemoveActiveAction(Key<DropdownRecord> Key);
    public record ClearActivesAction;
}