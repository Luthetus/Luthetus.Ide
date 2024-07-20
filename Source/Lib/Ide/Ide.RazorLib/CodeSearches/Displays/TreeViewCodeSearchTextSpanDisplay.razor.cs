using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.CodeSearches.Models;

namespace Luthetus.Ide.RazorLib.CodeSearches.Displays;

public partial class TreeViewCodeSearchTextSpanDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewCodeSearchTextSpan TreeViewCodeSearchTextSpan { get; set; } = null!;
}