using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
using Luthetus.Common.RazorLib.Options.States;

namespace Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

public partial class TreeViewSpinnerDisplay : ComponentBase
{
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    
	[Parameter, EditorRequired]
	public TreeViewSpinner TreeViewSpinner { get; set; } = null!;
}