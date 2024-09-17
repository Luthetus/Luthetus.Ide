using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Options.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppIconSize : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    private int IconSizeInPixels
    {
        get => AppOptionsService.AppOptionsStateWrap.Value.Options.IconSizeInPixels;
        set
        {
            if (value < AppOptionsState.MINIMUM_ICON_SIZE_IN_PIXELS)
                value = AppOptionsState.MINIMUM_ICON_SIZE_IN_PIXELS;

            AppOptionsService.SetIconSize(value);
        }
    }

    protected override Task OnInitializedAsync()
    {
        AppOptionsService.AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

        return base.OnInitializedAsync();
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