using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;

public partial class TreeViewStringFragmentDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewStringFragment TreeViewStringFragment { get; set; } = null!;
}