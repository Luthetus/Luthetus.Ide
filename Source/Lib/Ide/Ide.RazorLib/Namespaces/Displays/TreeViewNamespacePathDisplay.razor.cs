using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.Namespaces.Displays;

public partial class TreeViewNamespacePathDisplay : ComponentBase, ITreeViewNamespacePathRendererType
{
	[Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;

    [CascadingParameter(Name="LuthetusCommonIconWidthOverride")]
    public int? LuthetusCommonIconWidthOverride { get; set; }
    [CascadingParameter(Name="LuthetusCommonIconHeightOverride")]
    public int? LuthetusCommonIconHeightOverride { get; set; }

	[Parameter, EditorRequired]
    public NamespacePath NamespacePath { get; set; }
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    
    public int WidthInPixels => LuthetusCommonIconWidthOverride ??
        AppOptionsStateWrap.Value.Options.IconSizeInPixels;

    public int HeightInPixels => LuthetusCommonIconHeightOverride ??
        AppOptionsStateWrap.Value.Options.IconSizeInPixels;
}