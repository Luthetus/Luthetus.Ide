using Luthetus.Common.RazorLib.WatchWindows.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

public partial class TreeViewReflectionDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewReflection TreeViewReflection { get; set; } = null!;

    private string GetCssStylingForValue(Type itemType)
    {
        if (itemType == typeof(string))
            return "luth_te_string-literal";
        else if (itemType == typeof(bool))
            return "luth_te_keyword";
        else
            return string.Empty;
    }
}