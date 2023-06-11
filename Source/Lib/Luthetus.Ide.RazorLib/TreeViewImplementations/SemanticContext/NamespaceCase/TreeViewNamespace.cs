using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewClasses;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.BoundClassDefinitionNodeCase;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.NamespaceCase;

public class TreeViewNamespace : TreeViewWithType<BoundNamespaceStatementNode>
{
    public TreeViewNamespace(
        BoundNamespaceStatementNode boundNamespaceStatementNode,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                boundNamespaceStatementNode,
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
            typeof(TreeViewNamespaceDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewNamespaceDisplay.BoundNamespaceStatementNode),
                    Item
                },
            });
    }

    public override async Task LoadChildrenAsync()
    {
        if (Item is null)
            return;

        try
        {
            var newChildren = Item.Children
                .SelectMany(x => ((BoundNamespaceEntryNode)x).CompilationUnit.Children)
                .Where(x => x.SyntaxKind == SyntaxKind.BoundClassDefinitionNode)
                .Select(x => (TreeViewNoType) new TreeViewBoundClassDefinitionNode(
                    (BoundClassDefinitionNode)x,
                    LuthetusIdeComponentRenderers,
                    FileSystemProvider,
                    EnvironmentProvider,
                    true,
                    false))
                    .ToList();

            var oldChildrenMap = Children
                .ToDictionary(child => child);

            foreach (var newChild in newChildren)
            {
                if (oldChildrenMap.TryGetValue(newChild, out var oldChild))
                {
                    newChild.IsExpanded = oldChild.IsExpanded;
                    newChild.IsExpandable = oldChild.IsExpandable;
                    newChild.IsHidden = oldChild.IsHidden;
                    newChild.TreeViewNodeKey = oldChild.TreeViewNodeKey;
                    newChild.Children = oldChild.Children;
                }
            }

            for (int i = 0; i < newChildren.Count; i++)
            {
                var newChild = newChildren[i];

                newChild.IndexAmongSiblings = i;
                newChild.Parent = this;
                newChild.TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
            }

            Children = newChildren;
        }
        catch (Exception exception)
        {
            Children = new List<TreeViewNoType>
            {
                new TreeViewException(
                    exception,
                    false,
                    false,
                    LuthetusIdeComponentRenderers.LuthetusCommonComponentRenderers.WatchWindowTreeViewRenderers)
                {
                    Parent = this,
                    IndexAmongSiblings = 0,
                }
            };
        }

        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}
