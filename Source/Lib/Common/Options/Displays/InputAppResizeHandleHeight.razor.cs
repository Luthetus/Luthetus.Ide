using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Options.States;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppResizeHandleHeight : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    public int ResizeHandleHeightInPixels
    {
        get => AppOptionsService.AppOptionsStateWrap.Value.Options.ResizeHandleHeightInPixels;
        set
        {
            if (value < AppOptionsState.MINIMUM_RESIZE_HANDLE_HEIGHT_IN_PIXELS)
                value = AppOptionsState.MINIMUM_RESIZE_HANDLE_HEIGHT_IN_PIXELS;

            AppOptionsService.SetResizeHandleHeight(value);
        }
    }

    protected override void OnInitialized()
    {
        AppOptionsService.AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        AppOptionsService.AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}
