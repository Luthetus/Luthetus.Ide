using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Htmls.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Luthetus.Common.RazorLib.Dialogs.States;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Displays;

public partial class DialogDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<DialogState, bool> DialogStateIsActiveSelection { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private LuthetusCommonConfig CommonConfig { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter]
    public IDialog Dialog { get; set; } = null!;

    private const int COUNT_OF_CONTROL_BUTTONS = 2;

    private ResizableDisplay? _resizableDisplay;

    private string ElementDimensionsStyleCssString => Dialog.DialogElementDimensions.StyleString;

    private string IsMaximizedStyleCssString => Dialog.DialogIsMaximized
        ? CommonConfig.DialogServiceOptions.IsMaximizedStyleCssString
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
        DialogStateIsActiveSelection.SelectedValueChanged += DialogStateIsActiveSelection_SelectedValueChanged;

        DialogStateIsActiveSelection.Select(dialogState => dialogState.ActiveDialogKey == Dialog.DynamicViewModelKey);

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync(
                "luthetusTextEditor.focusHtmlElementById",
                Dialog.DialogFocusPointHtmlElementId);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void DialogStateIsActiveSelection_SelectedValueChanged(object? sender, bool e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task ReRenderAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task SubscribeMoveHandleAsync()
    {
        _resizableDisplay?.SubscribeToDragEventWithMoveHandle();
    }

    private void ToggleIsMaximized()
    {
        DialogService.SetDialogRecordIsMaximized(
            Dialog.DynamicViewModelKey,
            !Dialog.DialogIsMaximized);
    }

    private void DispatchDisposeDialogRecordAction()
    {
        DialogService.DisposeDialogRecord(Dialog.DynamicViewModelKey);
    }

    private string GetCssClassForDialogStateIsActiveSelection(bool isActive)
    {
        return isActive
            ? "luth_active"
            : string.Empty;
    }

    private void HandleOnFocusIn()
    {
        Dispatcher.Dispatch(new DialogState.SetActiveDialogKeyAction(Dialog.DynamicViewModelKey));
    }

    private void HandleOnMouseDown()
    {
        Dispatcher.Dispatch(new DialogState.SetActiveDialogKeyAction(Dialog.DynamicViewModelKey));
    }

    public void Dispose()
    {
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
        DialogStateIsActiveSelection.SelectedValueChanged += DialogStateIsActiveSelection_SelectedValueChanged;
    }
}