using System.Diagnostics;
using System.Text;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// Everytime one renders a unique <see cref="Luthetus.TextEditor.RazorLib.TextEditors.Displays.TextEditorViewModelSlimDisplay"/>,
/// a unique identifier for the HTML elements is created.
///
/// That unique identifier, and other data, is on this class.
///
/// This class is a "ui event broker" in order to avoid unnecessary invocations of
/// <see cref="ITextEditorService.Post"/.
///
/// For example, all <see cref="TextEditorViewModel"/> have a <see cref="TextEditorViewModel.TooltipViewModel"/>.
/// A sort of 'on mouse over' like event might set the current <see cref="TextEditorViewModel.TooltipViewModel"/>.
/// But, the event logic overhead is not handled by the <see cref="TextEditorViewModel"/>, but instead
/// this class, then the valid events are passed along to result in the <see cref="TextEditorViewModel.TooltipViewModel"/>
/// being set.
/// </summary>
public sealed class TextEditorComponentData
{
	private readonly Throttle _throttleApplySyntaxHighlighting = new(TimeSpan.FromMilliseconds(500));

	public static TimeSpan ThrottleDelayDefault { get; } = TimeSpan.FromMilliseconds(60);
    public static TimeSpan OnMouseOutTooltipDelay { get; } = TimeSpan.FromMilliseconds(1_000);
    public static TimeSpan MouseStoppedMovingDelay { get; } = TimeSpan.FromMilliseconds(400);

	public TextEditorComponentData(
		Guid textEditorHtmlElementId,
		ViewModelDisplayOptions viewModelDisplayOptions,
		TextEditorOptions options,
		TextEditorViewModelSlimDisplay textEditorViewModelSlimDisplay,
		IDropdownService dropdownService,
		IClipboardService clipboardService,
		ICommonComponentRenderers commonComponentRenderers,
		INotificationService notificationService,
		TextEditorService textEditorService,
		ILuthetusTextEditorComponentRenderers textEditorComponentRenderers,
		IFindAllService findAllService,
		IEnvironmentProvider environmentProvider,
		IFileSystemProvider fileSystemProvider,
		IServiceProvider serviceProvider)
	{
		TextEditorHtmlElementId = textEditorHtmlElementId;
		ViewModelDisplayOptions = viewModelDisplayOptions;
		Options = options;
		TextEditorViewModelSlimDisplay = textEditorViewModelSlimDisplay;
		DropdownService = dropdownService;
		ClipboardService = clipboardService;
		CommonComponentRenderers = commonComponentRenderers;
		NotificationService = notificationService;
		TextEditorService = textEditorService;
		TextEditorComponentRenderers = textEditorComponentRenderers;
		FindAllService = findAllService;
		EnvironmentProvider = environmentProvider;
		FileSystemProvider = fileSystemProvider;
		ServiceProvider = serviceProvider;
		
		ComponentDataKey = new Key<TextEditorComponentData>(TextEditorHtmlElementId);
		
		RowSectionElementId = $"luth_te_text-editor-body_{TextEditorHtmlElementId}";
		PrimaryCursorContentId = $"luth_te_text-editor-content_{TextEditorHtmlElementId}_primary-cursor";
		GutterElementId = $"luth_te_text-editor-gutter_{TextEditorHtmlElementId}";
		FindOverlayId = $"luth_te_find-overlay_{TextEditorHtmlElementId}";
	}
	
	public string? inlineUiWidthStyleCssString;
	
	public bool cursorIsOnHiddenLine = false;

	// Active is the one given to the UI after the current was validated and found to be valid.
    public TextEditorRenderBatch _renderBatch;
    
    public int shouldScroll;
    
	public string RowSectionElementId { get; }
	public string PrimaryCursorContentId { get; }
	public string GutterElementId { get; }
	public string FindOverlayId { get; }

	/// <summary>
	/// This property contains the global options, with an extra step of overriding any specified options
	/// for a specific text editor. The current implementation of this is quite hacky and not obvious
	/// when reading the code.
	/// </summary>
	public TextEditorOptions Options { get; init; }

	public Guid TextEditorHtmlElementId { get; }
	public Key<TextEditorComponentData> ComponentDataKey { get; }
	public ViewModelDisplayOptions ViewModelDisplayOptions { get; }
	public TextEditorViewModelSlimDisplay TextEditorViewModelSlimDisplay { get; }
	public IDropdownService DropdownService { get; }
	public IClipboardService ClipboardService { get; }
	public ICommonComponentRenderers CommonComponentRenderers { get; }
	public INotificationService NotificationService { get; }
	public TextEditorService TextEditorService { get; }
	public ILuthetusTextEditorComponentRenderers TextEditorComponentRenderers { get; }
	public IFindAllService FindAllService { get; }
	public IEnvironmentProvider EnvironmentProvider { get; }
	public IFileSystemProvider FileSystemProvider { get; }
	public IServiceProvider ServiceProvider { get; }
	public Task MouseStoppedMovingTask { get; set; } = Task.CompletedTask;
    public Task MouseNoLongerOverTooltipTask { get; set; } = Task.CompletedTask;
    public CancellationTokenSource MouseNoLongerOverTooltipCancellationTokenSource { get; set; } = new();
    public CancellationTokenSource MouseStoppedMovingCancellationTokenSource { get; set; } = new();
    
    public long MouseMovedTimestamp { get; private set; }

    /// <summary>
	/// This accounts for one who might hold down Left Mouse Button from outside the TextEditorDisplay's content div
	/// then move their mouse over the content div while holding the Left Mouse Button down.
	/// </summary>
    public bool ThinksLeftMouseButtonIsDown { get; set; }
    
    public bool MenuShouldTakeFocus { get; set; }
    
    public int useLowerBoundInclusiveLineIndex;
    public int useUpperBoundExclusiveLineIndex;
    public (int Position_LowerInclusiveIndex, int Position_UpperExclusiveIndex) selectionBoundsInPositionIndexUnits;
    
    public List<(string CssClassString, int StartInclusiveIndex, int EndExclusiveIndex)> firstPresentationLayerGroupList = new();
	public List<(string PresentationCssClass, string PresentationCssStyle)> firstPresentationLayerTextSpanList = new();
	
    public List<(string CssClassString, int StartInclusiveIndex, int EndExclusiveIndex)> lastPresentationLayerGroupList = new();
	public List<(string PresentationCssClass, string PresentationCssStyle)> lastPresentationLayerTextSpanList = new();
	
	public List<string> inlineUiStyleList = new();
    
    public List<string> SelectionStyleList = new List<string>();
    
    private List<TextEditorTextSpan> _virtualizedTextSpanList = new();
    private List<TextEditorTextSpan> _outTextSpansList = new();

	public void ThrottleApplySyntaxHighlighting(TextEditorModel modelModifier)
    {
        _throttleApplySyntaxHighlighting.Run(_ =>
        {
            modelModifier.PersistentState.CompilerService.ResourceWasModified(modelModifier.PersistentState.ResourceUri, Array.Empty<TextEditorTextSpan>());
			return Task.CompletedTask;
        });
    }

	public Task ContinueRenderingTooltipAsync()
    {
        MouseNoLongerOverTooltipCancellationTokenSource.Cancel();
        MouseNoLongerOverTooltipCancellationTokenSource = new();

        return Task.CompletedTask;
    }
    
    public void OnMouseMoved()
    {
    	MouseMovedTimestamp = Stopwatch.GetTimestamp();
    }
    
    public double _previousGutterWidthInPixels = 0;
	public double _previousLineHeightInPixels = 0;
	
	public double _previousTextEditorHeightInPixels = 0;
	public double _previousScrollHeightInPixels = 0;
	public double _previousScrollTopInPixels = 0;
	public double _previousTextEditorWidthInPixels = 0;
	public double _previousScrollWidthInPixels = 0;
	public double _previousScrollLeftInPixels = 0;
	
	public string _gutterPaddingStyleCssString;
    public string _gutterWidthStyleCssString;
    
    public string _scrollbarSectionLeftCssString;
    
    /// <summary>
    /// Each individual line number is a separate "gutter".
    /// Therefore, the UI in a loop will use a StringBuilder to .Append(...)
    /// - _lineHeightStyleCssString
    /// - _gutterWidthStyleCssString
    /// - _gutterPaddingStyleCssString
    ///
    /// Due to this occuring within a loop, it is presumed that
    /// pre-calculating all 3 strings together would be best.
    ///
    /// Issue with this: anytime any of the variables change that are used
    /// in this calculation, you need to re-calculate this string field,
    /// and it is a D.R.Y. code / omnipotent knowledge
    /// that this needs re-calculated nightmare.
    /// </summary>
    public string _gutterHeightWidthPaddingStyleCssString;
    
    public string _lineHeightStyleCssString;
    
    public string _previous_VERTICAL_GetSliderVerticalStyleCss_Result;
    public string _previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result;
    public string _previous_HORIZONTAL_GetScrollbarHorizontalStyleCss_Result;
    
    public string _bodyStyle = $"width: 100%; left: 0;";
    
    public string _scrollbarSizeInPixelsCssValue;
    
    public bool _previousIncludeHeader;
    public bool _previousIncludeFooter;
    public string _previousGetHeightCssStyleResult = "height: calc(100%);";
    
    public string _verticalVirtualizationBoundaryStyleCssString = "height: 0px;";
	public string _horizontalVirtualizationBoundaryStyleCssString = "width: 0px;";
	
	public double _previousTotalWidth;
	public double _previousTotalHeight;
    
    public string _personalWrapperCssClass;
    public string _personalWrapperCssStyle;
    
    public string _previousGetCursorStyleCss = string.Empty;
    public string _previousGetCaretRowStyleCss = string.Empty;
    
    public string _blinkAnimationCssClassOn;
    public string _blinkAnimationCssClassOff;
    
    public double valueTooltipRelativeX;
    public double valueTooltipRelativeY;
	
	public string tooltipRelativeX = string.Empty;
	public string tooltipRelativeY = string.Empty;
	
	// _ = "luth_te_text-editor-cursor " + BlinkAnimationCssClass + " " + _activeRenderBatch.Options.Keymap.GetCursorCssClassString();
	public string BlinkAnimationCssClass => TextEditorService.ViewModelApi.CursorShouldBlink
        ? _blinkAnimationCssClassOn
        : _blinkAnimationCssClassOff;

	public string WrapperCssClass { get; private set; }
    public string WrapperCssStyle { get; private set; }
    
    /// <summary>
    /// Share this StringBuilder when used for rendering and no other function is currently using it.
    /// (i.e.: only use this for methods that were invoked from the .razor file)
    /// </summary>
    public StringBuilder _uiStringBuilder = new();
    
    public string GetGutterStyleCss(string topCssValue)
    {
    	_uiStringBuilder.Clear();
    
    	_uiStringBuilder.Append("top: ");
        _uiStringBuilder.Append(topCssValue);
        _uiStringBuilder.Append("px;");
    
        _uiStringBuilder.Append(_gutterHeightWidthPaddingStyleCssString);

        return _uiStringBuilder.ToString();
    }
    
    public string GetGutterStyleCssImaginary()
    {
    	int lastIndex;
					
		if (_renderBatch.ViewModel.VirtualizationResult.EntryList.Count > 0)
		{
			lastIndex = _renderBatch.ViewModel.VirtualizationResult.EntryList.Last().LineIndex;
		}
		else
		{
			lastIndex = -1;
		}
		
		_uiStringBuilder.Clear();
		
		if (lastIndex == -1)
		{
			int topValue = 0;
	        _uiStringBuilder.Append("top: ");
	        _uiStringBuilder.Append(topValue);
	        _uiStringBuilder.Append("px;");
		}
    	else
    	{
    		var lastLineCacheEntry = LineIndexCacheEntryMap[lastIndex];
	
	        var topInPixelsInvariantCulture =
	        	((lastIndex + 1 - lastLineCacheEntry.HiddenLineCount) * _renderBatch.ViewModel.CharAndLineMeasurements.LineHeight)
	        	.ToCssValue();
	        	
	        _uiStringBuilder.Append("top: ");
	        _uiStringBuilder.Append(topInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
    	}
    	
        _uiStringBuilder.Append(_gutterHeightWidthPaddingStyleCssString);

        return _uiStringBuilder.ToString();
    }

    public string GetGutterSectionStyleCss()
    {
        return _gutterWidthStyleCssString;
    }
    
    /* RowSection.razor Open */
    public string RowSection_GetRowStyleCss(int lineIndex)
    {
    	_uiStringBuilder.Clear();
    
        _uiStringBuilder.Append("top: ");
        _uiStringBuilder.Append(LineIndexCacheEntryMap[lineIndex].TopCssValue);
        _uiStringBuilder.Append("px;");

        _uiStringBuilder.Append(_lineHeightStyleCssString);

        _uiStringBuilder.Append("left: ");
        _uiStringBuilder.Append(LineIndexCacheEntryMap[lineIndex].LeftCssValue);
        _uiStringBuilder.Append("px;");

        return _uiStringBuilder.ToString();
    }
        
    public void GetCursorAndCaretRowStyleCss()
    {
    	var shouldAppearAfterCollapsePoint = cursorIsOnHiddenLine;
    	
    	var leftInPixels = _renderBatch.ViewModel.GutterWidthInPixels;
    	var topInPixelsInvariantCulture = string.Empty;
	
		if (cursorIsOnHiddenLine)
		{
			foreach (var collapsePoint in _renderBatch.ViewModel.AllCollapsePointList)
			{
				if (!collapsePoint.IsCollapsed)
					continue;
			
				var lastLineIndex = collapsePoint.EndExclusiveLineIndex - 1;
				
				if (lastLineIndex == _renderBatch.ViewModel.LineIndex)
				{
					var lastLineInformation = _renderBatch.Model.GetLineInformation(lastLineIndex);
					
					var appendToLineInformation = _renderBatch.Model.GetLineInformation(collapsePoint.AppendToLineIndex);
					
					// Tab key column offset
			        {
			            var tabsOnSameLineBeforeCursor = _renderBatch.Model.GetTabCountOnSameLineBeforeCursor(
			                collapsePoint.AppendToLineIndex,
			                appendToLineInformation.LastValidColumnIndex);
			
			            // 1 of the character width is already accounted for
			
			            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
			
			            leftInPixels += extraWidthPerTabKey *
			                tabsOnSameLineBeforeCursor *
			                _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
			        }
			        
			        // +3 for the 3 dots: '[...]'
			        leftInPixels += _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth * (appendToLineInformation.LastValidColumnIndex + 3);
			        
			        if (LineIndexCacheEntryMap.ContainsKey(collapsePoint.AppendToLineIndex))
			        {
			        	topInPixelsInvariantCulture = LineIndexCacheEntryMap[collapsePoint.AppendToLineIndex].TopCssValue;
			        }
			        else
			        {
			        	if (_renderBatch.ViewModel.VirtualizationResult.EntryList.Count > 0)
			        	{
			        		var firstEntry = _renderBatch.ViewModel.VirtualizationResult.EntryList.First();
			        		topInPixelsInvariantCulture = LineIndexCacheEntryMap[firstEntry.LineIndex].TopCssValue;
			        	}
			        	else
			        	{
			        		topInPixelsInvariantCulture = 0.ToCssValue();
			        	}
			        }
			        
			        break;
				}
			}
		}

		if (!shouldAppearAfterCollapsePoint)
		{
	        // Tab key column offset
	        {
	            var tabsOnSameLineBeforeCursor = _renderBatch.Model.GetTabCountOnSameLineBeforeCursor(
	                _renderBatch.ViewModel.LineIndex,
	                _renderBatch.ViewModel.ColumnIndex);
	
	            // 1 of the character width is already accounted for
	
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            leftInPixels += extraWidthPerTabKey *
	                tabsOnSameLineBeforeCursor *
	                _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
	        }
	        
	        leftInPixels += _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth * _renderBatch.ViewModel.ColumnIndex;
	        
	        foreach (var inlineUiTuple in _renderBatch.ViewModel.InlineUiList)
			{
				var lineAndColumnIndices = _renderBatch.Model.GetLineAndColumnIndicesFromPositionIndex(inlineUiTuple.InlineUi.PositionIndex);
				
				if (lineAndColumnIndices.lineIndex == _renderBatch.ViewModel.LineIndex)
				{
					if (lineAndColumnIndices.columnIndex == _renderBatch.ViewModel.ColumnIndex)
					{
						if (_renderBatch.ViewModel.PersistentState.VirtualAssociativityKind == VirtualAssociativityKind.Right)
						{
							leftInPixels += _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth * 3;
						}
					}
					else if (lineAndColumnIndices.columnIndex <= _renderBatch.ViewModel.ColumnIndex)
					{
						leftInPixels += _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth * 3;
					}
				}
			}
	    }
        
        _uiStringBuilder.Clear();

        var leftInPixelsInvariantCulture = leftInPixels.ToCssValue();
        _uiStringBuilder.Append("left: ");
        _uiStringBuilder.Append(leftInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

		if (!shouldAppearAfterCollapsePoint)
			topInPixelsInvariantCulture = LineIndexCacheEntryMap[_renderBatch.ViewModel.LineIndex].TopCssValue;

		_uiStringBuilder.Append("top: ");
		_uiStringBuilder.Append(topInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        _uiStringBuilder.Append(_lineHeightStyleCssString);

        var widthInPixelsInvariantCulture = _renderBatch.TextEditorRenderBatchConstants.TextEditorOptions.CursorWidthInPixels.ToCssValue();
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(widthInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        _uiStringBuilder.Append(((ITextEditorKeymap)_renderBatch.TextEditorRenderBatchConstants.TextEditorOptions.Keymap).GetCursorCssStyleString(
            _renderBatch.Model,
            _renderBatch.ViewModel,
            _renderBatch.TextEditorRenderBatchConstants.TextEditorOptions));
        
        // This feels a bit hacky, exceptions are happening because the UI isn't accessing
        // the text editor in a thread safe way.
        //
        // When an exception does occur though, the cursor should receive a 'text editor changed'
        // event and re-render anyhow however.
        // 
        // So store the result of this method incase an exception occurs in future invocations,
        // to keep the cursor on screen while the state works itself out.
        _previousGetCursorStyleCss = _uiStringBuilder.ToString();
    
    	/////////////////////
    	/////////////////////
    	
    	// CaretRow starts here
    	
    	/////////////////////
    	/////////////////////
		
		_uiStringBuilder.Clear();
		
		_uiStringBuilder.Append("top: ");
		_uiStringBuilder.Append(topInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        _uiStringBuilder.Append(_lineHeightStyleCssString);

        var widthOfBodyInPixelsInvariantCulture =
            (_renderBatch.Model.MostCharactersOnASingleLineTuple.lineLength * _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth)
            .ToCssValue();

		_uiStringBuilder.Append("width: ");
		_uiStringBuilder.Append(widthOfBodyInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        // This feels a bit hacky, exceptions are happening because the UI isn't accessing
        // the text editor in a thread safe way.
        //
        // When an exception does occur though, the cursor should receive a 'text editor changed'
        // event and re-render anyhow however.
        // 
        // So store the result of this method incase an exception occurs in future invocations,
        // to keep the cursor on screen while the state works itself out.
        _previousGetCaretRowStyleCss = _uiStringBuilder.ToString();
    }

	public void HORIZONTAL_GetScrollbarHorizontalStyleCss()
    {
    	var scrollbarWidthInPixels = _renderBatch.ViewModel.TextEditorDimensions.Width -
                                     ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS -
                                     _renderBatch.ViewModel.GutterWidthInPixels;
        
        _uiStringBuilder.Clear();
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(scrollbarWidthInPixels.ToCssValue());
        _uiStringBuilder.Append("px;");

        _previous_HORIZONTAL_GetScrollbarHorizontalStyleCss_Result = _uiStringBuilder.ToString();
    }

    public void HORIZONTAL_GetSliderHorizontalStyleCss()
    {
    	var scrollbarWidthInPixels = _renderBatch.ViewModel.TextEditorDimensions.Width -
						             ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS -
						             _renderBatch.ViewModel.GutterWidthInPixels;
        
        // Proportional Left
    	var sliderProportionalLeftInPixels = _renderBatch.ViewModel.ScrollLeft *
            scrollbarWidthInPixels /
            _renderBatch.ViewModel.ScrollWidth;

		_uiStringBuilder.Clear();
		
        _uiStringBuilder.Append("bottom: 0; height: ");
        _uiStringBuilder.Append(_scrollbarSizeInPixelsCssValue);
        _uiStringBuilder.Append("px; ");
        _uiStringBuilder.Append(_previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result);
        
        _uiStringBuilder.Append(" left: ");
        _uiStringBuilder.Append(sliderProportionalLeftInPixels.ToCssValue());
        _uiStringBuilder.Append("px;");
        
        // Proportional Width
    	var pageWidth = _renderBatch.ViewModel.TextEditorDimensions.Width;

        var sliderProportionalWidthInPixels = pageWidth *
            scrollbarWidthInPixels /
            _renderBatch.ViewModel.ScrollWidth;
        
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(sliderProportionalWidthInPixels.ToCssValue());
        _uiStringBuilder.Append("px;");
        
        _previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result = _uiStringBuilder.ToString();
    }
	
    public void VERTICAL_GetSliderVerticalStyleCss()
    {
        var scrollbarHeightInPixels = _renderBatch.ViewModel.TextEditorDimensions.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Top
        var sliderProportionalTopInPixels = _renderBatch.ViewModel.ScrollTop *
            scrollbarHeightInPixels /
            _renderBatch.ViewModel.ScrollHeight;

		_uiStringBuilder.Clear();
		
		_uiStringBuilder.Append("left: 0; width: ");
		_uiStringBuilder.Append(_scrollbarSizeInPixelsCssValue);
		_uiStringBuilder.Append("px; ");
		_uiStringBuilder.Append(_previous_VERTICAL_GetSliderVerticalStyleCss_Result);
		
		_uiStringBuilder.Append("top: ");
		_uiStringBuilder.Append(sliderProportionalTopInPixels.ToCssValue());
		_uiStringBuilder.Append("px;");

        // Proportional Height
        var pageHeight = _renderBatch.ViewModel.TextEditorDimensions.Height;

        var sliderProportionalHeightInPixels = pageHeight *
            scrollbarHeightInPixels /
            _renderBatch.ViewModel.ScrollHeight;

        var sliderProportionalHeightInPixelsInvariantCulture = sliderProportionalHeightInPixels.ToCssValue();

		_uiStringBuilder.Append("height: ");
		_uiStringBuilder.Append(sliderProportionalHeightInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        _previous_VERTICAL_GetSliderVerticalStyleCss_Result = _uiStringBuilder.ToString();
    }
    
    public string PresentationGetCssStyleString(
        int position_LowerInclusiveIndex,
        int position_UpperExclusiveIndex,
        int lineIndex)
    {
        if (lineIndex >= _renderBatch.Model.LineEndList.Count)
            return string.Empty;

        var line = _renderBatch.Model.GetLineInformation(lineIndex);

        var startingColumnIndex = 0;
        var endingColumnIndex = line.Position_EndExclusiveIndex - 1;

        var fullWidthOfLineIsSelected = true;

        if (position_LowerInclusiveIndex > line.Position_StartInclusiveIndex)
        {
            startingColumnIndex = position_LowerInclusiveIndex - line.Position_StartInclusiveIndex;
            fullWidthOfLineIsSelected = false;
        }

        if (position_UpperExclusiveIndex < line.Position_EndExclusiveIndex)
        {
            endingColumnIndex = position_UpperExclusiveIndex - line.Position_StartInclusiveIndex;
            fullWidthOfLineIsSelected = false;
        }

        var topInPixelsInvariantCulture = LineIndexCacheEntryMap[lineIndex].TopCssValue;
        
        _uiStringBuilder.Clear();
        _uiStringBuilder.Append("position: absolute; ");

		_uiStringBuilder.Append("top: ");
		_uiStringBuilder.Append(topInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        _uiStringBuilder.Append("height: ");
        _uiStringBuilder.Append(_renderBatch.ViewModel.CharAndLineMeasurements.LineHeight.ToCssValue());
        _uiStringBuilder.Append("px;");
        
        // This only happens when the 'EOF' position index is "inclusive"
        // as something to be drawn for the presentation.
        if (startingColumnIndex > line.LastValidColumnIndex)
        	startingColumnIndex = line.LastValidColumnIndex;

        var startInPixels = _renderBatch.ViewModel.GutterWidthInPixels + startingColumnIndex * _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;

        // startInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameLineBeforeCursor = _renderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                lineIndex,
                startingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            startInPixels += extraWidthPerTabKey * tabsOnSameLineBeforeCursor * _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
        }

        var startInPixelsInvariantCulture = startInPixels.ToCssValue();
        _uiStringBuilder.Append("left: ");
        _uiStringBuilder.Append(startInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        var widthInPixels = endingColumnIndex * _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth - startInPixels + _renderBatch.ViewModel.GutterWidthInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameLineBeforeCursor = _renderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                lineIndex,
                line.LastValidColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            widthInPixels += extraWidthPerTabKey * tabsOnSameLineBeforeCursor * _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
        }

        _uiStringBuilder.Append("width: ");

        var fullWidthValue = _renderBatch.ViewModel.ScrollWidth;

        if (_renderBatch.ViewModel.TextEditorDimensions.Width > _renderBatch.ViewModel.ScrollWidth)
            fullWidthValue = _renderBatch.ViewModel.TextEditorDimensions.Width; // If content does not fill the viewable width of the Text Editor User Interface

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();

        if (fullWidthOfLineIsSelected)
        {
            _uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
            _uiStringBuilder.Append("px;");
        }
        else if (startingColumnIndex != 0 && position_UpperExclusiveIndex > line.Position_EndExclusiveIndex - 1)
        {
        	_uiStringBuilder.Append("calc(");
        	_uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
        	_uiStringBuilder.Append("px - ");
        	_uiStringBuilder.Append(startInPixelsInvariantCulture);
        	_uiStringBuilder.Append("px);");
        }
        else
        {
        	_uiStringBuilder.Append(widthInPixelsInvariantCulture);
        	_uiStringBuilder.Append("px;");
        }

        return _uiStringBuilder.ToString();
    }

    public string PresentationGetCssClass(TextEditorPresentationModel presentationModel, byte decorationByte)
    {
        return presentationModel.DecorationMapper.Map(decorationByte);
    }

    public IReadOnlyList<TextEditorTextSpan> PresentationVirtualizeAndShiftTextSpans(
        IReadOnlyList<TextEditorTextModification> textModifications,
        IReadOnlyList<TextEditorTextSpan> inTextSpanList)
    {
    	// TODO: Why virtualize then shift? Isn't it shift then virtualize? (2025-05-01)
    	
    	_virtualizedTextSpanList.Clear();
    	_outTextSpansList.Clear();
    
        // Virtualize the text spans
        if (_renderBatch.ViewModel.VirtualizationResult.EntryList.Any())
        {
            var lowerLineIndexInclusive = _renderBatch.ViewModel.VirtualizationResult.EntryList.First().LineIndex;
            var upperLineIndexInclusive = _renderBatch.ViewModel.VirtualizationResult.EntryList.Last().LineIndex;

            var lowerLine = _renderBatch.Model.GetLineInformation(lowerLineIndexInclusive);
            var upperLine = _renderBatch.Model.GetLineInformation(upperLineIndexInclusive);

			// Awkward enumeration was modified 'for loop' (2025-01-22)
			// Also, this shouldn't be done here, it should be done during the editContext.
			var count = inTextSpanList.Count;
            for (int i = 0; i < count; i++)
            {
            	var textSpan = inTextSpanList[i];
            	
                if (lowerLine.Position_StartInclusiveIndex <= textSpan.StartInclusiveIndex &&
                    upperLine.Position_EndExclusiveIndex >= textSpan.StartInclusiveIndex)
                {
                	_virtualizedTextSpanList.Add(textSpan);
                }
            }
        }
        else
        {
            // No 'VirtualizationResult', so don't render any text spans.
            return Array.Empty<TextEditorTextSpan>();
        }

        // Shift the text spans
        {
            foreach (var textSpan in _virtualizedTextSpanList)
            {
                var startingIndexInclusive = textSpan.StartInclusiveIndex;
                var endingIndexExclusive = textSpan.EndExclusiveIndex;

				// Awkward enumeration was modified 'for loop' (2025-01-22)
				// Also, this shouldn't be done here, it should be done during the editContext.
				var count = textModifications.Count;
                for (int i = 0; i < count; i++)
                {
                	var textModification = textModifications[i];
                
                    if (textModification.WasInsertion)
                    {
                        if (startingIndexInclusive >= textModification.TextEditorTextSpan.StartInclusiveIndex)
                        {
                            startingIndexInclusive += textModification.TextEditorTextSpan.Length;
                            endingIndexExclusive += textModification.TextEditorTextSpan.Length;
                        }
                    }
                    else // was deletion
                    {
                        if (startingIndexInclusive >= textModification.TextEditorTextSpan.StartInclusiveIndex)
                        {
                            startingIndexInclusive -= textModification.TextEditorTextSpan.Length;
                            endingIndexExclusive -= textModification.TextEditorTextSpan.Length;
                        }
                    }
                }

                _outTextSpansList.Add(textSpan with
                {
                    StartInclusiveIndex = startingIndexInclusive,
                    EndExclusiveIndex = endingIndexExclusive
                });
            }
        }

        return _outTextSpansList;
    }
    
    public (int FirstLineToSelectDataInclusive, int LastLineToSelectDataExclusive) PresentationGetBoundsInLineIndexUnits(
    	(int StartInclusiveIndex, int EndExclusiveIndex) boundsInPositionIndexUnits)
    {
        var firstLineToSelectDataInclusive = _renderBatch.Model
            .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.StartInclusiveIndex)
            .Index;

        var lastLineToSelectDataExclusive = _renderBatch.Model
            .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.EndExclusiveIndex)
            .Index +
            1;

        return (firstLineToSelectDataInclusive, lastLineToSelectDataExclusive);
    }
    
    public string GetTextSelectionStyleCss(
        int position_LowerInclusiveIndex,
        int position_UpperExclusiveIndex,
        int lineIndex)
    {
        if (lineIndex >= _renderBatch.Model.LineEndList.Count)
            return string.Empty;

        var line = _renderBatch.Model.GetLineInformation(lineIndex);

        var selectionStartingColumnIndex = 0;
        var selectionEndingColumnIndex = line.Position_EndExclusiveIndex - 1;

        var fullWidthOfLineIsSelected = true;

        if (position_LowerInclusiveIndex > line.Position_StartInclusiveIndex)
        {
            selectionStartingColumnIndex = position_LowerInclusiveIndex - line.Position_StartInclusiveIndex;
            fullWidthOfLineIsSelected = false;
        }

        if (position_UpperExclusiveIndex < line.Position_EndExclusiveIndex)
        {
            selectionEndingColumnIndex = position_UpperExclusiveIndex - line.Position_StartInclusiveIndex;
            fullWidthOfLineIsSelected = false;
        }

        var charMeasurements = _renderBatch.ViewModel.CharAndLineMeasurements;

        _uiStringBuilder.Clear();
        
        var topInPixelsInvariantCulture = LineIndexCacheEntryMap[lineIndex].TopCssValue;
        _uiStringBuilder.Append("top: ");
        _uiStringBuilder.Append(topInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        _uiStringBuilder.Append(_lineHeightStyleCssString);

        var selectionStartInPixels = _renderBatch.ViewModel.GutterWidthInPixels + selectionStartingColumnIndex * charMeasurements.CharacterWidth;

        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameLineBeforeCursor = _renderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                lineIndex,
                selectionStartingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionStartInPixels += 
                extraWidthPerTabKey * tabsOnSameLineBeforeCursor * charMeasurements.CharacterWidth;
        }

        var selectionStartInPixelsInvariantCulture = selectionStartInPixels.ToCssValue();
        _uiStringBuilder.Append("left: ");
        _uiStringBuilder.Append(selectionStartInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        var selectionWidthInPixels = 
            selectionEndingColumnIndex * charMeasurements.CharacterWidth - selectionStartInPixels + _renderBatch.ViewModel.GutterWidthInPixels;

        // Tab keys a width of many characters
        {
            var lineInformation = _renderBatch.Model.GetLineInformation(lineIndex);

            selectionEndingColumnIndex = Math.Min(
                selectionEndingColumnIndex,
                lineInformation.LastValidColumnIndex);

            var tabsOnSameLineBeforeCursor = _renderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                lineIndex,
                selectionEndingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionWidthInPixels += extraWidthPerTabKey * tabsOnSameLineBeforeCursor * charMeasurements.CharacterWidth;
        }

        _uiStringBuilder.Append("width: ");
        var fullWidthValue = _renderBatch.ViewModel.ScrollWidth;

        if (_renderBatch.ViewModel.TextEditorDimensions.Width >
            _renderBatch.ViewModel.ScrollWidth)
        {
            // If content does not fill the viewable width of the Text Editor User Interface
            fullWidthValue = _renderBatch.ViewModel.TextEditorDimensions.Width;
        }

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        if (fullWidthOfLineIsSelected)
        {
        	_uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
        	_uiStringBuilder.Append("px;");
        }
        else if (selectionStartingColumnIndex != 0 &&
                 position_UpperExclusiveIndex > line.Position_EndExclusiveIndex - 1)
        {
        	_uiStringBuilder.Append("calc(");
        	_uiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
        	_uiStringBuilder.Append("px - ");
        	_uiStringBuilder.Append(selectionStartInPixelsInvariantCulture);
        	_uiStringBuilder.Append("px);");
        }
        else
        {
        	_uiStringBuilder.Append(selectionWidthInPixels.ToCssValue());
        	_uiStringBuilder.Append("px;");
        }

        return _uiStringBuilder.ToString();
    }

    public void GetSelection()
    {
    	if (TextEditorSelectionHelper.HasSelectedText(_renderBatch.ViewModel) &&
	         _renderBatch.ViewModel.VirtualizationResult.EntryList.Count > 0)
	    {
	    	SelectionStyleList.Clear();
	    
	        selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
	            _renderBatch.ViewModel);
	
	        var selectionBoundsInLineIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToLineIndexUnits(
                _renderBatch.Model,
                selectionBoundsInPositionIndexUnits);
	
	        var virtualLowerBoundInclusiveLineIndex = _renderBatch.ViewModel.VirtualizationResult.EntryList.First().LineIndex;
	        var virtualUpperBoundExclusiveLineIndex = 1 + _renderBatch.ViewModel.VirtualizationResult.EntryList.Last().LineIndex;
	
	        useLowerBoundInclusiveLineIndex = virtualLowerBoundInclusiveLineIndex >= selectionBoundsInLineIndexUnits.Line_LowerInclusiveIndex
	            ? virtualLowerBoundInclusiveLineIndex
	            : selectionBoundsInLineIndexUnits.Line_LowerInclusiveIndex;
	
	        useUpperBoundExclusiveLineIndex = virtualUpperBoundExclusiveLineIndex <= selectionBoundsInLineIndexUnits.Line_UpperExclusiveIndex
	            ? virtualUpperBoundExclusiveLineIndex
            	: selectionBoundsInLineIndexUnits.Line_UpperExclusiveIndex;
            
            var hiddenLineCount = 0;
			var checkHiddenLineIndex = 0;
            
            for (; checkHiddenLineIndex < useLowerBoundInclusiveLineIndex; checkHiddenLineIndex++)
            {
            	if (_renderBatch.ViewModel.HiddenLineIndexHashSet.Contains(checkHiddenLineIndex))
            		hiddenLineCount++;
            }
            
            for (var i = useLowerBoundInclusiveLineIndex; i < useUpperBoundExclusiveLineIndex; i++)
	        {
	        	checkHiddenLineIndex++;
	        
	        	if (_renderBatch.ViewModel.HiddenLineIndexHashSet.Contains(i))
	        	{
	        		hiddenLineCount++;
	        		continue;
	        	}
	        	
	        	SelectionStyleList.Add(GetTextSelectionStyleCss(
		     	   selectionBoundsInPositionIndexUnits.Position_LowerInclusiveIndex,
		     	   selectionBoundsInPositionIndexUnits.Position_UpperExclusiveIndex,
		     	   lineIndex: i));
	        }
	    }
    }

	/// <summary>
	/// WARNING: Do not use '_uiStringBuilder' in this method. This method can be invoked from outside the UI thread via events.
	/// </summary>
    public void SetWrapperCssAndStyle()
    {
    	var stringBuilder = new StringBuilder();
    	
    	WrapperCssClass = TextEditorService.ThemeCssClassString;
    	
    	stringBuilder.Append("luth_te_text-editor luth_unselectable luth_te_text-editor-css-wrapper ");
    	stringBuilder.Append(WrapperCssClass);
    	stringBuilder.Append(" ");
    	stringBuilder.Append(ViewModelDisplayOptions.TextEditorClassCssString);
    	_personalWrapperCssClass = stringBuilder.ToString();
    	
    	stringBuilder.Clear();
    	
    	var options = TextEditorService.OptionsApi.GetTextEditorOptionsState().Options;
    	
    	var fontSizeInPixels = TextEditorOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS;
    	if (options.CommonOptions?.FontSizeInPixels is not null)
            fontSizeInPixels = options!.CommonOptions.FontSizeInPixels;
            
        stringBuilder.Append("font-size: ");
        stringBuilder.Append(fontSizeInPixels.ToCssValue());
        stringBuilder.Append("px;");
    	
    	var fontFamily = TextEditorRenderBatch.DEFAULT_FONT_FAMILY;
    	if (!string.IsNullOrWhiteSpace(options?.CommonOptions?.FontFamily))
        	fontFamily = options!.CommonOptions!.FontFamily;
    	
    	stringBuilder.Append("font-family: ");
    	stringBuilder.Append(fontFamily);
    	stringBuilder.Append(";");
    	
    	WrapperCssStyle = stringBuilder.ToString();
    	
    	stringBuilder.Append(WrapperCssStyle);
    	stringBuilder.Append(" ");
    	// string GetGlobalHeightInPixelsStyling()
	    {
	        var heightInPixels = TextEditorService.OptionsApi.GetTextEditorOptionsState().Options.TextEditorHeightInPixels;
	
	        if (heightInPixels is not null)
	        {
	        	var heightInPixelsInvariantCulture = heightInPixels.Value.ToCssValue();
	        
		        stringBuilder.Append("height: ");
		        stringBuilder.Append(heightInPixelsInvariantCulture);
		        stringBuilder.Append("px;");
	        }
	    }
    	stringBuilder.Append(" ");
    	stringBuilder.Append(ViewModelDisplayOptions.TextEditorStyleCssString);
    	stringBuilder.Append(" ");
    	// string GetHeightCssStyle()
	    {
	    	if (_previousIncludeHeader != ViewModelDisplayOptions.HeaderComponentType is not null ||
	    	    _previousIncludeFooter != ViewModelDisplayOptions.FooterComponentType is not null)
	    	{
	    		// Start with a calc statement and a value of 100%
		        stringBuilder.Append("height: calc(100%");
		
		        if (ViewModelDisplayOptions.HeaderComponentType is not null)
		            stringBuilder.Append(" - var(--luth_te_text-editor-header-height)");
		
		        if (ViewModelDisplayOptions.FooterComponentType is not null)
		            stringBuilder.Append(" - var(--luth_te_text-editor-footer-height)");
		
		        // Close the calc statement, and the height style attribute
		        stringBuilder.Append(");");
		        
		        _previousGetHeightCssStyleResult = stringBuilder.ToString();
	    	}
	    }
    	_personalWrapperCssStyle = stringBuilder.ToString();
    	
    	TextEditorViewModelSlimDisplay.SetRenderBatchConstants();
    }
    
    public void ConstructVirtualizationStyleCssStrings()
    {
    	if (_renderBatch.ViewModel.VirtualizationResult.TotalWidth != _previousTotalWidth)
    	{
    		_previousTotalWidth = _renderBatch.ViewModel.VirtualizationResult.TotalWidth;
    		
    		_uiStringBuilder.Clear();
	    	_uiStringBuilder.Append("width: ");
	    	_uiStringBuilder.Append(_renderBatch.ViewModel.VirtualizationResult.TotalWidth.ToCssValue());
	    	_uiStringBuilder.Append("px;");
	        _horizontalVirtualizationBoundaryStyleCssString = _uiStringBuilder.ToString();
    	}
	    	
    	if (_renderBatch.ViewModel.VirtualizationResult.TotalHeight != _previousTotalHeight)
    	{
    		_previousTotalHeight = _renderBatch.ViewModel.VirtualizationResult.TotalHeight;
    	
    		_uiStringBuilder.Clear();
	    	_uiStringBuilder.Append("height: ");
	    	_uiStringBuilder.Append(_renderBatch.ViewModel.VirtualizationResult.TotalHeight.ToCssValue());
	    	_uiStringBuilder.Append("px;");
	    	_verticalVirtualizationBoundaryStyleCssString = _uiStringBuilder.ToString();
    	}
    }
    
    private void GetPresentationLayer(
    	List<(string CssClassString, int StartInclusiveIndex, int EndExclusiveIndex)> presentationLayerGroupList,
    	List<(string PresentationCssClass, string PresentationCssStyle)> presentationLayerTextSpanList)
    {
    	presentationLayerGroupList.Clear();
    	presentationLayerTextSpanList.Clear();
    
    	foreach (var presentationKey in _renderBatch.ViewModel.PersistentState.FirstPresentationLayerKeysList)
	    {
	    	var presentationLayer = _renderBatch.Model.PresentationModelList.FirstOrDefault(
	    		x => x.TextEditorPresentationKey == presentationKey);
	        if (presentationLayer is null)
	        	continue;
	    	
	        var completedCalculation = presentationLayer.CompletedCalculation;
	        if (completedCalculation is null)
	        	continue;
	
			IReadOnlyList<TextEditorTextSpan> textSpansList = completedCalculation.TextSpanList
	            ?? Array.Empty<TextEditorTextSpan>();
	
	        IReadOnlyList<TextEditorTextModification> textModificationList = ((IReadOnlyList<TextEditorTextModification>?)completedCalculation.TextModificationsSinceRequestList)
	            ?? Array.Empty<TextEditorTextModification>();
	
        	// Should be using 'textSpansList' not 'completedCalculation.TextSpanList'?
            textSpansList = PresentationVirtualizeAndShiftTextSpans(textModificationList, completedCalculation.TextSpanList);

			var indexInclusiveStart = presentationLayerTextSpanList.Count;
			
			var hiddenLineCount = 0;
			var checkHiddenLineIndex = 0;

            foreach (var textSpan in textSpansList)
            {
                var boundsInPositionIndexUnits = (textSpan.StartInclusiveIndex, textSpan.EndExclusiveIndex);

                var boundsInLineIndexUnits = PresentationGetBoundsInLineIndexUnits(boundsInPositionIndexUnits);
                
                for (; checkHiddenLineIndex < boundsInLineIndexUnits.FirstLineToSelectDataInclusive; checkHiddenLineIndex++)
                {
                	if (_renderBatch.ViewModel.HiddenLineIndexHashSet.Contains(checkHiddenLineIndex))
                		hiddenLineCount++;
                }

                for (var i = boundsInLineIndexUnits.FirstLineToSelectDataInclusive;
                     i < boundsInLineIndexUnits.LastLineToSelectDataExclusive;
                     i++)
                {
                	checkHiddenLineIndex++;
                	
                	if (_renderBatch.ViewModel.HiddenLineIndexHashSet.Contains(i))
                	{
                		hiddenLineCount++;
                		continue;
                	}
                		
                	presentationLayerTextSpanList.Add((
                		PresentationGetCssClass(presentationLayer, textSpan.DecorationByte),
                		PresentationGetCssStyleString(
                            boundsInPositionIndexUnits.StartInclusiveIndex,
                            boundsInPositionIndexUnits.EndExclusiveIndex,
                            lineIndex: i)));
                }
            }
            
            presentationLayerGroupList.Add(
            	(
            		presentationLayer.CssClassString,
            	    indexInclusiveStart,
            	    indexExclusiveEnd: presentationLayerTextSpanList.Count)
            	);
	    }
    }
    
    private void GetInlineUiStyleList()
    {
    	if (inlineUiWidthStyleCssString is null)
    	{
	    	var widthPixels = _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth * 3;
			var widthCssValue = widthPixels.ToCssValue();
			inlineUiWidthStyleCssString = $"width: {widthCssValue}px;";
			// width: @(widthCssValue)px;
		}
    
    	inlineUiStyleList.Clear();
    	
    	foreach (var entry in _renderBatch.ViewModel.InlineUiList)
    	{
    		var lineAndColumnIndices = _renderBatch.Model.GetLineAndColumnIndicesFromPositionIndex(entry.InlineUi.PositionIndex);
    		
    		if (!LineIndexCacheEntryMap.ContainsKey(lineAndColumnIndices.lineIndex))
    			continue;
    		
    		var leftInPixels = _renderBatch.ViewModel.GutterWidthInPixels + lineAndColumnIndices.columnIndex * _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
    		
    		// Tab key column offset
    		{
	    		var tabsOnSameLineBeforeCursor = _renderBatch.Model.GetTabCountOnSameLineBeforeCursor(
				    lineAndColumnIndices.lineIndex,
				    lineAndColumnIndices.columnIndex);
				
				// 1 of the character width is already accounted for
				var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
				
				leftInPixels += extraWidthPerTabKey *
				    tabsOnSameLineBeforeCursor *
				    _renderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
			}
    		
    		var topCssValue = LineIndexCacheEntryMap[lineAndColumnIndices.lineIndex].TopCssValue;

    		_uiStringBuilder.Clear();
    		
    		_uiStringBuilder.Append("position: absolute;");
    		
    		_uiStringBuilder.Append("left: ");
    		_uiStringBuilder.Append(leftInPixels.ToCssValue());
    		_uiStringBuilder.Append("px;");
    		
    		_uiStringBuilder.Append("top: ");
    		_uiStringBuilder.Append(topCssValue);
    		_uiStringBuilder.Append("px;");
    		
    		_uiStringBuilder.Append(inlineUiWidthStyleCssString);
    		
    		_uiStringBuilder.Append(_lineHeightStyleCssString);
    		
    		inlineUiStyleList.Add(_uiStringBuilder.ToString());
    	}
    }
    
    public Dictionary<int, TextEditorLineIndexCacheEntry> LineIndexCacheEntryMap = new();
    private HashSet<int> LineIndexCacheUsageHashSet = new();
    private List<int> LineIndexKeyList = new();
    
    private void ClearCache()
    {
	   LineIndexCacheEntryMap.Clear();
	   LineIndexCacheUsageHashSet.Clear();
	   LineIndexKeyList.Clear();
    }
    
    private void CreateCache()
    {
    	if (Math.Abs(_previousViewModelGutterWidth - _renderBatch.ViewModel.GutterWidthInPixels) > 0.1)
    	{
    		_previousViewModelGutterWidth = _renderBatch.ViewModel.GutterWidthInPixels;
    		ClearCache();
    	}
    
    	var hiddenLineCount = 0;
    	var checkHiddenLineIndex = 0;
    	var handledCursor = false;
    	var isHandlingCursor = false;
    	
    	for (int i = 0; i < _renderBatch.ViewModel.VirtualizationResult.EntryList.Count; i++)
    	{
    		int lineIndex = _renderBatch.ViewModel.VirtualizationResult.EntryList[i].LineIndex;
    		
    		if (lineIndex >= _renderBatch.ViewModel.LineIndex && !handledCursor)
    		{
    		 	isHandlingCursor = true;
    		 	lineIndex = _renderBatch.ViewModel.LineIndex;
			}
    		
    		for (; checkHiddenLineIndex < lineIndex; checkHiddenLineIndex++)
            {
            	if (_renderBatch.ViewModel.HiddenLineIndexHashSet.Contains(checkHiddenLineIndex))
            		hiddenLineCount++;
            }
            
            LineIndexCacheUsageHashSet.Add(lineIndex);
            
            if (LineIndexCacheEntryMap.ContainsKey(lineIndex))
	    	{
	    		var cacheEntry = LineIndexCacheEntryMap[lineIndex];
	    		
	    		if (hiddenLineCount != cacheEntry.HiddenLineCount)
	            {
	            	cacheEntry.TopCssValue = ((lineIndex - hiddenLineCount) * _renderBatch.ViewModel.CharAndLineMeasurements.LineHeight)
	            		.ToCssValue();
	            		
	            	cacheEntry.HiddenLineCount = hiddenLineCount;
	            	
	            	LineIndexCacheEntryMap[lineIndex] = cacheEntry;
	            }
	    	}
	    	else
	    	{
	    		LineIndexKeyList.Add(lineIndex);
	    		
	    		LineIndexCacheEntryMap.Add(lineIndex, new TextEditorLineIndexCacheEntry(
	    			topCssValue: ((lineIndex - hiddenLineCount) * _renderBatch.ViewModel.CharAndLineMeasurements.LineHeight).ToCssValue(),
	    			leftCssValue: _renderBatch.ViewModel.VirtualizationResult.EntryList[i].LeftInPixels.ToCssValue(),
					lineNumberString: (lineIndex + 1).ToString(),
					hiddenLineCount: hiddenLineCount));
	    	}
	    	
	    	if (isHandlingCursor)
	    	{
	    		isHandlingCursor = false;
	    		handledCursor = true;
	    		i--;
	    		
	    		if (_renderBatch.ViewModel.HiddenLineIndexHashSet.Contains(_renderBatch.ViewModel.LineIndex))
	    			cursorIsOnHiddenLine = true;
	    	}
    	}
    	
    	if (!handledCursor)
    	{
    		LineIndexCacheUsageHashSet.Add(_renderBatch.ViewModel.LineIndex);
    		
    		if (LineIndexCacheEntryMap.ContainsKey(_renderBatch.ViewModel.LineIndex))
	    	{
	    		var cacheEntry = LineIndexCacheEntryMap[_renderBatch.ViewModel.LineIndex];
	    		
	    		if (hiddenLineCount != cacheEntry.HiddenLineCount)
	            {
	            	cacheEntry.TopCssValue = (_renderBatch.ViewModel.LineIndex * _renderBatch.ViewModel.CharAndLineMeasurements.LineHeight)
	            		.ToCssValue();
	            		
	            	cacheEntry.HiddenLineCount = 0;
	            	
	            	LineIndexCacheEntryMap[_renderBatch.ViewModel.LineIndex] = cacheEntry;
	            }
	    	}
	    	else
	    	{
	    		LineIndexKeyList.Add(_renderBatch.ViewModel.LineIndex);
	    		
	    		LineIndexCacheEntryMap.Add(_renderBatch.ViewModel.LineIndex, new TextEditorLineIndexCacheEntry(
	    			topCssValue: (_renderBatch.ViewModel.LineIndex * _renderBatch.ViewModel.CharAndLineMeasurements.LineHeight).ToCssValue(),
					lineNumberString: (_renderBatch.ViewModel.LineIndex + 1).ToString(),
					// TODO: This will cause a bug, this declares a lines left but in reality its trying to just describe the cursor and this value is placeholder.
					// But, since this placeholder is cached, if this line comes up in a future render it may or may not be positioned correctly.
					leftCssValue: _renderBatch.ViewModel.GutterWidthInPixels.ToCssValue(),
					hiddenLineCount: 0));
	    	}
	    		
    		if (_renderBatch.ViewModel.HiddenLineIndexHashSet.Contains(_renderBatch.ViewModel.LineIndex))
	    		cursorIsOnHiddenLine = true;
    	}
    }
    
    /// <summary>If the scroll left changes you have to discard the virtualized line cache.</summary>
    public double VirtualizedLineCacheCreatedWithScrollLeft = -1;
    /// <summary></summary>
    public Dictionary<int, VirtualizationLine> VirtualizedLineCacheEntryMap = new();
    /// <summary>
    /// Every virtualized line has its "spans" stored in this flat list.
    ///
    /// Then, 'virtualizationSpan_StartInclusiveIndex' and 'virtualizationSpan_EndExclusiveIndex'
    /// indicate the section of the flat list that relates to each individual line.
    ///
    /// This points to a TextEditorViewModel('s) VirtualizationGrid('s) list directly.
	/// If you clear it that'll cause a UI race condition exception.
    /// </summary>
    public List<VirtualizationSpan> VirtualizedLineCacheSpanList = new();
    private List<VirtualizationSpan> _emptyVirtualizedLineCacheSpanList = new();
    public HashSet<int> VirtualizedLineCacheUsageHashSet = new();
    public List<int> VirtualizedLineIndexKeyList = new();
    public Key<TextEditorViewModel> VirtualizedLineCacheViewModelKey = Key<TextEditorViewModel>.Empty;
    public bool VisualizationLineCacheIsInvalid { get; set; }
    /// <summary>
    /// If a line index is in the cache, but also in this list, then you need to throw away
    /// the cached result for that particular line.
    ///
    /// Any edit that changes the line endings in terms of "existence"
    /// will require throwing away of all cached results (it just won't initially be supported).
    /// </summary>
    public List<int> VirtualizedLineLineIndexWithModificationList { get; set; } = new();
    
    public void VirtualizationLineCacheClear()
    {
	    VirtualizedLineCacheCreatedWithScrollLeft = -1;
	    VirtualizedLineCacheEntryMap.Clear();
	    
	    // This points to a TextEditorViewModel('s) VirtualizationGrid('s) list directly.
	    // If you clear it that'll cause a UI race condition exception.
	    VirtualizedLineCacheSpanList = _emptyVirtualizedLineCacheSpanList;
	    
	    VirtualizedLineCacheUsageHashSet.Clear();
	    VirtualizedLineIndexKeyList.Clear();
	    VirtualizedLineCacheViewModelKey = Key<TextEditorViewModel>.Empty;
	    VisualizationLineCacheIsInvalid = false;
	    VirtualizedLineLineIndexWithModificationList.Clear();
    }
    
    private int _counter;
    
    private double _previousViewModelGutterWidth = 0;
    
    public void CreateUi()
    {
    	TextEditorViewModel? viewModel = null;
    	TextEditorModel? model = null;
    	
    	try
    	{
    		viewModel = TextEditorService.TextEditorState._viewModelMap[TextEditorViewModelSlimDisplay.TextEditorViewModelKey];
			model = TextEditorService.TextEditorState._modelMap[viewModel.PersistentState.ResourceUri];
    	}
    	catch (Exception e)
    	{
    		Console.WriteLine(e);
    	}
    
        _renderBatch = new TextEditorRenderBatch(
            model,
            viewModel,
            TextEditorViewModelSlimDisplay._textEditorRenderBatchConstants);
        
        if (!_renderBatch.IsValid)
        {
        	DiagnoseIssues(model, viewModel);
        	return;
        }
    		
    	cursorIsOnHiddenLine = false;
    		
    	LineIndexCacheUsageHashSet.Clear();
    	
    	CreateCache();
    
    	// Somewhat hacky second try-catch so the presentations
    	// don't clobber the text editor's default behavior when they throw an exception.
    	try
    	{
	        GetCursorAndCaretRowStyleCss();
	        GetSelection();
	        
	        GetPresentationLayer(firstPresentationLayerGroupList, firstPresentationLayerTextSpanList);
	        GetPresentationLayer(lastPresentationLayerGroupList, lastPresentationLayerTextSpanList);
	        
	        GetInlineUiStyleList();
        }
        catch (Exception e)
        {
        	Console.WriteLine("inner " + e);
        }
        
        if (_renderBatch.ViewModel is null)
        	return;
        
        // TODO: I think the statement:...
        // ....`if (Math.Abs(_previousGutterWidthInPixels - _renderBatch.GutterWidthInPixels) >= 0.2)`
        // can visually "thrash" back and forth between two values if one of them is negative.
        //
        // (it renders in one spot, then immediately renders in a different spot, then renders again back in the original spot
        //  and etc... cause it keeps thinking the value changed).
        //
        // I don't have any proof for this though, so until I do I'll remove the:
        // `if (_renderBatch.GutterWidthInPixels >= 0)` checks.
        //
        // And if the "thrash" effect is real then a better solution than the if check for >= 0
        // is to do this in the setter of the property itself.
        //
        // Since the measurements are "rarely" taken relative to this very hot "UI" pathway.
        
        
        // Check if the gutter width changed. If so, re-measure text editor.
    	if (Math.Abs(_previousGutterWidthInPixels - _renderBatch.GutterWidthInPixels) >= 0.1)
    	{
    		_previousGutterWidthInPixels = _renderBatch.GutterWidthInPixels;
    		var widthInPixelsInvariantCulture = _renderBatch.GutterWidthInPixels.ToCssValue();
    		
    		_uiStringBuilder.Clear();
    		_uiStringBuilder.Append("width: ");
    		_uiStringBuilder.Append(widthInPixelsInvariantCulture);
    		_uiStringBuilder.Append("px;");
    		_gutterWidthStyleCssString = _uiStringBuilder.ToString();
    		
    		_uiStringBuilder.Clear();
    		_uiStringBuilder.Append(_lineHeightStyleCssString);
	        _uiStringBuilder.Append(_gutterWidthStyleCssString);
	        _uiStringBuilder.Append(_gutterPaddingStyleCssString);
    		_gutterHeightWidthPaddingStyleCssString = _uiStringBuilder.ToString();
    		
    		_uiStringBuilder.Clear();
    		_uiStringBuilder.Append("width: calc(100% - ");
	        _uiStringBuilder.Append(widthInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px); left: ");
	        _uiStringBuilder.Append(widthInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
    		_bodyStyle = _uiStringBuilder.ToString();

    		_renderBatch.ViewModel.PersistentState.DisplayTracker.PostScrollAndRemeasure();
    		
    		HORIZONTAL_GetScrollbarHorizontalStyleCss();
    		HORIZONTAL_GetSliderHorizontalStyleCss();
    		
    		_uiStringBuilder.Clear();
    		_uiStringBuilder.Append("left: ");
    		_uiStringBuilder.Append(widthInPixelsInvariantCulture);
    		_uiStringBuilder.Append("px;");
    		_scrollbarSectionLeftCssString = _uiStringBuilder.ToString();
    		return;
    	}
		
		// NOTE: The 'gutterWidth' version of this will do a re-measure,...
		// ...and therefore will return if its condition branch was entered.
    	if (Math.Abs(_previousLineHeightInPixels - _renderBatch.ViewModel.CharAndLineMeasurements.LineHeight) >= 0.1)
    	{
    		_previousLineHeightInPixels = _renderBatch.ViewModel.CharAndLineMeasurements.LineHeight;
			
			_uiStringBuilder.Clear();
    		_uiStringBuilder.Append("height: ");
	        _uiStringBuilder.Append(_renderBatch.ViewModel.CharAndLineMeasurements.LineHeight.ToCssValue());
	        _uiStringBuilder.Append("px;");
	        _lineHeightStyleCssString = _uiStringBuilder.ToString();
	        
	        _uiStringBuilder.Clear();
    		_uiStringBuilder.Append(_lineHeightStyleCssString);
	        _uiStringBuilder.Append(_gutterWidthStyleCssString);
	        _uiStringBuilder.Append(_gutterPaddingStyleCssString);
    		_gutterHeightWidthPaddingStyleCssString = _uiStringBuilder.ToString();
		}
		
		bool shouldCalculateVerticalSlider = false;
		bool shouldCalculateHorizontalSlider = false;
		bool shouldCalculateHorizontalScrollbar = false;
		
    	if (Math.Abs(_previousTextEditorHeightInPixels - _renderBatch.ViewModel.TextEditorDimensions.Height) >= 0.1)
    	{
    		_previousTextEditorHeightInPixels = _renderBatch.ViewModel.TextEditorDimensions.Height;
    		shouldCalculateVerticalSlider = true;
	    }
		
    	if (Math.Abs(_previousScrollHeightInPixels - _renderBatch.ViewModel.ScrollHeight) >= 0.1)
    	{
    		_previousScrollHeightInPixels = _renderBatch.ViewModel.ScrollHeight;
    		shouldCalculateVerticalSlider = true;
	    }
		
    	if (Math.Abs(_previousScrollTopInPixels - _renderBatch.ViewModel.ScrollTop) >= 0.1)
    	{
    		_previousScrollTopInPixels = _renderBatch.ViewModel.ScrollTop;
    		shouldCalculateVerticalSlider = true;
	    }
		
    	if (Math.Abs(_previousTextEditorWidthInPixels - _renderBatch.ViewModel.TextEditorDimensions.Width) >= 0.1)
    	{
    		_previousTextEditorWidthInPixels = _renderBatch.ViewModel.TextEditorDimensions.Width;
    		shouldCalculateHorizontalSlider = true;
    		shouldCalculateHorizontalScrollbar = true;
	    }
		
    	if (Math.Abs(_previousScrollWidthInPixels - _renderBatch.ViewModel.ScrollWidth) >= 0.1)
    	{
    		_previousScrollWidthInPixels = _renderBatch.ViewModel.ScrollWidth;
    		shouldCalculateHorizontalSlider = true;
	    }
		
    	if (Math.Abs(_previousScrollLeftInPixels - _renderBatch.ViewModel.ScrollLeft) >= 0.1)
    	{
    		_previousScrollLeftInPixels = _renderBatch.ViewModel.ScrollLeft;
    		shouldCalculateHorizontalSlider = true;
	    }

		if (shouldCalculateVerticalSlider)
			VERTICAL_GetSliderVerticalStyleCss();
		
		if (shouldCalculateHorizontalSlider)
			HORIZONTAL_GetSliderHorizontalStyleCss();
		
		if (shouldCalculateHorizontalScrollbar)
			HORIZONTAL_GetScrollbarHorizontalStyleCss();
    
    	ConstructVirtualizationStyleCssStrings();
    	
    	if (_renderBatch.ViewModel.PersistentState.TooltipViewModel is not null)
		{
			var x = _renderBatch.ViewModel.PersistentState.TooltipViewModel.RelativeCoordinates.RelativeX +
					_renderBatch.ViewModel.PersistentState.TooltipViewModel.RelativeCoordinates.RelativeScrollLeft;
					
			var y = _renderBatch.ViewModel.PersistentState.TooltipViewModel.RelativeCoordinates.RelativeY +
					_renderBatch.ViewModel.PersistentState.TooltipViewModel.RelativeCoordinates.RelativeScrollTop;
		
			if (Math.Abs(valueTooltipRelativeX - x) >= 0.1)
			{
				valueTooltipRelativeX = x;
				tooltipRelativeX = x.ToCssValue();
			}
		
			if (Math.Abs(valueTooltipRelativeY - y) >= 0.1)
			{
				valueTooltipRelativeY = y;
				tooltipRelativeY = y.ToCssValue();
			}
		}
		
		for (int i = LineIndexKeyList.Count - 1; i >= 0; i--)
		{
			if (!LineIndexCacheUsageHashSet.Contains(LineIndexKeyList[i]))
			{
				LineIndexCacheEntryMap.Remove(LineIndexKeyList[i]);
				LineIndexKeyList.RemoveAt(i);
			}
		}
    }
    
    private void DiagnoseIssues(TextEditorModel model, TextEditorViewModel viewModel)
    {
    	if (_renderBatch.ViewModel is null)
    	{
    		return;
    	}
    }
}
