using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays.Internals;

public partial class TreeViewProjectTestModelDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewProjectTestModel TreeViewProjectTestModel { get; set; } = null!;
}