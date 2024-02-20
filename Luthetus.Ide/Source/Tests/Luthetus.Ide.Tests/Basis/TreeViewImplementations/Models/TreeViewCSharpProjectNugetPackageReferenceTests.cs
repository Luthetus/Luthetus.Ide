using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Nugets.Models;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

public class TreeViewCSharpProjectNugetPackageReferenceTests
{
    public TreeViewCSharpProjectNugetPackageReference(
            CSharpProjectNugetPackageReference cSharpProjectNugetPackageReference,
            ILuthetusIdeComponentRenderers ideComponentRenderers,
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            bool isExpandable,
            bool isExpanded)
        : base(cSharpProjectNugetPackageReference, isExpandable, isExpanded)
    {
        IdeComponentRenderers = ideComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }

    public ILuthetusIdeComponentRenderers IdeComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewCSharpProjectNugetPackageReference otherTreeView)
            return false;

        return otherTreeView.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode()
    {
        var uniqueString = Item.CSharpProjectAbsolutePathString + Item.LightWeightNugetPackageRecord.Id;
        return uniqueString.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            IdeComponentRenderers.LuthetusIdeTreeViews.TreeViewLightWeightNugetPackageRecordRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewCSharpProjectNugetPackageReferenceRendererType.CSharpProjectNugetPackageReference),
                    Item
                },
            });
    }

    public override Task LoadChildListAsync()
    {
        TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}