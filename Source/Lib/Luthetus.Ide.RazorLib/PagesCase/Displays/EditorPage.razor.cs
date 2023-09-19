using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Panel.States;
using Luthetus.Common.RazorLib.Resize.Displays;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.PagesCase.Displays;

public partial class EditorPage : ComponentBase
{
    [Inject]
    private IState<PanelsState> PanelsCollectionWrap { get; set; } = null!;

    private ElementDimensions _bodyElementDimensions = new();

    protected override void OnInitialized()
    {
        var bodyHeight = _bodyElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

        bodyHeight.DimensionUnits.AddRange(new[]
        {
        new DimensionUnit
        {
            Value = 78,
            DimensionUnitKind = DimensionUnitKind.Percentage
        },
        new DimensionUnit
        {
            Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
            DimensionUnitKind = DimensionUnitKind.Pixels,
            DimensionOperatorKind = DimensionOperatorKind.Subtract
        },
        new DimensionUnit
        {
            Value = SizeFacts.Ide.Header.Height.Value / 2,
            DimensionUnitKind = SizeFacts.Ide.Header.Height.DimensionUnitKind,
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