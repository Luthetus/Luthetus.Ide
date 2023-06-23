using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Options;
using Luthetus.Common.RazorLib.Resize;
using Luthetus.Common.RazorLib.StateHasChangedBoundaryCase;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Common.RazorLib.Store.DragCase;

namespace Luthetus.Ide.Photino.TestApps.TestAppLuthetusCommonCase.InternalComponents;

public partial class TestAppLuthetusCommonLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private string UnselectableClassCss => DragStateWrap.Value.ShouldDisplay
        ? "balc_unselectable"
        : string.Empty;

    private bool _previousDragStateWrapShouldDisplay;

    private ElementDimensions _bodyElementDimensions = new();

    private StateHasChangedBoundary _bodyAndFooterStateHasChangedBoundaryComponent = null!;

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await AppOptionsService.SetFromLocalStorageAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
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

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}
