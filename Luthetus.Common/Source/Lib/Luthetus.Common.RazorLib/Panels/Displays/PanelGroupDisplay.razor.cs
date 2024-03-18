using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Panels.Displays;

public partial class PanelGroupDisplay : FluxorComponent
{
    [Inject]
    private IState<PanelsState> PanelsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<PanelGroup> PanelGroupKey { get; set; } = Key<PanelGroup>.Empty;
    [Parameter, EditorRequired]
    public ElementDimensions AdjacentElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public DimensionAttributeKind DimensionAttributeKind { get; set; }
    [Parameter, EditorRequired]
    public Func<Task> ReRenderSelfAndAdjacentElementDimensionsFunc { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = null!;
    [Parameter]
    public RenderFragment? JustifyEndRenderFragment { get; set; } = null;

    public string DimensionAttributeModificationPurpose => $"take_size_of_adjacent_hidden_panel_{PanelGroupKey}";

    private string PanelPositionCssClass => GetPanelPositionCssClass();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            await PassAlongSizeIfHiddenAsync();

        await base.OnAfterRenderAsync(firstRender);
    }

	private ImmutableArray<IPanelTab> GetTabList(PanelGroup panelGroup)
	{
		panelGroup.Dispatcher = Dispatcher;

		var tabList = new List<IPanelTab>();

		foreach (var panelTab in panelGroup.TabList)
		{
            panelTab.TabGroup = panelGroup;
            panelTab.Dispatcher = Dispatcher;
            panelTab.DialogService = DialogService;
			panelTab.JsRuntime = JsRuntime;
			tabList.Add(panelTab);
		}

		return tabList.ToImmutableArray();
	}

    private string GetHtmlId()
	{
		return GetPanelPositionCssClass();
	}

    private string GetPanelPositionCssClass()
    {
        var position = string.Empty;

        if (PanelFacts.LeftPanelRecordKey == PanelGroupKey)
            position = "left";
        else if (PanelFacts.RightPanelRecordKey == PanelGroupKey)
            position = "right";
        else if (PanelFacts.BottomPanelRecordKey == PanelGroupKey)
            position = "bottom";

        return $"luth_ide_panel_{position}";
    }

    private async Task PassAlongSizeIfHiddenAsync()
    {
        var panelsState = PanelsStateWrap.Value;

        var panelGroup = panelsState.PanelGroupList.FirstOrDefault(x => x.Key == PanelGroupKey);

        if (panelGroup is not null)
        {
            var activePanelTab = panelGroup.TabList.FirstOrDefault(
                x => x.Key == panelGroup.ActiveTabKey);

            var adjacentElementSizeDimensionAttribute = AdjacentElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind);

            var indexOfPreviousPassAlong = adjacentElementSizeDimensionAttribute.DimensionUnitList.FindIndex(
                x => x.Purpose == DimensionAttributeModificationPurpose);

            if (activePanelTab is null && indexOfPreviousPassAlong == -1)
            {
                var panelGroupSizeDimensionsAttribute = panelGroup.ElementDimensions.DimensionAttributeList.First(
                    x => x.DimensionAttributeKind == DimensionAttributeKind);

                var panelGroupPercentageSize = panelGroupSizeDimensionsAttribute.DimensionUnitList.First(
                    x => x.DimensionUnitKind == DimensionUnitKind.Percentage);

                adjacentElementSizeDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
                {
                    Value = panelGroupPercentageSize.Value,
                    DimensionUnitKind = panelGroupPercentageSize.DimensionUnitKind,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    Purpose = DimensionAttributeModificationPurpose
                });

                await ReRenderSelfAndAdjacentElementDimensionsFunc.Invoke();
            }
            else if (activePanelTab is not null && indexOfPreviousPassAlong != -1)
            {
                adjacentElementSizeDimensionAttribute.DimensionUnitList.RemoveAt(indexOfPreviousPassAlong);

                await ReRenderSelfAndAdjacentElementDimensionsFunc.Invoke();
            }
        }
    }

    private string GetElementDimensionsStyleString(PanelGroup? panelGroup, IPanelTab? activePanelTab)
    {
        if (activePanelTab is null)
        {
            return "calc(" +
                   "var(--luth_ide_panel-tabs-font-size)" +
                   " + var(--luth_ide_panel-tabs-margin)" +
                   " + var(--luth_ide_panel-tabs-bug-are-not-aligning-need-to-fix-todo))";
        }

        return panelGroup?.ElementDimensions.StyleString ?? string.Empty;
    }

    private Task TopDropzoneOnMouseUp(MouseEventArgs mouseEventArgs)
    {
        var panelsState = PanelsStateWrap.Value;

        var panelGroup = panelsState.PanelGroupList.FirstOrDefault(x => x.Key == PanelGroupKey);

        if (panelGroup is null)
            return Task.CompletedTask;

        var panelDragEventArgs = panelsState.DragEventArgs;

        if (panelDragEventArgs is not null)
        {
            Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(
                panelDragEventArgs.Value.PanelGroup.Key,
                panelDragEventArgs.Value.PanelTab.Key));

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
                panelGroup.Key,
                panelDragEventArgs.Value.PanelTab,
                true));

            Dispatcher.Dispatch(new PanelsState.SetDragEventArgsAction(null));

            Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
            {
                ShouldDisplay = false,
                MouseEventArgs = null
            }));
        }

        return Task.CompletedTask;
    }

    private Task BottomDropzoneOnMouseUp(MouseEventArgs mouseEventArgs)
    {
        var panelsState = PanelsStateWrap.Value;

        var panelGroup = panelsState.PanelGroupList.FirstOrDefault(x => x.Key == PanelGroupKey);

        if (panelGroup is null)
            return Task.CompletedTask;

        var panelDragEventArgs = panelsState.DragEventArgs;

        if (panelDragEventArgs is not null)
        {
            Dispatcher.Dispatch(new PanelsState.DisposePanelTabAction(
                panelDragEventArgs.Value.PanelGroup.Key,
                panelDragEventArgs.Value.PanelTab.Key));

            Dispatcher.Dispatch(new PanelsState.RegisterPanelTabAction(
                panelGroup.Key,
                panelDragEventArgs.Value.PanelTab,
                false));

            Dispatcher.Dispatch(new PanelsState.SetDragEventArgsAction(null));

            Dispatcher.Dispatch(new DragState.WithAction(inState => inState with
            {
                ShouldDisplay = false,
                MouseEventArgs = null,
            }));
        }

        return Task.CompletedTask;
    }
}