using Luthetus.Common.RazorLib.Widgets.Models;

namespace Luthetus.Common.RazorLib.Widgets.Models;

/// <summary>
/// This UI is similar, but not equivalent, to <see cref="DialogState"/>.<br/>
///
/// This UI:<br/>
/// - Only one can be rendered at any given time<br/>
/// - The <see cref="Luthetus.Common.RazorLib.OutOfBoundsClicks.Displays.OutOfBoundsClickDisplay"/>
///       will be rendered, so if the user clicks off, the widget will stop being rendered.<br/>
/// - If the user onfocusout events from the widget, the widget will stop being rendered.<br/>
/// </summary>
public record struct WidgetState(WidgetModel? Widget)
{
	public WidgetState() : this((WidgetModel?)null)
    {
        
    }
}
