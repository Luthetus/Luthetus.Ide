using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.Namespaces.Displays;

public partial class TreeViewNamespacePathDisplay : ComponentBase, ITreeViewNamespacePathRendererType
{
	[Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    [CascadingParameter(Name="LuthetusCommonIconWidthOverride")]
    public int? LuthetusCommonIconWidthOverride { get; set; }
    [CascadingParameter(Name="LuthetusCommonIconHeightOverride")]
    public int? LuthetusCommonIconHeightOverride { get; set; }

	[Parameter, EditorRequired]
    public NamespacePath NamespacePath { get; set; }
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    
    public int WidthInPixels => LuthetusCommonIconWidthOverride ??
		CommonApi.AppOptionApi.GetAppOptionsState().Options.IconSizeInPixels;

    public int HeightInPixels => LuthetusCommonIconHeightOverride ??
		CommonApi.AppOptionApi.GetAppOptionsState().Options.IconSizeInPixels;
}