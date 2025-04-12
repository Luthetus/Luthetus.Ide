using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.FindAllReferences.Models;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Displays;

public partial class TreeViewFindAllReferencesDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewFindAllReferences TreeViewFindAllReferences { get; set; } = null!;
}