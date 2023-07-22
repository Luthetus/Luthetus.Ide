using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.Nuget;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewLightWeightNugetPackageRecordDisplay :
    ComponentBase, ITreeViewLightWeightNugetPackageRecordRendererType
{
    [Parameter, EditorRequired]
    public LightWeightNugetPackageRecord LightWeightNugetPackageRecord { get; set; } = null!;
}