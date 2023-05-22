using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Ide.ClassLib.Store.PanelCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.ClassLib.Panel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Ide.RazorLib.Panel;

public partial class PanelDisplay : FluxorComponent
{
    [Inject]
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public PanelRecordKey PanelRecordKey { get; set; } = null!;
    [Parameter, EditorRequired]
    public ElementDimensions AdjacentElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public DimensionAttributeKind DimensionAttributeKind { get; set; }
    [Parameter, EditorRequired]
    public Func<Task> ReRenderSelfAndAdjacentElementDimensionsFunc { get; set; } = null!;
    [Parameter]
    public string CssClassString { get; set; } = null!;

    public string DimensionAttributeModificationPurpose => $"take_size_of_adjacent_hidden_panel_{PanelRecordKey}";

    private string PanelPositionCssClass => GetPanelPositionCssClass();
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            await PassAlongSizeIfHiddenAsync();

        await base.OnAfterRenderAsync(firstRender);
    }

    private string GetPanelPositionCssClass()
    {
        var position = string.Empty;

        if (PanelFacts.LeftPanelRecordKey == PanelRecordKey)
        {
            position = "left";
        }
        else if (PanelFacts.RightPanelRecordKey == PanelRecordKey)
        {
            position = "right";
        }
        else if (PanelFacts.BottomPanelRecordKey == PanelRecordKey)
        {
            position = "bottom";
        }

        return $"luth_ide_panel_{position}";
    }

    private async Task PassAlongSizeIfHiddenAsync()
    {
        var panelsCollection = PanelsCollectionWrap.Value;

        var panelRecord = panelsCollection.PanelRecordsList
            .FirstOrDefault(x => x.PanelRecordKey == PanelRecordKey);

        if (panelRecord is not null)
        {
            var activePanelTab = panelRecord.PanelTabs
                .FirstOrDefault(x =>
                    x.PanelTabKey == panelRecord.ActivePanelTabKey);

            var adjacentElementSizeDimensionAttribute = AdjacentElementDimensions.DimensionAttributes
                .First(x =>
                    x.DimensionAttributeKind == DimensionAttributeKind);

            var indexOfPreviousPassAlong = adjacentElementSizeDimensionAttribute.DimensionUnits.FindIndex(
                x => x.Purpose == DimensionAttributeModificationPurpose);

            if (activePanelTab is null &&
                indexOfPreviousPassAlong == -1)
            {
                var panelRecordSizeDimensionsAttribute =
                    panelRecord.ElementDimensions.DimensionAttributes.First(x =>
                        x.DimensionAttributeKind == DimensionAttributeKind);

                var panelRecordPercentageSize = panelRecordSizeDimensionsAttribute.DimensionUnits
                    .First(x => x.DimensionUnitKind == DimensionUnitKind.Percentage);

                adjacentElementSizeDimensionAttribute.DimensionUnits.Add(new DimensionUnit
                {
                    Value = panelRecordPercentageSize.Value,
                    DimensionUnitKind = panelRecordPercentageSize.DimensionUnitKind,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    Purpose = DimensionAttributeModificationPurpose
                });

                await ReRenderSelfAndAdjacentElementDimensionsFunc.Invoke();
            }
            else if (activePanelTab is not null &&
                     indexOfPreviousPassAlong != -1)
            {
                adjacentElementSizeDimensionAttribute.DimensionUnits.RemoveAt(indexOfPreviousPassAlong);

                await ReRenderSelfAndAdjacentElementDimensionsFunc.Invoke();
            }
        }
    }

    private string GetElementDimensionsStyleString(
        PanelRecord? panelRecord,
        PanelTab? activePanelTab)
    {
        if (activePanelTab is null)
        {
            return "calc(" +
                   "var(--luth_ide_panel-tabs-font-size)" +
                   " + var(--luth_ide_panel-tabs-margin)" +
                   " + var(--luth_ide_panel-tabs-bug-are-not-aligning-need-to-fix-todo))";
        }

        return panelRecord?.ElementDimensions.StyleString ?? string.Empty;
    }

    private Task TopDropzoneOnMouseUp(MouseEventArgs mouseEventArgs)
    {
        var panelsCollection = PanelsCollectionWrap.Value;

        var panelRecord = panelsCollection.PanelRecordsList
            .FirstOrDefault(x => x.PanelRecordKey == PanelRecordKey);

        if (panelRecord is null)
            return Task.CompletedTask;

        var panelDragEventArgs = panelsCollection.PanelDragEventArgs;

        if (panelDragEventArgs is not null)
        {
            Dispatcher.Dispatch(new PanelsCollection.DisposePanelTabAction(
                panelDragEventArgs.Value.ParentPanelRecord.PanelRecordKey,
                panelDragEventArgs.Value.TagDragTarget.PanelTabKey));

            Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
                panelRecord.PanelRecordKey,
                panelDragEventArgs.Value.TagDragTarget));
        }

        return Task.CompletedTask;
    }

    private Task BottomDropzoneOnMouseUp(MouseEventArgs mouseEventArgs)
    {
        var panelsCollection = PanelsCollectionWrap.Value;

        var panelRecord = panelsCollection.PanelRecordsList
            .FirstOrDefault(x => x.PanelRecordKey == PanelRecordKey);

        if (panelRecord is null)
            return Task.CompletedTask;

        var panelDragEventArgs = panelsCollection.PanelDragEventArgs;

        if (panelDragEventArgs is not null)
        {
            Dispatcher.Dispatch(new PanelsCollection.DisposePanelTabAction(
                panelDragEventArgs.Value.ParentPanelRecord.PanelRecordKey,
                panelDragEventArgs.Value.TagDragTarget.PanelTabKey));

            Dispatcher.Dispatch(new PanelsCollection.RegisterPanelTabAction(
                panelRecord.PanelRecordKey,
                panelDragEventArgs.Value.TagDragTarget));
        }

        return Task.CompletedTask;
    }
}
