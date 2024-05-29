using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Icons.Models;

namespace Luthetus.Common.RazorLib.Icons.Displays;

public partial class IconBadgeDisplay : ComponentBase
{
	[Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    [Parameter]
    public IconBadgeHorizontalPositionKind IconBadgeHorizontalPositionKind { get; set; } = IconBadgeHorizontalPositionKind.Right;
    [Parameter]
    public IconBadgeVerticalPositionKind IconBadgeVerticalPositionKind { get; set; } = IconBadgeVerticalPositionKind.Bottom;

	[Parameter]
    public string CssClassString { get; set; } = string.Empty;
}

