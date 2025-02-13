using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

public partial class TreeViewSpinnerDisplay : ComponentBase
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    
	[Parameter, EditorRequired]
	public TreeViewSpinner TreeViewSpinner { get; set; } = null!;
}