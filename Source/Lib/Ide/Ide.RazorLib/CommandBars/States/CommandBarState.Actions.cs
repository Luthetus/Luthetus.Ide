namespace Luthetus.Ide.RazorLib.CommandBars.States;

public partial record CommandBarState
{
	public record struct SetShouldDisplayAction(bool ShouldDisplay);
}
