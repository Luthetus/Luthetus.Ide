using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Displays;

public partial class TreeViewFindAllTextSpanDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewFindAllTextSpan TreeViewFindAllTextSpan { get; set; } = null!;
}