using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppResizeHandleWidth : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    public int ResizeHandleWidthInPixels
    {
        get => AppOptionsService.GetAppOptionsState().Options.ResizeHandleWidthInPixels;
        set
        {
            if (value < AppOptionsState.MINIMUM_RESIZE_HANDLE_WIDTH_IN_PIXELS)
                value = AppOptionsState.MINIMUM_RESIZE_HANDLE_WIDTH_IN_PIXELS;

            AppOptionsService.SetResizeHandleWidth(value);
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
