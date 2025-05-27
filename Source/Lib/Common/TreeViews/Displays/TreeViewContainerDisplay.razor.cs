using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Displays;

/// <summary>
/// TODO: SphagettiCode - The context menu logic feels scuffed. A field is used to track the
/// "_mostRecentContextMenuEvent". This feels quite wrong and should be looked into. (2023-09-19)
/// </summary>
public partial class TreeViewContainerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
    private CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TreeViewContainer> TreeViewContainerKey { get; set; } = Key<TreeViewContainer>.Empty;
    [Parameter, EditorRequired]
    public TreeViewMouseEventHandler TreeViewMouseEventHandler { get; set; } = null!;
    [Parameter, EditorRequired]
    public TreeViewKeyboardEventHandler TreeViewKeyboardEventHandler { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    /// <summary>If a consumer of the TreeView component does not have logic for their own DropdownComponent, then one can provide a RenderFragment and a dropdown will be rendered for the consumer and their RenderFragment is rendered within that dropdown.<br/><br/>If one has their own DropdownComponent, then it is recommended that they use <see cref="OnContextMenuFunc"/> instead.</summary>
    [Parameter]
    public RenderFragment<TreeViewCommandArgs>? OnContextMenuRenderFragment { get; set; }
    /// <summary>If a consumer of the TreeView component does not have logic for their own DropdownComponent, then it is recommended to use <see cref="OnContextMenuRenderFragment"/><br/><br/> <see cref="OnContextMenuFunc"/> allows one to be notified of the ContextMenu event along with the necessary parameters by being given <see cref="TreeViewCommandArgs"/></summary>
    [Parameter]
    public Func<TreeViewCommandArgs, Task>? OnContextMenuFunc { get; set; }

    private TreeViewCommandArgs _treeViewContextMenuCommandArgs;
    private ElementReference? _treeViewStateDisplayElementReference;

    protected override void OnInitialized()
    {
        TreeViewService.TreeViewStateChanged += OnTreeViewStateChanged;
        base.OnInitialized();
    }

    private int GetRootDepth(TreeViewNoType rootNode)
    {
        return rootNode is TreeViewAdhoc ? -1 : 0;
    }

    private async Task HandleTreeViewOnKeyDownWithPreventScroll(
        KeyboardEventArgs keyboardEventArgs,
        TreeViewContainer? treeViewContainer)
    {
        if (treeViewContainer is null)
            return;

        var treeViewCommandArgs = new TreeViewCommandArgs(
            TreeViewService,
            treeViewContainer,
            null,
            async () =>
            {
                _treeViewContextMenuCommandArgs = default;
                await InvokeAsync(StateHasChanged);

                var localTreeViewStateDisplayElementReference = _treeViewStateDisplayElementReference;

                try
                {
                    if (localTreeViewStateDisplayElementReference.HasValue)
                    {
                        await localTreeViewStateDisplayElementReference.Value
                            .FocusAsync()
                            .ConfigureAwait(false);
                    }
                }
                catch (Exception)
                {
                    // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                    //             This bug is seemingly happening randomly. I have a suspicion
                    //             that there are race-condition exceptions occurring with "FocusAsync"
                    //             on an ElementReference.
                }
            },
            null,
            null,
            keyboardEventArgs);

        await TreeViewKeyboardEventHandler
            .OnKeyDownAsync(treeViewCommandArgs)
            .ConfigureAwait(false);
    }

    private async Task HandleTreeViewOnContextMenu(
        MouseEventArgs? mouseEventArgs,
        Key<TreeViewContainer> treeViewContainerKey,
        TreeViewNoType? treeViewMouseWasOver)
    {
        if (treeViewContainerKey == Key<TreeViewContainer>.Empty || mouseEventArgs is null)
            return;

        var treeViewContainer = TreeViewService.GetTreeViewContainer(TreeViewContainerKey);
        // Validate that the treeViewContainer did not change out from under us
        if (treeViewContainer is null || treeViewContainer.Key != treeViewContainerKey)
            return;

        ContextMenuFixedPosition contextMenuFixedPosition;
        TreeViewNoType contextMenuTargetTreeViewNoType;

        if (mouseEventArgs.Button == -1) // -1 here means ContextMenu event was from keyboard
        {
            if (treeViewContainer.ActiveNode is null)
                return;

            // If dedicated context menu button or shift + F10 was pressed as opposed to
            // a mouse RightClick then use JavaScript to determine the ContextMenu position.
            contextMenuFixedPosition = await CommonBackgroundTaskApi.JsRuntimeCommonApi
                .GetTreeViewContextMenuFixedPosition(TreeViewService.GetNodeElementId(treeViewContainer.ActiveNode))
                .ConfigureAwait(false);

            contextMenuTargetTreeViewNoType = treeViewContainer.ActiveNode;
        }
        else
        {
            // If a mouse RightClick caused the event then
            // use the MouseEventArgs to determine the ContextMenu position
            if (treeViewMouseWasOver is null)
            {
                // 'whitespace' of the TreeView was right clicked as opposed to
                // a TreeView node and the event should be ignored.
                return;
            }

            contextMenuFixedPosition = new ContextMenuFixedPosition(
                true,
                mouseEventArgs.ClientX,
                mouseEventArgs.ClientY);

            contextMenuTargetTreeViewNoType = treeViewMouseWasOver;
        }

        _treeViewContextMenuCommandArgs = new TreeViewCommandArgs(
            TreeViewService,
            treeViewContainer,
            contextMenuTargetTreeViewNoType,
            async () =>
            {
                _treeViewContextMenuCommandArgs = default;
                await InvokeAsync(StateHasChanged);

                var localTreeViewStateDisplayElementReference = _treeViewStateDisplayElementReference;

                try
                {
                    if (localTreeViewStateDisplayElementReference.HasValue)
                    {
                        await localTreeViewStateDisplayElementReference.Value
                            .FocusAsync()
                            .ConfigureAwait(false);
                    }
                }
                catch (Exception)
                {
                    // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
                    //             This bug is seemingly happening randomly. I have a suspicion
                    //             that there are race-condition exceptions occurring with "FocusAsync"
                    //             on an ElementReference.
                }
            },
            contextMenuFixedPosition,
            mouseEventArgs,
            null);

        if (OnContextMenuFunc is not null)
		{
            CommonBackgroundTaskApi.Enqueue(new CommonWorkArgs
            {
    			WorkKind = CommonWorkKind.TreeView_HandleTreeViewOnContextMenu,
            	OnContextMenuFunc = OnContextMenuFunc,
                TreeViewContextMenuCommandArgs = _treeViewContextMenuCommandArgs,
            });
        }

        await InvokeAsync(StateHasChanged);
    }

    private string GetHasActiveNodeCssClass(TreeViewContainer? treeViewContainer)
    {
        if (treeViewContainer?.ActiveNode is null)
            return string.Empty;

        return "luth_active";
    }

    private string GetContextMenuCssStyleString()
    {
        if (_treeViewContextMenuCommandArgs.TreeViewService is null || _treeViewContextMenuCommandArgs.ContextMenuFixedPosition is null)
        {
            // This should never happen.
            return "display: none;";
        }

        var left =
            $"left: {_treeViewContextMenuCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";

        var top =
            $"top: {_treeViewContextMenuCommandArgs.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
    
    public async void OnTreeViewStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	TreeViewService.TreeViewStateChanged -= OnTreeViewStateChanged;
    }
}