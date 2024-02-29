using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Htmls.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class CursorDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;
    [CascadingParameter(Name = "ProportionalFontMeasurementsContainerElementId")]
    public string ProportionalFontMeasurementsContainerElementId { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorCursor Cursor { get; set; } = null!;
    [Parameter, EditorRequired]
    public string ScrollableContainerId { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsFocusTarget { get; set; }
    [Parameter, EditorRequired]
    public int TabIndex { get; set; }

    [Parameter]
    public bool IncludeContextMenuHelperComponent { get; set; }
    [Parameter]
    public RenderFragment OnContextMenuRenderFragment { get; set; } = null!;
    [Parameter]
    public RenderFragment AutoCompleteMenuRenderFragment { get; set; } = null!;

    private readonly Guid _intersectionObserverMapKey = Guid.NewGuid();
    private readonly IThrottle _throttleShouldRevealCursor = new Throttle(TimeSpan.FromMilliseconds(50));

    private ElementReference? _cursorDisplayElementReference;
    private TextEditorMenuKind _menuKind;
    private int _menuShouldGetFocusRequestCount;

    private string _previouslyObservedCursorDisplayId = string.Empty;
    private double _leftRelativeToParentInPixels;

    public string CursorDisplayId => Cursor.IsPrimaryCursor
        ? RenderBatch.ViewModel!.PrimaryCursorContentId
        : string.Empty;

    public string CursorStyleCss => GetCursorStyleCss();
    public string CaretRowStyleCss => GetCaretRowStyleCss();
    public string MenuStyleCss => GetMenuStyleCss();

    public string BlinkAnimationCssClass => TextEditorService.ViewModelApi.CursorShouldBlink
        ? "luth_te_blink"
        : string.Empty;

    public TextEditorMenuKind MenuKind => _menuKind;

    protected override void OnInitialized()
    {
        TextEditorService.ViewModelApi.CursorShouldBlinkChanged += ViewModel_CursorShouldBlinkChanged;

        base.OnInitialized();
    }

    private async void ViewModel_CursorShouldBlinkChanged()
    {
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // This method is async, so I'll grab the references locally.
        var model = RenderBatch.Model!;
        var viewModel = RenderBatch.ViewModel!;
        var options = RenderBatch.Options!;

        if (!options.UseMonospaceOptimizations)
        {
            var textOffsettingCursor = model.GetTextOffsettingCursor(Cursor).EscapeHtml();

            var guid = Guid.NewGuid();

            var nextLeftRelativeToParentInPixels = await JsRuntime.InvokeAsync<double>(
                    "luthetusTextEditor.calculateProportionalLeftOffset",
                    ProportionalFontMeasurementsContainerElementId,
                    $"luth_te_proportional-font-measurement-parent_{viewModel.ViewModelKey.Guid}_cursor_{guid}",
                    $"luth_te_proportional-font-measurement-cursor_{viewModel.ViewModelKey.Guid}_cursor_{guid}",
                    textOffsettingCursor,
                    true)
                .ConfigureAwait(false);

            var previousLeftRelativeToParentInPixels = _leftRelativeToParentInPixels;

            _leftRelativeToParentInPixels = nextLeftRelativeToParentInPixels;

            if ((int)nextLeftRelativeToParentInPixels != (int)previousLeftRelativeToParentInPixels)
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }

        if (_previouslyObservedCursorDisplayId != CursorDisplayId && IsFocusTarget)
        {
            await JsRuntime.InvokeVoidAsync(
                    "luthetusTextEditor.initializeTextEditorCursorIntersectionObserver",
                    _intersectionObserverMapKey.ToString(),
                    DotNetObjectReference.Create(this),
                    ScrollableContainerId,
                    CursorDisplayId)
                .ConfigureAwait(false);

            _previouslyObservedCursorDisplayId = CursorDisplayId;
        }

        if (Cursor.ShouldRevealCursor)
        {
            Cursor.ShouldRevealCursor = false;

            if (!Cursor.IsIntersecting)
            {
                _throttleShouldRevealCursor.FireAndForget(async _ =>
                {
                    await JsRuntime.InvokeVoidAsync(
                            "luthetusTextEditor.scrollElementIntoView",
                            CursorDisplayId)
                        .ConfigureAwait(false);
                });
            }
        }

        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
    }

    [JSInvokable]
    public Task OnCursorPassedIntersectionThresholdAsync(bool cursorIsIntersecting)
    {
        Cursor.IsIntersecting = cursorIsIntersecting;
        return Task.CompletedTask;
    }

    private string GetCursorStyleCss()
    {
        var measurements = RenderBatch.ViewModel!.VirtualizationResult.CharAndRowMeasurements;

        var leftInPixels = 0d;

        // Tab key column offset
        {
            var tabsOnSameRowBeforeCursor = RenderBatch.Model!.GetTabsCountOnSameRowBeforeCursor(
                Cursor.RowIndex,
                Cursor.ColumnIndex);

            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            leftInPixels += extraWidthPerTabKey *
                tabsOnSameRowBeforeCursor *
                measurements.CharacterWidth;
        }

        leftInPixels += measurements.CharacterWidth * Cursor.ColumnIndex;

        var leftInPixelsInvariantCulture = leftInPixels.ToCssValue();
        var left = $"left: {leftInPixelsInvariantCulture}px;";

        var topInPixelsInvariantCulture = (measurements.RowHeight * Cursor.RowIndex)
            .ToCssValue();

        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = measurements.RowHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var widthInPixelsInvariantCulture = RenderBatch.Options!.CursorWidthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        var keymapStyling = ((ITextEditorKeymap)RenderBatch.Options!.Keymap!).GetCursorCssStyleString(
            RenderBatch.Model!,
            RenderBatch.ViewModel!,
            RenderBatch.Options!);

        return $"{left} {top} {height} {width} {keymapStyling}";
    }

    private string GetCaretRowStyleCss()
    {
        var measurements = RenderBatch.ViewModel!.VirtualizationResult.CharAndRowMeasurements;

        var topInPixelsInvariantCulture = (measurements.RowHeight * Cursor.RowIndex)
            .ToCssValue();

        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = measurements.RowHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var widthOfBodyInPixelsInvariantCulture =
            (RenderBatch.Model!.MostCharactersOnASingleRowTuple.rowLength * measurements.CharacterWidth)
            .ToCssValue();

        var width = $"width: {widthOfBodyInPixelsInvariantCulture}px;";

        return $"{top} {width} {height}";
    }

    private string GetMenuStyleCss()
    {
        var measurements = RenderBatch.ViewModel!.VirtualizationResult.CharAndRowMeasurements;

        var leftInPixels = 0d;

        // Tab key column offset
        {
            var tabsOnSameRowBeforeCursor = RenderBatch.Model!.GetTabsCountOnSameRowBeforeCursor(
                Cursor.RowIndex,
                Cursor.ColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            leftInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor *
                measurements.CharacterWidth;
        }

        leftInPixels += measurements.CharacterWidth * Cursor.ColumnIndex;

        var leftInPixelsInvariantCulture = leftInPixels.ToCssValue();
        var left = $"left: {leftInPixelsInvariantCulture}px;";

        var topInPixelsInvariantCulture = (measurements.RowHeight * (Cursor.RowIndex + 1))
            .ToCssValue();

        // Top is 1 row further than the cursor so it does not cover text at cursor position.
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var minWidthInPixelsInvariantCulture = (measurements.CharacterWidth * 16).ToCssValue();
        var minWidth = $"min-Width: {minWidthInPixelsInvariantCulture}px;";

        var minHeightInPixelsInvariantCulture = (measurements.RowHeight * 4).ToCssValue();
        var minHeight = $"min-height: {minHeightInPixelsInvariantCulture}px;";

        return $"{left} {top} {minWidth} {minHeight}";
    }

    public async Task FocusAsync()
    {
        try
        {
            if (_cursorDisplayElementReference is not null)
                await _cursorDisplayElementReference.Value.FocusAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
            //             This bug is seemingly happening randomly. I have a suspicion
            //             that there are race-condition exceptions occurring with "FocusAsync"
            //             on an ElementReference.
        }
    }

    public void PauseBlinkAnimation()
    {
        TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
    }

    private void HandleOnKeyDown()
    {
        PauseBlinkAnimation();
    }

    public async Task SetShouldDisplayMenuAsync(TextEditorMenuKind textEditorMenuKind, bool shouldFocusCursor = true)
    {
        // Clear the counter of requests for the Menu to take focus
        _ = TextEditorMenuShouldTakeFocus();

        _menuKind = textEditorMenuKind;

        await InvokeAsync(StateHasChanged).ConfigureAwait(false);

        if (shouldFocusCursor && _menuKind == TextEditorMenuKind.None)
            await FocusAsync().ConfigureAwait(false);
    }

    public async Task SetFocusToActiveMenuAsync()
    {
        _menuShouldGetFocusRequestCount++;
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }

    private bool TextEditorMenuShouldTakeFocus()
    {
        if (_menuShouldGetFocusRequestCount > 0)
        {
            _menuShouldGetFocusRequestCount = 0;
            return true;
        }

        return false;
    }

    private int GetTabIndex()
    {
        if (!IsFocusTarget)
            return -1;

        return TabIndex;
    }

    public void Dispose()
    {
        TextEditorService.ViewModelApi.CursorShouldBlinkChanged -= ViewModel_CursorShouldBlinkChanged;

        if (IsFocusTarget)
        {
            // ICommonBackgroundTaskQueue does not work well here because
            // this Task does not need to be tracked.
            _ = Task.Run(async () =>
            {
                try
                {
                    await JsRuntime.InvokeVoidAsync(
                            "luthetusTextEditor.disposeTextEditorCursorIntersectionObserver",
                            CancellationToken.None,
                            _intersectionObserverMapKey.ToString())
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }, CancellationToken.None).ConfigureAwait(false);
        }
    }
}