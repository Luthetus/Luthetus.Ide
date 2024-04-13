using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;

public partial class TreeViewProjectTestModelDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewProjectTestModel TreeViewProjectTestModel { get; set; } = null!;
}