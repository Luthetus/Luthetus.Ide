using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Resize;
using Luthetus.Ide.ClassLib.Store.PanelCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shared;

public partial class LuthetusTextEditorBody : ComponentBase
{
    [Inject]
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions BodyElementDimensions { get; set; } = null!;

    private ElementDimensions _editorElementDimensions = new();

    protected override void OnInitialized()
    {
        var editorWidth = _editorElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

        editorWidth.DimensionUnits.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 33.3333,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
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