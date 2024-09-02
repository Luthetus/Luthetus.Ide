using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public class CursorDriver
{
	private readonly TextEditorViewModelDisplay _root;

	public CursorDriver(TextEditorViewModelDisplay textEditorViewModelDisplay)
	{
		_root = textEditorViewModelDisplay;
	}

	// Odd public but am middle of thinking
	public TextEditorRenderBatchValidated _renderBatch;

	public void GetRenderFragment(TextEditorRenderBatchValidated renderBatch)
	{
		// Dangerous state can change mid run possible?
		_renderBatch = renderBatch;
		// return CursorStaticRenderFragments.GetRenderFragment(this);
	}

    /// <summary>This property will need to be used when multi-cursor is added.</summary>
    public bool IsFocusTarget => true;

    public readonly Guid _intersectionObserverMapKey = Guid.NewGuid();
    public readonly Throttle _throttleShouldRevealCursor = new(TimeSpan.FromMilliseconds(333));

    public ElementReference? _cursorDisplayElementReference;
    public int _menuShouldGetFocusRequestCount;
    public string _previousGetCursorStyleCss = string.Empty;
    public string _previousGetCaretRowStyleCss = string.Empty;
    public string _previousGetMenuStyleCss = string.Empty;

    public string _previouslyObservedCursorDisplayId = string.Empty;
    public double _leftRelativeToParentInPixels;
        
    public bool GetIncludeContextMenuHelperComponent(TextEditorRenderBatchValidated renderBatchLocal)
    {
    	return renderBatchLocal.ViewModelDisplayOptions.IncludeContextMenuHelperComponent;
    }

	public string GetScrollableContainerId(TextEditorRenderBatchValidated renderBatchLocal)
	{ 
		return renderBatchLocal.ViewModel.BodyElementId;
	}

    public string GetCursorDisplayId(TextEditorRenderBatchValidated renderBatchLocal)
    {
    	return renderBatchLocal.ViewModel.PrimaryCursor.IsPrimaryCursor
	        ? renderBatchLocal?.ViewModel?.PrimaryCursorContentId ?? string.Empty
	        : string.Empty;
    }

    public string BlinkAnimationCssClass => _root.TextEditorService.ViewModelApi.CursorShouldBlink
        ? "luth_te_blink"
        : string.Empty;

    public string GetCursorStyleCss()
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return string.Empty;
    	
        try
        {
            var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

            var leftInPixels = 0d;

            // Tab key column offset
            {
                var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
                    renderBatchLocal.ViewModel.PrimaryCursor.LineIndex,
                    renderBatchLocal.ViewModel.PrimaryCursor.ColumnIndex);

                // 1 of the character width is already accounted for

                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                leftInPixels += extraWidthPerTabKey *
                    tabsOnSameRowBeforeCursor *
                    measurements.CharacterWidth;
            }

            leftInPixels += measurements.CharacterWidth * renderBatchLocal.ViewModel.PrimaryCursor.ColumnIndex;

            var leftInPixelsInvariantCulture = leftInPixels.ToCssValue();
            var left = $"left: {leftInPixelsInvariantCulture}px;";

            var topInPixelsInvariantCulture = (measurements.LineHeight * renderBatchLocal.ViewModel.PrimaryCursor.LineIndex)
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

    public string GetCaretRowStyleCss()
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return string.Empty;
    	
        try
        {
            var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

            var topInPixelsInvariantCulture = (measurements.LineHeight * renderBatchLocal.ViewModel.PrimaryCursor.LineIndex)
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

    public string GetMenuStyleCss()
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return string.Empty;
    	
        try
        {
            var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

            var leftInPixels = 0d;

            // Tab key column offset
            {
                var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
                    renderBatchLocal.ViewModel.PrimaryCursor.LineIndex,
                    renderBatchLocal.ViewModel.PrimaryCursor.ColumnIndex);

                // 1 of the character width is already accounted for
                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                leftInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor *
                    measurements.CharacterWidth;
            }

            leftInPixels += measurements.CharacterWidth * renderBatchLocal.ViewModel.PrimaryCursor.ColumnIndex;

            var leftInPixelsInvariantCulture = leftInPixels.ToCssValue();
            var left = $"left: {leftInPixelsInvariantCulture}px;";

            var topInPixelsInvariantCulture = (measurements.LineHeight * (renderBatchLocal.ViewModel.PrimaryCursor.LineIndex + 1))
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

    public void HandleOnKeyDown()
    {
        // _root.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
    }

    public async Task SetFocusToActiveMenuAsync()
    {
        _menuShouldGetFocusRequestCount++;
        // await InvokeAsync(StateHasChanged);
    }

    public bool TextEditorMenuShouldTakeFocus()
    {
        if (_menuShouldGetFocusRequestCount > 0)
        {
            _menuShouldGetFocusRequestCount = 0;
            return true;
        }

        return false;
    }

    public int GetTabIndex(TextEditorRenderBatchValidated renderBatchLocal)
    {
        if (!IsFocusTarget)
            return -1;

        return renderBatchLocal.ViewModelDisplayOptions.TabIndex;;
    }
}
