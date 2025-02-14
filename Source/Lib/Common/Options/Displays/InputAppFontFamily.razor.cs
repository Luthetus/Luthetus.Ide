using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppFontFamily : ComponentBase, IDisposable
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    public string FontFamily
    {
        get => CommonApi.AppOptionApi.GetAppOptionsState().Options.FontFamily ?? "unset";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
				CommonApi.AppOptionApi.SetFontFamily(null);

			CommonApi.AppOptionApi.SetFontFamily(value.Trim());
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