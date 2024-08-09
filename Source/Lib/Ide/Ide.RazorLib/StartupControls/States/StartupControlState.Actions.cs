using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.States;

public partial record StartupControlState
{
	public record RegisterStartupControlAction(IStartupControlModel StartupControl);
	public record DisposeStartupControlAction(Key<IStartupControlModel> StartupControlKey);
	public record SetActiveStartupControlKeyAction(Key<IStartupControlModel> StartupControlKey);
	
	/// <summary>Trigger a re-render on any Blazor components which subscribe to this state.</summary>
	public record StateChangedAction;
}
