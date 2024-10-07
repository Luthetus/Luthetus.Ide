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
		
		[ReducerMethod]
		public static IdeHeaderState ReduceModifyMenuFileAction(
			IdeHeaderState inState,
			ModifyMenuFileAction modifyMenuFileAction)
		{
			return inState with
			{
				MenuFile = modifyMenuFileAction.MenuFunc.Invoke(
					inState.MenuFile)
			};
		}
		
		[ReducerMethod]
		public static IdeHeaderState ReduceModifyMenuToolsAction(
			IdeHeaderState inState,
			ModifyMenuToolsAction modifyMenuToolsAction)
		{
			return inState with
			{
				MenuTools = modifyMenuToolsAction.MenuFunc.Invoke(
					inState.MenuTools)
			};
		}

		[ReducerMethod]
		public static IdeHeaderState ReduceModifyMenuViewAction(
			IdeHeaderState inState,
			ModifyMenuViewAction modifyMenuViewAction)
		{
			return inState with
			{
				MenuView = modifyMenuViewAction.MenuFunc.Invoke(
					inState.MenuView)
			};
		}
		
		[ReducerMethod]
		public static IdeHeaderState ReduceModifyMenuRunAction(
			IdeHeaderState inState,
			ModifyMenuRunAction modifyMenuRunAction)
		{
			return inState with
			{
				MenuRun = modifyMenuRunAction.MenuFunc.Invoke(
					inState.MenuRun)
			};
		}
	}
}
