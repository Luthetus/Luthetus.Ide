using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.Models;

public interface IStartupControlService
{
	public event Action? StartupControlStateChanged;
	
	public StartupControlState GetStartupControlState();

	public void ReduceRegisterStartupControlAction(IStartupControlModel startupControl);
	public void ReduceDisposeStartupControlAction(Key<IStartupControlModel> startupControlKey);
	public void ReduceSetActiveStartupControlKeyAction(Key<IStartupControlModel> startupControlKey);
	public void ReduceStateChangedAction();
}
