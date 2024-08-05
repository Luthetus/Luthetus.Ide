using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;

namespace Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

public partial class TreeViewGroupDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewGroup TreeViewGroup { get; set; } = null!;
}