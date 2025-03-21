using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Htmls.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Displays;

public partial class DialogDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private LuthetusCommonConfig CommonConfig { get; set; } = null!;
    [Inject]
    private CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; } = null!;

    [Parameter, EditorRequired]
    public IDialog Dialog { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<IDialog, Task> OnFocusInFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<IDialog, Task> OnFocusOutFunc { get; set; } = null!;

	private const int COUNT_OF_CONTROL_BUTTONS = 2;

    private ResizableDisplay? _resizableDisplay;

    private string ElementDimensionsStyleCssString => Dialog.DialogElementDimensions.StyleString;

    private string IsMaximizedStyleCssString => Dialog.DialogIsMaximized
        ? CommonConfig.IsMaximizedStyleCssString
        : string.Empty;

    private string IconSizeInPixelsCssValue =>
        $"{AppOptionsService.GetAppOptionsState().Options.IconSizeInPixels.ToCssValue()}";

    private string DialogTitleCssStyleString =>
        "width: calc(100% -" +
        $" ({COUNT_OF_CONTROL_BUTTONS} * ({IconSizeInPixelsCssValue}px)) -" +
        $" ({COUNT_OF_CONTROL_BUTTONS} * ({HtmlFacts.Button.ButtonPaddingHorizontalTotalInPixelsCssValue})));";

    protected override void OnInitialized()
    {
        AppOptionsService.AppOptionsStateChanged += AppOptionsStateWrapOnStateChanged;
        DialogService.ActiveDialogKeyChanged += OnActiveDialogKeyChanged;

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await CommonBackgroundTaskApi.JsRuntimeCommonApi
                .FocusHtmlElementById(Dialog.DialogFocusPointHtmlElementId)
                .ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async void OnActiveDialogKeyChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void AppOptionsStateWrapOnStateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    private Task ReRenderAsync()
    {
        return InvokeAsync(StateHasChanged);
    }

    private Task SubscribeMoveHandleAsync()
    {
        _resizableDisplay?.SubscribeToDragEventWithMoveHandle();
        return Task.CompletedTask;
    }

    private void ToggleIsMaximized()
    {
        DialogService.ReduceSetIsMaximizedAction(
            Dialog.DynamicViewModelKey,
            !Dialog.DialogIsMaximized);
    }

    private async Task DispatchDisposeDialogRecordAction()
    {
        DialogService.ReduceDisposeAction(Dialog.DynamicViewModelKey);
        
        await CommonBackgroundTaskApi.JsRuntimeCommonApi
	        .FocusHtmlElementById(Dialog.SetFocusOnCloseElementId
	        	 ?? IDynamicViewModel.DefaultSetFocusOnCloseElementId)
	        .ConfigureAwait(false);
    }

    private string GetCssClassForDialogStateIsActiveSelection(bool isActive)
    {
        return isActive
            ? "luth_active"
            : string.Empty;
    }

    private Task HandleOnFocusIn()
    {
        DialogService.ReduceSetActiveDialogKeyAction(Dialog.DynamicViewModelKey);
        return OnFocusInFunc.Invoke(Dialog);
    }
    
	private Task HandleOnFocusOut()
    {
    	return OnFocusOutFunc.Invoke(Dialog);
    }

    private void HandleOnMouseDown()
    {
        DialogService.ReduceSetActiveDialogKeyAction(Dialog.DynamicViewModelKey);
    }

    public void Dispose()
    {
        AppOptionsService.AppOptionsStateChanged -= AppOptionsStateWrapOnStateChanged;
        DialogService.ActiveDialogKeyChanged -= OnActiveDialogKeyChanged;
    }
}