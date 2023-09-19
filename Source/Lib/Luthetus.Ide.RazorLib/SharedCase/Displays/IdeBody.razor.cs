using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Panel.States;
using Luthetus.Common.RazorLib.Resize.Displays;
using Luthetus.Common.RazorLib.StateHasChangedBoundaryCase.Displays;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.SharedCase.Displays;

public partial class IdeBody : ComponentBase
{
    [Inject]
    private IState<PanelsState> PanelsCollectionWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions BodyElementDimensions { get; set; } = null!;

    private ElementDimensions _editorElementDimensions = new();
    private StateHasChangedBoundary? _leftPanelStateHasChangedBoundaryComponent;
    private StateHasChangedBoundary? _editorStateHasChangedBoundaryComponent;
    private StateHasChangedBoundary? _rightPanelStateHasChangedBoundaryComponent;

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

    private async Task ReRenderLeftPanelAndEditor()
    {
        await (_leftPanelStateHasChangedBoundaryComponent?
            .InvokeStateHasChangedAsync() ?? Task.CompletedTask);

        await (_editorStateHasChangedBoundaryComponent?
            .InvokeStateHasChangedAsync() ?? Task.CompletedTask);
    }

    private async Task ReRenderEditorAndRightPanel()
    {
        await (_editorStateHasChangedBoundaryComponent?
            .InvokeStateHasChangedAsync() ?? Task.CompletedTask);

        await (_rightPanelStateHasChangedBoundaryComponent?
            .InvokeStateHasChangedAsync() ?? Task.CompletedTask);
    }
}