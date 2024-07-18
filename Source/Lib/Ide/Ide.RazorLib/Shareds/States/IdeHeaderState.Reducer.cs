using Fluxor;

namespace Luthetus.Ide.RazorLib.Shareds.States;

public partial record IdeHeaderState
{
	public class Reducer
	{
		[ReducerMethod]
		public static IdeHeaderState ReduceSetMenuFileAction(
			IdeHeaderState inState,
			SetMenuFileAction setMenuFileAction)
		{
			return inState with
			{
				MenuFile = setMenuFileAction.Menu
			};
		}
		
		[ReducerMethod]
		public static IdeHeaderState ReduceSetMenuToolsAction(
			IdeHeaderState inState,
			SetMenuToolsAction setMenuToolsAction)
		{
			return inState with
			{
				MenuTools = setMenuToolsAction.Menu
			};
		}
		
		[ReducerMethod]
		public static IdeHeaderState ReduceSetMenuViewAction(
			IdeHeaderState inState,
			SetMenuViewAction setMenuViewAction)
		{
			return inState with
			{
				MenuView = setMenuViewAction.Menu
			};
		}
		
		[ReducerMethod]
		public static IdeHeaderState ReduceSetMenuRunAction(
			IdeHeaderState inState,
			SetMenuRunAction setMenuRunAction)
		{
			return inState with
			{
				MenuRun = setMenuRunAction.Menu
			};
		}
	}
}
