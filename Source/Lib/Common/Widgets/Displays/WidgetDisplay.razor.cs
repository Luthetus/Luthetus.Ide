using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Widgets.Models;

namespace Luthetus.Common.RazorLib.Widgets.Displays;

public partial class WidgetDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public WidgetModel Widget { get; set; } = null!;
	[Parameter, EditorRequired]
    public Func<WidgetModel, Task> OnFocusInFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<WidgetModel, Task> OnFocusOutFunc { get; set; } = null!;
}