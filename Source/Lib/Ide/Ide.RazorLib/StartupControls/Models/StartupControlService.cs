using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.Models;

public class StartupControlService : IStartupControlService
{
	private StartupControlState _startupControlState = new();
	
	public event Action? StartupControlStateChanged;
	
	public StartupControlState GetStartupControlState() => _startupControlState;

	public void ReduceRegisterStartupControlAction(IStartupControlModel startupControl)
	{
		var inState = GetStartupControlState();
	
		var indexOfStartupControl = inState.StartupControlList.FindIndex(
			x => x.Key == startupControl.Key);
			
		if (indexOfStartupControl != -1)
		{
			StartupControlStateChanged?.Invoke();
			return;
		}
	
		_startupControlState = inState with
		{
			StartupControlList = inState.StartupControlList.Add(startupControl)
		};
		
		StartupControlStateChanged?.Invoke();
		return;
	}
	
	public void ReduceDisposeStartupControlAction(Key<IStartupControlModel> startupControlKey)
	{
		var inState = GetStartupControlState();
	
		var indexOfStartupControl = inState.StartupControlList.FindIndex(
			x => x.Key == startupControlKey);
			
		if (indexOfStartupControl == -1)
		{
			StartupControlStateChanged?.Invoke();
			return;
		}
		
		var outActiveStartupControlKey = inState.ActiveStartupControlKey;
		if (inState.ActiveStartupControlKey == startupControlKey)
			outActiveStartupControlKey = Key<IStartupControlModel>.Empty;
					
		_startupControlState = inState with
		{
			StartupControlList = inState.StartupControlList.RemoveAt(indexOfStartupControl),
			ActiveStartupControlKey = outActiveStartupControlKey
		};
		
		StartupControlStateChanged?.Invoke();
		return;
	}
	
	public void ReduceSetActiveStartupControlKeyAction(Key<IStartupControlModel> startupControlKey)
	{
		var inState = GetStartupControlState();
	
		var startupControl = inState.StartupControlList.FirstOrDefault(
			x => x.Key == startupControlKey);
	
		if (startupControlKey == Key<IStartupControlModel>.Empty ||
		    startupControl is null)
		{
			_startupControlState = inState with
			{
				ActiveStartupControlKey = Key<IStartupControlModel>.Empty
			};
			
			StartupControlStateChanged?.Invoke();
			return;
		}
	
		_startupControlState = inState with
		{
			ActiveStartupControlKey = startupControl.Key
		};
		
		StartupControlStateChanged?.Invoke();
		return;
	}
	
	public void ReduceStateChangedAction()
	{
		StartupControlStateChanged?.Invoke();
		return;
	}
}
