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
using Luthetus.Ide.Photino.TestApps.TestAppLuthetusCommonCase.InternalComponents.RenderCounter;

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

    private readonly object _ignoreStateHasChangedCounterLock = new();

    private bool _previousDragStateWrapShouldDisplay;

    private StateHasChangedBoundary _stateHasChangedBoundaryComponentAaa = null!;
    private StateHasChangedBoundary _stateHasChangedBoundaryComponentBbb = null!;

    private RenderCounterDisplay _renderCounterDisplayComponent = null!;

    /// <summary>
    /// At this point in time I'm just messing around. Getting a bit experimental.
    /// This seems silly but I want to do it at least once just to see what happens.
    /// </summary>
    private int _ignoreStateHasChangedCounter;

    private string UnselectableClassCss => DragStateWrap.Value.ShouldDisplay
        ? "balc_unselectable"
        : string.Empty;

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    protected override bool ShouldRender()
    {
        bool shouldRender = true;

        lock (_ignoreStateHasChangedCounterLock)
        {
            if (_ignoreStateHasChangedCounter > 0)
            {
                _ignoreStateHasChangedCounter--;
                shouldRender = false;
            }
        }

        return shouldRender;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _renderCounterDisplayComponent.IncrementCount();

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

    /// <summary>
    /// At this point in time I'm just messing around. Getting a bit experimental.
    /// This seems silly but I want to do it at least once just to see what happens.
    /// </summary>
    private async Task IgnoreOnClickAsync(Func<Task> onClickFunc)
    {
        lock (_ignoreStateHasChangedCounterLock)
        {
            _ignoreStateHasChangedCounter++;
        }

        await onClickFunc.Invoke();
    }

    /// <summary>
    /// At this point in time I'm just messing around. Getting a bit experimental.
    /// This seems silly but I want to do it at least once just to see what happens.
    /// </summary>
    private void IgnoreOnClick(Action onClickAction)
    {
        lock (_ignoreStateHasChangedCounterLock)
        {
            _ignoreStateHasChangedCounter++;
        }

        onClickAction.Invoke();
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}
