using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.Nuget;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations;

public class TreeViewCSharpProjectNugetPackageReference : TreeViewWithType<CSharpProjectNugetPackageReference>
{
    public TreeViewCSharpProjectNugetPackageReference(
        CSharpProjectNugetPackageReference cSharpProjectNugetPackageReference,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                cSharpProjectNugetPackageReference,
                isExpandable,
                isExpanded)
    {
        LuthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }

    public ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewCSharpProjectNugetPackageReference otherTreeView)
        {
            return false;
        }

        return otherTreeView.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode()
    {
        var uniqueString = Item.CSharpProjectAbsoluteFilePathString +
            Item.LightWeightNugetPackageRecord.Id;

        return uniqueString.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            LuthetusIdeComponentRenderers.TreeViewLightWeightNugetPackageRecordRendererType!,
            new Dictionary<string, object?>
            {
            {
                nameof(ITreeViewCSharpProjectNugetPackageReferenceRendererType.CSharpProjectNugetPackageReference),
                Item
            },
            });
    }

    public override Task LoadChildrenAsync()
    {
        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}