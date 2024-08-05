using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Htmls.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class CursorDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatchValidated? RenderBatch { get; set; }
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

    private readonly ThrottleAsync _throttleShouldRevealCursor = new(TimeSpan.FromMilliseconds(333));

    private ElementReference? _cursorDisplayElementReference;
    private int _menuShouldGetFocusRequestCount;
    private string _previousGetCursorStyleCss = string.Empty;
    private string _previousGetCaretRowStyleCss = string.Empty;
    private string _previousGetMenuStyleCss = string.Empty;

    private string _previouslyObservedCursorDisplayId = string.Empty;
    private double _leftRelativeToParentInPixels;

    public string CursorDisplayId => Cursor.IsPrimaryCursor
        ? RenderBatch?.ViewModel?.PrimaryCursorContentId ?? string.Empty
        : string.Empty;

    public string CursorStyleCss => GetCursorStyleCss();
    public string CaretRowStyleCss => GetCaretRowStyleCss();
    public string MenuStyleCss => GetMenuStyleCss();

    public string BlinkAnimationCssClass => TextEditorService.ViewModelApi.CursorShouldBlink
        ? "luth_te_blink"
        : string.Empty;

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
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
        // This method is async, so I'll grab the references locally.
        var model = renderBatchLocal.Model;
        var viewModel = renderBatchLocal.ViewModel;
        var options = renderBatchLocal.Options;

        if (!options.UseMonospaceOptimizations)
        {
            var textOffsettingCursor = model.GetTextOffsettingCursor(Cursor).EscapeHtml();

            var guid = Guid.NewGuid();

            var nextLeftRelativeToParentInPixels = await TextEditorService.JsRuntimeTextEditorApi
                .CalculateProportionalLeftOffset(
                    ProportionalFontMeasurementsContainerElementId,
                    $"luth_te_proportional-font-measurement-parent_{viewModel.ViewModelKey.Guid}_cursor_{guid}",
                    $"luth_te_proportional-font-measurement-cursor_{viewModel.ViewModelKey.Guid}_cursor_{guid}",
                    textOffsettingCursor,
                    true)
                .ConfigureAwait(false);

            var previousLeftRelativeToParentInPixels = _leftRelativeToParentInPixels;

            _leftRelativeToParentInPixels = nextLeftRelativeToParentInPixels;

            if ((int)nextLeftRelativeToParentInPixels != (int)previousLeftRelativeToParentInPixels)
                await InvokeAsync(StateHasChanged);
        }

        if (_previouslyObservedCursorDisplayId != CursorDisplayId && IsFocusTarget)
        {
            await TextEditorService.JsRuntimeTextEditorApi
                .InitializeTextEditorCursorIntersectionObserver(
                    _intersectionObserverMapKey.ToString(),
                    DotNetObjectReference.Create(this),
                    ScrollableContainerId,
                    CursorDisplayId)
                .ConfigureAwait(false);

            _previouslyObservedCursorDisplayId = CursorDisplayId;
        }

        if (viewModel.UnsafeState.ShouldRevealCursor)
        {
            viewModel.UnsafeState.ShouldRevealCursor = false;

            if (!renderBatchLocal.ViewModel.UnsafeState.CursorIsIntersecting)
            {
                await _throttleShouldRevealCursor.PushEvent(_ =>
                {
                    // I think after removing the throttle, that this is an infinite loop on WASM,
                    // i.e. holding down ArrowRight
                    TextEditorService.PostUnique(
                        nameof(_throttleShouldRevealCursor),
                        editContext =>
                        {
							try
							{
								var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.ResourceUri);
				            	var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.ViewModelKey);
				
								if (modelModifier is null || viewModelModifier is null)
									return Task.CompletedTask;
							
	                            var cursorPositionIndex = renderBatchLocal.Model.GetPositionIndex(Cursor);
	
	                            var cursorTextSpan = new TextEditorTextSpan(
	                                cursorPositionIndex,
	                                cursorPositionIndex + 1,
	                                0,
	                                renderBatchLocal.Model.ResourceUri,
	                                renderBatchLocal.Model.GetAllText());
	
	                            TextEditorService.ViewModelApi.ScrollIntoView(
                            		editContext,
							        modelModifier,
							        viewModelModifier,
							        cursorTextSpan);
							 }
							 catch (LuthetusTextEditorException)
							 {
							     // Eat this specific exception
							 }
                        
                        	return Task.CompletedTask;
                        });
					return Task.CompletedTask;
                }).ConfigureAwait(false);
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable]
    public Task OnCursorPassedIntersectionThresholdAsync(bool cursorIsIntersecting)
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
        renderBatchLocal.ViewModel.UnsafeState.CursorIsIntersecting = cursorIsIntersecting;
        return Task.CompletedTask;
    }

    private string GetCursorStyleCss()
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
        try
        {
            var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

            var leftInPixels = 0d;

            // Tab key column offset
            {
                var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
                    Cursor.LineIndex,
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

            var topInPixelsInvariantCulture = (measurements.LineHeight * Cursor.LineIndex)
                .ToCssValue();

            var top = $"top: {topInPixelsInvariantCulture}px;";

            var heightInPixelsInvariantCulture = measurements.LineHeight.ToCssValue();
            var height = $"height: {heightInPixelsInvariantCulture}px;";

            var widthInPixelsInvariantCulture = renderBatchLocal.Options.CursorWidthInPixels.ToCssValue();
            var width = $"width: {widthInPixelsInvariantCulture}px;";

            var keymapStyling = ((ITextEditorKeymap)renderBatchLocal.Options.Keymap).GetCursorCssStyleString(
                renderBatchLocal.Model,
                renderBatchLocal.ViewModel,
                renderBatchLocal.Options);
            
            // This feels a bit hacky, exceptions are happening because the UI isn't accessing
            // the text editor in a thread safe way.
            //
            // When an exception does occur though, the cursor should receive a 'text editor changed'
            // event and re-render anyhow however.
            // 
            // So store the result of this method incase an exception occurs in future invocations,
            // to keep the cursor on screen while the state works itself out.
            return _previousGetCursorStyleCss = $"{left} {top} {height} {width} {keymapStyling}";
        }
        catch (LuthetusTextEditorException)
        {
            return _previousGetCursorStyleCss;
        }
    }

    private string GetCaretRowStyleCss()
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
        try
        {
            var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

            var topInPixelsInvariantCulture = (measurements.LineHeight * Cursor.LineIndex)
                .ToCssValue();

            var top = $"top: {topInPixelsInvariantCulture}px;";

            var heightInPixelsInvariantCulture = measurements.LineHeight.ToCssValue();
            var height = $"height: {heightInPixelsInvariantCulture}px;";

            var widthOfBodyInPixelsInvariantCulture =
                (renderBatchLocal.Model.MostCharactersOnASingleLineTuple.lineLength * measurements.CharacterWidth)
                .ToCssValue();

            var width = $"width: {widthOfBodyInPixelsInvariantCulture}px;";

            // This feels a bit hacky, exceptions are happening because the UI isn't accessing
            // the text editor in a thread safe way.
            //
            // When an exception does occur though, the cursor should receive a 'text editor changed'
            // event and re-render anyhow however.
            // 
            // So store the result of this method incase an exception occurs in future invocations,
            // to keep the cursor on screen while the state works itself out.
            return _previousGetCaretRowStyleCss = $"{top} {width} {height}";
        }
        catch (LuthetusTextEditorException)
        {
            return _previousGetCaretRowStyleCss;
        }
    }

    private string GetMenuStyleCss()
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
        try
        {
            var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

            var leftInPixels = 0d;

            // Tab key column offset
            {
                var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
                    Cursor.LineIndex,
                    Cursor.ColumnIndex);

                // 1 of the character width is already accounted for
                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                leftInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor *
                    measurements.CharacterWidth;
            }

            leftInPixels += measurements.CharacterWidth * Cursor.ColumnIndex;

            var leftInPixelsInvariantCulture = leftInPixels.ToCssValue();
            var left = $"left: {leftInPixelsInvariantCulture}px;";

            var topInPixelsInvariantCulture = (measurements.LineHeight * (Cursor.LineIndex + 1))
                .ToCssValue();

            // Top is 1 row further than the cursor so it does not cover text at cursor position.
            var top = $"top: {topInPixelsInvariantCulture}px;";

            var minWidthInPixelsInvariantCulture = (measurements.CharacterWidth * 16).ToCssValue();
            var minWidth = $"min-Width: {minWidthInPixelsInvariantCulture}px;";

            var minHeightInPixelsInvariantCulture = (measurements.LineHeight * 4).ToCssValue();
            var minHeight = $"min-height: {minHeightInPixelsInvariantCulture}px;";

            // This feels a bit hacky, exceptions are happening because the UI isn't accessing
            // the text editor in a thread safe way.
            //
            // When an exception does occur though, the cursor should receive a 'text editor changed'
            // event and re-render anyhow however.
            // 
            // So store the result of this method incase an exception occurs in future invocations,
            // to keep the cursor on screen while the state works itself out.
            return _previousGetMenuStyleCss = $"{left} {top} {minWidth} {minHeight}";
        }
        catch (LuthetusTextEditorException)
        {
            return _previousGetMenuStyleCss;
        }
    }

    public async Task FocusAsync()
    {
        try
        {
            if (_cursorDisplayElementReference is not null)
            {
                await _cursorDisplayElementReference.Value
                    .FocusAsync(preventScroll: true)
                    .ConfigureAwait(false);
            }
        }
        catch (Exception)
        {
            // 2023-04-18: The app has had a bug where it "freezes" and must be restarted.
            //             This bug is seemingly happening randomly. I have a suspicion
            //             that there are race-condition exceptions occurring with "FocusAsync"
            //             on an ElementReference.
        }
    }

    private void HandleOnKeyDown()
    {
        TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
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
            _ = Task.Run(async () =>
            {
                try
                {
                    await TextEditorService.JsRuntimeTextEditorApi
                        .DisposeTextEditorCursorIntersectionObserver(
                            CancellationToken.None,
                            _intersectionObserverMapKey.ToString())
                        .ConfigureAwait(false);
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