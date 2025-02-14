using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppResizeHandleHeight : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    public int ResizeHandleHeightInPixels
    {
        get => CommonApi.AppOptionApi.GetAppOptionsState().Options.ResizeHandleHeightInPixels;
        set
        {
            if (value < AppOptionsState.MINIMUM_RESIZE_HANDLE_HEIGHT_IN_PIXELS)
                value = AppOptionsState.MINIMUM_RESIZE_HANDLE_HEIGHT_IN_PIXELS;

            CommonApi.AppOptionApi.SetResizeHandleHeight(value);
        }
    }

    protected override void OnInitialized()
    {
        CommonApi.AppOptionApi.AppOptionsStateChanged += AppOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void AppOptionsStateWrapOnStateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        CommonApi.AppOptionApi.AppOptionsStateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}
