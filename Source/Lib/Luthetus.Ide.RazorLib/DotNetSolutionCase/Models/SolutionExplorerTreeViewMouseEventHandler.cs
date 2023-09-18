using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.EditorCase.States;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.Models;

public class SolutionExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly EditorSync _editorSync;
    private readonly IDispatcher _dispatcher;

    public SolutionExplorerTreeViewMouseEventHandler(
        EditorSync editorSync,
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
        : base(treeViewService)
    {
        _editorSync = editorSync;
        _dispatcher = dispatcher;
    }

    public override void OnDoubleClick(TreeViewCommandParameter treeViewCommandParameter)
    {
        base.OnDoubleClick(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode is not TreeViewNamespacePath treeViewNamespacePath)
            return;

        _dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            _editorSync,
            treeViewNamespacePath.Item.AbsolutePath,
            true));
    }
}