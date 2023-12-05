using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;

public partial class TreeViewSpinnerDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewSpinner TreeViewSpinner { get; set; } = null!;
}