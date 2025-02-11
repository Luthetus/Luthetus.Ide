using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppResizeHandleHeight : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    public int ResizeHandleHeightInPixels
    {
        get => AppOptionsService.GetAppOptionsState().Options.ResizeHandleHeightInPixels;
        set
        {
            if (value < AppOptionsState.MINIMUM_RESIZE_HANDLE_HEIGHT_IN_PIXELS)
                value = AppOptionsState.MINIMUM_RESIZE_HANDLE_HEIGHT_IN_PIXELS;

            AppOptionsService.SetResizeHandleHeight(value);
        }
    }

    protected override void OnInitialized()
    {
        AppOptionsService.AppOptionsStateChanged += AppOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void AppOptionsStateWrapOnStateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        AppOptionsService.AppOptionsStateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}
