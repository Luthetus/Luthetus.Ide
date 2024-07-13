using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays.Internals;

public partial class TreeViewGroupDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewGroup TreeViewGroup { get; set; } = null!;
}