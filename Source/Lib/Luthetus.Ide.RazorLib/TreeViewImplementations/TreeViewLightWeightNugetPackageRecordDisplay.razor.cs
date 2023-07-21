namespace Luthetus.Ide.RazorLib.TreeViewImplementations;

public partial class TreeViewLightWeightNugetPackageRecordDisplay :
    ComponentBase, ITreeViewLightWeightNugetPackageRecordRendererType
{
    [Parameter, EditorRequired]
    public LightWeightNugetPackageRecord LightWeightNugetPackageRecord { get; set; } = null!;
}