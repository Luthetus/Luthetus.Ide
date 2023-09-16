using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Dropdown.Models;
using Luthetus.Common.RazorLib.KeyCase;
using Luthetus.Common.RazorLib.Menu.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Displays;

public partial class CompilerServiceExplorerTreeViewContextMenu : ComponentBase
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITreeViewCommandParameter TreeViewCommandParameter { get; set; } = null!;

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

    private MenuRecord GetMenuRecord(ITreeViewCommandParameter treeViewCommandParameter)
    {
        return MenuRecord.Empty;
    }

    /// <summary>
    /// This method I believe is causing bugs
    /// <br/><br/>
    /// For example, when removing a C# Project the
    /// solution is reloaded and a new root is made.
    /// <br/><br/>
    /// Then there is a timing issue where the new root is made and set
    /// as the root. But this method erroneously reloads the old root.
    /// </summary>
    /// <param name="treeViewModel"></param>
    private async Task ReloadTreeViewModel(
        TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildrenAsync();

        TreeViewService.ReRenderNode(
            DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            treeViewModel);

        TreeViewService.MoveUp(
            DotNetSolutionState.TreeViewSolutionExplorerStateKey,
            false);
    }

    public static string GetContextMenuCssStyleString(
        ITreeViewCommandParameter? treeViewCommandParameter)
    {
        if (treeViewCommandParameter?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left =
            $"left: {treeViewCommandParameter.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";

        var top =
            $"top: {treeViewCommandParameter.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
}