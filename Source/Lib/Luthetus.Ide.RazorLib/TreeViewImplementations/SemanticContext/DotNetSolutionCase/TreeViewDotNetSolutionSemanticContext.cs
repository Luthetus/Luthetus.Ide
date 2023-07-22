using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewClasses;
using Luthetus.CompilerServices.Lang.CSharp.SemanticContextCase.Implementations;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Store.SemanticContextCase;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.DotNetSolutionCase;

public class TreeViewDotNetSolutionSemanticContext : TreeViewWithType<(SemanticContextState semanticContextState, DotNetSolutionSemanticContext dotNetSolutionSemanticContext)>
{
    public TreeViewDotNetSolutionSemanticContext(
        (SemanticContextState semanticContextState, DotNetSolutionSemanticContext dotNetSolutionSemanticContext) semanticContextTuple,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                semanticContextTuple,
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
            obj is not TreeViewDotNetSolutionSemanticContext treeViewDotNetSolutionSemanticContext ||
            treeViewDotNetSolutionSemanticContext.Item.dotNetSolutionSemanticContext is null ||
            Item.dotNetSolutionSemanticContext is null)
        {
            return false;
        }

        return treeViewDotNetSolutionSemanticContext.Item.dotNetSolutionSemanticContext.DotNetSolution.NamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString() ==
               Item.dotNetSolutionSemanticContext.DotNetSolution.NamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString();
    }

    public override int GetHashCode()
    {
        return Item.dotNetSolutionSemanticContext.DotNetSolution.NamespacePath.AbsoluteFilePath
            .GetAbsoluteFilePathString()
            .GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            typeof(TreeViewDotNetSolutionSemanticContextDisplay),
            new Dictionary<string, object?>
            {
            {
                nameof(TreeViewDotNetSolutionSemanticContextDisplay.DotNetSolutionSemanticContext),
                Item.dotNetSolutionSemanticContext
            },
            });
    }

    public override async Task LoadChildrenAsync()
    {
        if (Item.dotNetSolutionSemanticContext is null ||
            Item.semanticContextState.DotNetSolutionSemanticContext is null)
            return;

        try
        {
            // TODO: (2023-06-29) I'm rewritting the TextEditor 'ISemanticModel.cs' to be 'ICompilerService.cs'. This method broke in the process and is not high priority to fix yet.
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