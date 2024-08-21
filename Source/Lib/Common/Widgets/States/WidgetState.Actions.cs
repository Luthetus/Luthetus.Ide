using Luthetus.Common.RazorLib.Widgets.Models;

namespace Luthetus.Common.RazorLib.Widgets.States;

public partial record WidgetState
{
	public record SetWidgetAction(WidgetModel? Widget);
}
