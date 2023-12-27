using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Htmls.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

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
        await InvokeAsync(StateHasChanged);
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
                true);

            var previousLeftRelativeToParentInPixels = _leftRelativeToParentInPixels;

            _leftRelativeToParentInPixels = nextLeftRelativeToParentInPixels;

            if ((int)nextLeftRelativeToParentInPixels != (int)previousLeftRelativeToParentInPixels)
                await InvokeAsync(StateHasChanged);
        }

        if (_previouslyObservedCursorDisplayId != CursorDisplayId && IsFocusTarget)
        {
            await JsRuntime.InvokeVoidAsync(
                "luthetusTextEditor.initializeTextEditorCursorIntersectionObserver",
                _intersectionObserverMapKey.ToString(),
                DotNetObjectReference.Create(this),
                ScrollableContainerId,
                CursorDisplayId);

            _previouslyObservedCursorDisplayId = CursorDisplayId;
        }

        // TODO: This code block seems to bug out 'enter' keystrokes...
        // ...I'm commenting it out for now, but I am still investigating the issue (2023-12-26)
        //
        // var rowIndex = Cursor.RowIndex;
        //
        // Ensure cursor stays within the row count index range
        // if (rowIndex > model.RowCount - 1)
        //    rowIndex = model.RowCount - 1;
        //
        // Ensure cursor stays within the column count index range for the current row
        // {
        //    var columnIndex = Cursor.ColumnIndex;
        //
        //    var rowLength = model.GetLengthOfRow(rowIndex);
        //
        //    if (columnIndex > rowLength)
        //        columnIndex = rowLength - 1;
        //
        //    rowIndex = Math.Max(0, rowIndex);
        //    columnIndex = Math.Max(0, columnIndex);
        //
        //    var modelResourceUri = RenderBatch.Model!.ResourceUri;
        //    var viewModelKey = RenderBatch.ViewModel!.ViewModelKey;
        //
        //    if (Cursor.RowIndex != rowIndex || Cursor.ColumnIndex != columnIndex)
        //    {
        //        TextEditorService.Post(editContext =>
        //        {
        //            var modelModifier = editContext.GetModelModifier(modelResourceUri);
        //            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
        //
        //            if (modelModifier is null || viewModelModifier is null)
        //                return Task.CompletedTask;
        //
        //            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
        //            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
        //
        //            if (primaryCursorModifier is null)
        //                return Task.CompletedTask;
        //
        //            var columnIndex = primaryCursorModifier.ColumnIndex;
        //
        //            var rowLength = modelModifier.GetLengthOfRow(rowIndex);
        //
        //            if (columnIndex > rowLength)
        //                columnIndex = rowLength - 1;
        //
        //            rowIndex = Math.Max(0, rowIndex);
        //            columnIndex = Math.Max(0, columnIndex);
        //
        //            primaryCursorModifier.RowIndex = rowIndex;
        //            primaryCursorModifier.ColumnIndex = columnIndex;
        //
        //            return Task.CompletedTask;
        //        });
        //    }
        // }

        if (Cursor.ShouldRevealCursor)
        {
            Cursor.ShouldRevealCursor = false;

            if (!Cursor.IsIntersecting)
            {
                await JsRuntime.InvokeVoidAsync("luthetusTextEditor.scrollElementIntoView",
                    CursorDisplayId);
            }
        }

        await base.OnAfterRenderAsync(firstRender);
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
                await _cursorDisplayElementReference.Value.FocusAsync();
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

        await InvokeAsync(StateHasChanged);

        if (shouldFocusCursor && _menuKind == TextEditorMenuKind.None)
            await FocusAsync();
    }

    public async Task SetFocusToActiveMenuAsync()
    {
        _menuShouldGetFocusRequestCount++;
        await InvokeAsync(StateHasChanged);
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
                    await JsRuntime.InvokeVoidAsync("luthetusTextEditor.disposeTextEditorCursorIntersectionObserver",
                        CancellationToken.None,
                        _intersectionObserverMapKey.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }, CancellationToken.None);
        }
    }
}