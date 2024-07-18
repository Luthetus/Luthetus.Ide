using Fluxor;

namespace Luthetus.Ide.RazorLib.StartupControls.States;

public partial record StartupControlState
{
	public class Reducer
	{
		[ReducerMethod]
		public static StartupControlState ReduceRegisterStartupControlAction(
			StartupControlState inState,
			RegisterStartupControlAction registerStartupControlAction)
		{
			var indexOfStartupControl = inState.StartupControlList.FindIndex(
				x => x.Key == registerStartupControlAction.StartupControl.Key);
				
			if (indexOfStartupControl != -1)
				return inState;
		
			return inState with
			{
				StartupControlList = inState.StartupControlList.Add(
					registerStartupControlAction.StartupControl)
			};
		}
		
		[ReducerMethod]
		public static StartupControlState ReduceDisposeStartupControlAction(
			StartupControlState inState,
			DisposeStartupControlAction disposeStartupControlAction)
		{
			var indexOfStartupControl = inState.StartupControlList.FindIndex(
				x => x.Key == disposeStartupControlAction.StartupControlKey);
				
			if (indexOfStartupControl == -1)
				return inState;
			
			var outActiveStartupControl = inState.ActiveStartupControl;
			if (inState.ActiveStartupControl.Key == disposeStartupControlAction.StartupControlKey)
				outActiveStartupControl = null;
						
			return inState with
			{
				StartupControlList = inState.StartupControlList.RemoveAt(indexOfStartupControl),
				ActiveStartupControl = outActiveStartupControl
			};
		}
		
		[ReducerMethod]
		public static StartupControlState ReduceSetActiveStartupControlAction(
			StartupControlState inState,
			SetActiveStartupControlAction setActiveStartupControlAction)
		{
			if (setActiveStartupControlAction.StartupControlKey is null)
			{
				return inState with
				{
					ActiveStartupControl = null
				};
			}
		
			var startupControl = inState.StartupControlList.FirstOrDefault(
				x => x.Key == setActiveStartupControlAction.StartupControlKey);
				
			if (startupControl is null)
				return inState;
		
			return inState with
			{
				ActiveStartupControl = startupControl
			};
		}
	}
}
