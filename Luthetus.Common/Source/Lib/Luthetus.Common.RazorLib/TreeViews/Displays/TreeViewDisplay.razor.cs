using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.TreeViews.Displays;

public partial class TreeViewDisplay : ComponentBase
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    [CascadingParameter]
    public TreeViewContainer TreeViewState { get; set; } = null!;
    [CascadingParameter(Name = "HandleTreeViewOnContextMenu")]
    public Func<MouseEventArgs, TreeViewContainer?, TreeViewNoType?, Task> HandleTreeViewOnContextMenu { get; set; } = null!;
    [CascadingParameter(Name = "TreeViewMouseEventHandler")]
    public TreeViewMouseEventHandler TreeViewMouseEventHandler { get; set; } = null!;
    [CascadingParameter(Name = "TreeViewKeyboardEventHandler")]
    public TreeViewKeyboardEventHandler TreeViewKeyboardEventHandler { get; set; } = null!;
    [CascadingParameter(Name = "OffsetPerDepthInPixels")]
    public int OffsetPerDepthInPixels { get; set; } = 12;
    [CascadingParameter(Name = "LuthetusTreeViewIconWidth")]
    public int WidthOfTitleExpansionChevron { get; set; } = 16;

    [Parameter, EditorRequired]
    public TreeViewNoType TreeViewNoType { get; set; } = null!;
    [Parameter, EditorRequired]
    public int Depth { get; set; }

    private ElementReference? _treeViewTitleElementReference;
    private Key<TreeViewChanged> _previousTreeViewChangedKey = Key<TreeViewChanged>.Empty;
    private bool _previousIsActive;

    private int OffsetInPixels => OffsetPerDepthInPixels * Depth;

    private bool IsSelected => TreeViewState.SelectedNodeBag.Any(x => x.Key == TreeViewNoType.Key);

    private bool IsActive => TreeViewState.ActiveNode is not null &&
                             TreeViewState.ActiveNode.Key == TreeViewNoType.Key;

    private string IsSelectedCssClass => IsSelected ? "luth_selected" : string.Empty;
    private string IsActiveCssClass => IsActive ? "luth_active" : string.Empty;

    protected override bool ShouldRender()
    {
        if (_previousTreeViewChangedKey != TreeViewNoType.TreeViewChangedKey)
        {
            _previousTreeViewChangedKey = TreeViewNoType.TreeViewChangedKey;
            return true;
        }

        return false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var localIsActive = IsActive;

        if (_previousIsActive != localIsActive)
        {
            _previousIsActive = localIsActive;

            if (localIsActive)
                await FocusAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task FocusAsync()
    {
        try
        {
            var localTreeViewTitleElementReference = _treeViewTitleElementReference;

            if (localTreeViewTitleElementReference is not null)
                await localTreeViewTitleElementReference.Value.FocusAsync();
        }
        catch (Exception)
        {
            // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
            //             This bug is seemingly happening randomly. I have a suspicion
            //             that there are race-condition exceptions occurring with "FocusAsync"
            //             on an ElementReference.
        }
    }

    private void HandleExpansionChevronOnMouseDown(TreeViewNoType localTreeViewNoType)
    {
        if (!localTreeViewNoType.IsExpandable)
            return;

        localTreeViewNoType.IsExpanded = !localTreeViewNoType.IsExpanded;

        if (localTreeViewNoType.IsExpanded)
        {
            _ = Task.Run(async () =>
            {
                await localTreeViewNoType.LoadChildBagAsync();

                TreeViewService.ReRenderNode(TreeViewState.Key, localTreeViewNoType);
            });
        }
        else
        {
            TreeViewService.ReRenderNode(TreeViewState.Key, localTreeViewNoType);
        }
    }

    private void ManuallyPropagateOnContextMenu(
        MouseEventArgs mouseEventArgs,
        TreeViewContainer? treeViewState,
        TreeViewNoType treeViewNoType)
    {
        var treeViewCommandParameter = new TreeViewCommandArgs(
            TreeViewService,
            TreeViewState,
            TreeViewNoType,
            FocusAsync,
            null,
            mouseEventArgs,
            null);

        TreeViewMouseEventHandler.OnMouseDown(treeViewCommandParameter);

        _ = Task.Run(async () => await HandleTreeViewOnContextMenu.Invoke(
            mouseEventArgs,
            treeViewState,
            treeViewNoType));
    }

    private void HandleOnClick(MouseEventArgs? mouseEventArgs)
    {
        var treeViewCommandParameter = new TreeViewCommandArgs(
            TreeViewService,
            TreeViewState,
            TreeViewNoType,
            FocusAsync,
            null,
            mouseEventArgs,
            null);

        TreeViewMouseEventHandler.OnClick(treeViewCommandParameter);
    }

    private void HandleOnDoubleClick(MouseEventArgs? mouseEventArgs)
    {
        var treeViewCommandParameter = new TreeViewCommandArgs(
            TreeViewService,
            TreeViewState,
            TreeViewNoType,
            FocusAsync,
            null,
            mouseEventArgs,
            null);

        TreeViewMouseEventHandler.OnDoubleClick(treeViewCommandParameter);
    }

    private void HandleOnMouseDown(MouseEventArgs? mouseEventArgs)
    {
        var treeViewCommandParameter = new TreeViewCommandArgs(
            TreeViewService,
            TreeViewState,
            TreeViewNoType,
            FocusAsync,
            null,
            mouseEventArgs,
            null);

        TreeViewMouseEventHandler.OnMouseDown(treeViewCommandParameter);
    }

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == "ContextMenu")
        {
            var mouseEventArgs = new MouseEventArgs { Button = -1 };
            ManuallyPropagateOnContextMenu(mouseEventArgs, TreeViewState, TreeViewNoType);
        }
    }
}