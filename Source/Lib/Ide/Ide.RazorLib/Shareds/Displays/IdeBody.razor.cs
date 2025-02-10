using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Luthetus.Common.RazorLib.StateHasChangedBoundaries.Displays;
using Luthetus.Common.RazorLib.Options.States;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class IdeBody : ComponentBase
{
    [Inject]
    private IPanelService PanelService { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions BodyElementDimensions { get; set; } = null!;

    private ElementDimensions _editorElementDimensions = new();
    private StateHasChangedBoundary? _leftPanelStateHasChangedBoundaryComponent;
    private StateHasChangedBoundary? _editorStateHasChangedBoundaryComponent;
    private StateHasChangedBoundary? _rightPanelStateHasChangedBoundaryComponent;

    protected override void OnInitialized()
    {
        _editorElementDimensions.WidthDimensionAttribute.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit(
            	33.3333,
            	DimensionUnitKind.Percentage),
            new DimensionUnit(
            	AppOptionsStateWrap.Value.Options.ResizeHandleWidthInPixels / 2,
            	DimensionUnitKind.Pixels,
            	DimensionOperatorKind.Subtract)
        });

        base.OnInitialized();
    }

    private async Task ReRenderLeftPanelAndEditor()
    {
        await (_leftPanelStateHasChangedBoundaryComponent?.InvokeStateHasChangedAsync() ?? Task.CompletedTask);
        await (_editorStateHasChangedBoundaryComponent?.InvokeStateHasChangedAsync() ?? Task.CompletedTask);
    }

    private async Task ReRenderEditorAndRightPanel()
    {
        await (_editorStateHasChangedBoundaryComponent?.InvokeStateHasChangedAsync() ?? Task.CompletedTask);
        await (_rightPanelStateHasChangedBoundaryComponent?.InvokeStateHasChangedAsync() ?? Task.CompletedTask);
    }
}