using BlazorCommon.RazorLib.Dimensions;
using BlazorCommon.RazorLib.Resize;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shared;

public partial class BlazorTextEditorExplorers : ComponentBase
{
    [Parameter, EditorRequired]
    public ElementDimensions ExplorersElementDimensions { get; set; } = null!;

    private ElementDimensions _solutionExplorerElementDimensions = new();
    private ElementDimensions _folderExplorerElementDimensions = new();

    protected override void OnInitialized()
    {
        var solutionExplorerHeight = _solutionExplorerElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

        solutionExplorerHeight.DimensionUnits.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 50,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        var folderExplorerHeight = _folderExplorerElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

        folderExplorerHeight.DimensionUnits.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 50,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        base.OnInitialized();
    }

    private async Task ReRenderAsync()
    {
        await InvokeAsync(StateHasChanged);
    }
}