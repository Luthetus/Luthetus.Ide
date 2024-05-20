using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.TreeViewUtils.Models;

namespace Luthetus.Ide.RazorLib.TreeViewUtils.Displays;

public partial class TreeViewSpinnerDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewSpinner TreeViewSpinner { get; set; } = null!;
}