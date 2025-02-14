using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppIconSize : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    private int IconSizeInPixels
    {
        get => CommonApi.AppOptionApi.GetAppOptionsState().Options.IconSizeInPixels;
        set
        {
            if (value < AppOptionsState.MINIMUM_ICON_SIZE_IN_PIXELS)
                value = AppOptionsState.MINIMUM_ICON_SIZE_IN_PIXELS;

            CommonApi.AppOptionApi.SetIconSize(value);
        }
    }

    protected override Task OnInitializedAsync()
    {
        CommonApi.AppOptionApi.AppOptionsStateChanged += AppOptionsStateWrapOnStateChanged;

        return base.OnInitializedAsync();
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