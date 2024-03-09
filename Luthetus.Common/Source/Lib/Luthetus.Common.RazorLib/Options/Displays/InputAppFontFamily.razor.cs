using Luthetus.Common.RazorLib.Options.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppFontFamily : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    public string FontFamily
    {
        get => AppOptionsService.AppOptionsStateWrap.Value.Options.FontFamily ?? "unset";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                AppOptionsService.SetFontFamily(null);

            AppOptionsService.SetFontFamily(value.Trim());
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