using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;

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
			
			var outActiveStartupControlKey = inState.ActiveStartupControlKey;
			if (inState.ActiveStartupControlKey == disposeStartupControlAction.StartupControlKey)
				outActiveStartupControlKey = Key<IStartupControlModel>.Empty;
						
			return inState with
			{
				StartupControlList = inState.StartupControlList.RemoveAt(indexOfStartupControl),
				ActiveStartupControlKey = outActiveStartupControlKey
			};
		}
		
		[ReducerMethod]
		public static StartupControlState ReduceSetActiveStartupControlKeyAction(
			StartupControlState inState,
			SetActiveStartupControlKeyAction setActiveStartupControlKeyAction)
		{
			var startupControl = inState.StartupControlList.FirstOrDefault(
				x => x.Key == setActiveStartupControlKeyAction.StartupControlKey);
		
			if (setActiveStartupControlKeyAction.StartupControlKey == Key<IStartupControlModel>.Empty ||
			    startupControl is null)
			{
				return inState with
				{
					ActiveStartupControlKey = Key<IStartupControlModel>.Empty
				};
			}
		
			return inState with
			{
				ActiveStartupControlKey = startupControl.Key
			};
		}
	}
}
