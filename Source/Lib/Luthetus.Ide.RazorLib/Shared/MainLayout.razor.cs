using BlazorCommon.RazorLib.Dimensions;
using BlazorCommon.RazorLib.Options;
using BlazorCommon.RazorLib.Resize;
using BlazorCommon.RazorLib.Store.ApplicationOptions;
using BlazorCommon.RazorLib.Store.DragCase;
using BlazorStudio.ClassLib.Store.PanelCase;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Luthetus.Ide.ClassLib.Dimensions;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    private string _message = string.Empty;

    private string UnselectableClassCss => DragStateWrap.Value.ShouldDisplay
        ? "balc_unselectable"
        : string.Empty;

    private bool _previousDragStateWrapShouldDisplay;

    private ElementDimensions _bodyElementDimensions = new();
    private ElementDimensions _footerElementDimensions = new();

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

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
                Value = SizeFacts.Bstudio.Header.Height.Value / 2,
                DimensionUnitKind = SizeFacts.Bstudio.Header.Height.DimensionUnitKind,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await TextEditorService.Options.SetFromLocalStorageAsync();
            await AppOptionsService.SetFromLocalStorageAsync();

            if (System.IO.File.Exists("/home/hunter/Repos/Demos/TestApp/TestApp.sln"))
            {
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void FontStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (_previousDragStateWrapShouldDisplay != DragStateWrap.Value.ShouldDisplay)
        {
            _previousDragStateWrapShouldDisplay = DragStateWrap.Value.ShouldDisplay;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task ReRenderAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}
