using Fluxor;

namespace Luthetus.Ide.RazorLib.CommandBars.States;

public partial record CommandBarState
{
	public class Reducer
	{
		[ReducerMethod]
		public static CommandBarState ReduceSetShouldDisplayAction(
			CommandBarState inState,
			SetShouldDisplayAction setShouldDisplayAction)
		{
			return inState with
			{
				ShouldDisplay = setShouldDisplayAction.ShouldDisplay
			};
		}
	}
}
