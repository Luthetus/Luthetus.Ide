using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

public partial class TreeViewMissingRendererFallbackDisplay : ComponentBase,
    ITreeViewMissingRendererFallbackType
{
    [Parameter, EditorRequired]
    public string DisplayText { get; set; } = string.Empty;
}