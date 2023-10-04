using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Htmls.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;

namespace Luthetus.Common.RazorLib.Dialogs.Displays;

public partial class DialogDisplay : IDisposable
{
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private LuthetusCommonOptions LuthetusCommonOptions { get; set; } = null!;

    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private const int COUNT_OF_CONTROL_BUTTONS = 2;

    private ResizableDisplay? _resizableDisplay;

    private string ElementDimensionsStyleCssString => DialogRecord.ElementDimensions.StyleString;

    private string IsMaximizedStyleCssString => DialogRecord.IsMaximized
        ? LuthetusCommonOptions.DialogServiceOptions.IsMaximizedStyleCssString
        : string.Empty;

    private string IconSizeInPixelsCssValue =>
        $"{AppOptionsStateWrap.Value.Options.IconSizeInPixels.ToCssValue()}";

    private string DialogTitleCssStyleString =>
        "width: calc(100% -" +
        $" ({COUNT_OF_CONTROL_BUTTONS} * ({IconSizeInPixelsCssValue}px)) -" +
        $" ({COUNT_OF_CONTROL_BUTTONS} * ({HtmlFacts.Button.ButtonPaddingHorizontalTotalInPixelsCssValue})));";

    protected override void OnInitialized()
    {
        AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task ReRenderAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void SubscribeMoveHandle()
    {
        _resizableDisplay?.SubscribeToDragEventWithMoveHandle();
    }

    private void ToggleIsMaximized()
    {
        DialogService.SetDialogRecordIsMaximized(
            DialogRecord.Key,
            !DialogRecord.IsMaximized);
    }

    private void DispatchDisposeDialogRecordAction()
    {
        DialogService.DisposeDialogRecord(DialogRecord.Key);
    }

    public void Dispose()
    {
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}