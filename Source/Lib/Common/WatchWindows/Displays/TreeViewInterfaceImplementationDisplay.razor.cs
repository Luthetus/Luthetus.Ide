using Luthetus.Common.RazorLib.WatchWindows.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

public partial class TreeViewInterfaceImplementationDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewInterfaceImplementation TreeViewInterfaceImplementation { get; set; } = null!;
}