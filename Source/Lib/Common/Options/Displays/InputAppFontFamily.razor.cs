using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppFontFamily : ComponentBase, IDisposable
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    public string FontFamily
    {
        get => AppOptionsService.GetAppOptionsState().Options.FontFamily ?? "unset";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                AppOptionsService.SetFontFamily(null);

            AppOptionsService.SetFontFamily(value.Trim());
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