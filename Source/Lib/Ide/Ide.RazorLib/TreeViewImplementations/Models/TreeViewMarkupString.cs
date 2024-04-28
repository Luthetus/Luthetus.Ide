using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public class TreeViewMarkupString : TreeViewWithType<MarkupString>
{
    public TreeViewMarkupString(
            MarkupString markupString,
            ILuthetusIdeComponentRenderers ideComponentRenderers,
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            bool isExpandable,
            bool isExpanded)
        : base(
            markupString,
            isExpandable,
            isExpanded)
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
        // TODO: Equals
        return false;
    }

    public override int GetHashCode()
    {
        // TODO: GetHashCode
        return Path.GetRandomFileName().GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            typeof(TreeViewMarkupStringDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewMarkupStringDisplay.MarkupString),
                    Item
                },
            });
    }

    public override Task LoadChildListAsync()
    {
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}