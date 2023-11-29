using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.ProgramExecutions.States;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Ide.RazorLib.CSharpProjectForms.Displays;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers;

public partial class TestExplorerContextMenu : ComponentBase
{
    [Inject]
    private IState<TerminalSessionState> TerminalSessionStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;
    [Inject]
    private InputFileSync InputFileSync { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewCommandArgs TreeViewCommandArgs { get; set; } = null!;

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

    /// <summary>
    /// The program is currently running using Photino locally on the user's computer
    /// therefore this static solution works without leaking any information.
    /// </summary>
    public static TreeViewNoType? ParentOfCutFile;

    private MenuRecord GetMenuRecord(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.TargetNode is null)
            return MenuRecord.Empty;

        var menuRecordsBag = new List<MenuOptionRecord>();

        if (!menuRecordsBag.Any())
            return MenuRecord.Empty;

        return new MenuRecord(menuRecordsBag.ToImmutableArray());
    }

    private MenuOptionRecord[] GetDebugMenuOptions(TreeViewNamespacePath treeViewModel)
    {
        return new MenuOptionRecord[]
        {
            // new MenuOptionRecord(
            //     $"namespace: {treeViewModel.Item.Namespace}",
            //     MenuOptionKind.Read)
        };
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
    private async Task ReloadTreeViewModel(TreeViewNoType? treeViewModel)
    {
        if (treeViewModel is null)
            return;

        await treeViewModel.LoadChildBagAsync();

        TreeViewService.ReRenderNode(DotNetSolutionState.TreeViewSolutionExplorerStateKey, treeViewModel);
        TreeViewService.MoveUp(DotNetSolutionState.TreeViewSolutionExplorerStateKey, false);
    }

    public static string GetContextMenuCssStyleString(TreeViewCommandArgs? commandArgs)
    {
        if (commandArgs?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left =
            $"left: {commandArgs.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";

        var top =
            $"top: {commandArgs.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
}