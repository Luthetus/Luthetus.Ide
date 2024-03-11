using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Options.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppFontSize : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    public int FontSizeInPixels
    {
        get => AppOptionsService.AppOptionsStateWrap.Value.Options.FontSizeInPixels;
        set
        {
            if (value < AppOptionsState.MINIMUM_FONT_SIZE_IN_PIXELS)
                value = AppOptionsState.MINIMUM_FONT_SIZE_IN_PIXELS;

            AppOptionsService.SetFontSize(value);
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