using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;

namespace Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

public partial class TreeViewSpinnerDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewSpinner TreeViewSpinner { get; set; } = null!;
}