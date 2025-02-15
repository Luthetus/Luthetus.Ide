using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.Models;

public interface IStartupControlService
{
	public event Action? StartupControlStateChanged;
	
	public StartupControlState GetStartupControlState();

	public void RegisterStartupControl(IStartupControlModel startupControl);
	public void DisposeStartupControl(Key<IStartupControlModel> startupControlKey);
	public void SetActiveStartupControlKey(Key<IStartupControlModel> startupControlKey);
	public void StateChanged();
}
