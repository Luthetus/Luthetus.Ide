using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Displays;

public partial class TreeViewFindAllGroupDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewFindAllGroup TreeViewFindAllGroup { get; set; } = null!;
}