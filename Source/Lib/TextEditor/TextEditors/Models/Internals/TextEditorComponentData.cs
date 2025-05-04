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
		ITextEditorService textEditorService,
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
	}

	/*public TextEditorComponentData(
		TextEditorComponentData otherComponentData,
		Keymap keymap)
	{
		TextEditorHtmlElementId = otherComponentData.TextEditorHtmlElementId;
		ViewModelDisplayOptions = otherComponentData.ViewModelDisplayOptions;

		Options = otherComponentData.Options with
		{
			Keymap = keymap
		};

		TextEditorViewModelSlimDisplay = otherComponentData.TextEditorViewModelSlimDisplay;
		ServiceProvider = otherComponentData.ServiceProvider;
	}*/
	
	// Active is the one given to the UI after the current was validated and found to be valid.
    public TextEditorRenderBatch _currentRenderBatch;
    public TextEditorRenderBatch _previousRenderBatch;
    public TextEditorRenderBatch _activeRenderBatch;

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
	public ITextEditorService TextEditorService { get; }
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
    
    public int useLowerBoundInclusiveRowIndex;
    public int useUpperBoundExclusiveRowIndex;
    public (int Position_LowerInclusiveIndex, int Position_UpperExclusiveIndex) selectionBoundsInPositionIndexUnits;
    
    public List<(string CssClassString, int StartInclusiveIndex, int EndExclusiveIndex)> firstPresentationLayerGroupList = new();
	public List<(string PresentationCssClass, string PresentationCssStyle)> firstPresentationLayerTextSpanList = new();
	
    public List<(string CssClassString, int StartInclusiveIndex, int EndExclusiveIndex)> lastPresentationLayerGroupList = new();
	public List<(string PresentationCssClass, string PresentationCssStyle)> lastPresentationLayerTextSpanList = new();
    
    public List<string> SelectionStyleList = new List<string>();
    
    private List<TextEditorTextSpan> _virtualizedTextSpanList = new();
    private List<TextEditorTextSpan> _outTextSpansList = new();

	public void ThrottleApplySyntaxHighlighting(TextEditorModel modelModifier)
    {
        _throttleApplySyntaxHighlighting.Run(_ =>
        {
            modelModifier.CompilerService.ResourceWasModified(modelModifier.ResourceUri, Array.Empty<TextEditorTextSpan>());
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
    
    public string GetGutterStyleCss(string topCssStyle)
    {
    	_uiStringBuilder.Clear();
    
        _uiStringBuilder.Append(topCssStyle);
        _uiStringBuilder.Append(_gutterHeightWidthPaddingStyleCssString);

        return _uiStringBuilder.ToString();
    }
    
    public string GetGutterStyleCssImaginary(int index)
    {
    	_uiStringBuilder.Clear();
    
        var measurements = _activeRenderBatch.ViewModel.CharAndLineMeasurements;

        var topInPixelsInvariantCulture = (index * measurements.LineHeight).ToCssValue();
        _uiStringBuilder.Append("top: ");
        _uiStringBuilder.Append(topInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        _uiStringBuilder.Append(_gutterHeightWidthPaddingStyleCssString);

        return _uiStringBuilder.ToString();
    }

    public string GetGutterSectionStyleCss()
    {
        return _gutterWidthStyleCssString;
    }
    
    /* RowSection.razor Open */
    public string RowSection_GetRowStyleCss(string topCssStyle, double virtualizedRowLeftInPixels)
    {
    	_uiStringBuilder.Clear();
    
        _uiStringBuilder.Append(topCssStyle);

        _uiStringBuilder.Append(_lineHeightStyleCssString);

		if (virtualizedRowLeftInPixels > 0)
		{
	        var virtualizedRowLeftInPixelsInvariantCulture = virtualizedRowLeftInPixels.ToCssValue();
	        _uiStringBuilder.Append("left: ");
	        _uiStringBuilder.Append(virtualizedRowLeftInPixelsInvariantCulture);
	        _uiStringBuilder.Append("px;");
        }

        return _uiStringBuilder.ToString();
    }
        
    public void GetCursorAndCaretRowStyleCss()
    {
    	var measurements = _activeRenderBatch.ViewModel.CharAndLineMeasurements;
    	
    	var shouldAppearAfterCollapsePoint = false;
    	
    	var leftInPixels = 0d;
    	var topInPixels = 0d;

		foreach (var collapsePoint in _activeRenderBatch.ViewModel.AllCollapsePointList)
		{
			if (!collapsePoint.IsCollapsed)
				continue;
		
			var lastLineIndex = collapsePoint.EndExclusiveLineIndex - 1;
			
			if (lastLineIndex == _activeRenderBatch.ViewModel.PrimaryCursor.LineIndex)
			{
				var lastLineInformation = _activeRenderBatch.Model.GetLineInformation(lastLineIndex);
				
				if (lastLineInformation.LastValidColumnIndex == _activeRenderBatch.ViewModel.PrimaryCursor.ColumnIndex)
				{
					shouldAppearAfterCollapsePoint = true;
					
					var appendToLineInformation = _activeRenderBatch.Model.GetLineInformation(collapsePoint.AppendToLineIndex);
					
					// Tab key column offset
			        {
			            var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
			                collapsePoint.AppendToLineIndex,
			                appendToLineInformation.LastValidColumnIndex);
			
			            // 1 of the character width is already accounted for
			
			            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
			
			            leftInPixels += extraWidthPerTabKey *
			                tabsOnSameRowBeforeCursor *
			                measurements.CharacterWidth;
			        }
			        
			        // +3 for the 3 dots: '[...]'
			        leftInPixels += measurements.CharacterWidth * (appendToLineInformation.LastValidColumnIndex + 3);
			        
			        var hiddenLineCount = 0;
	
					for (int i = 0; i < collapsePoint.AppendToLineIndex; i++)
					{
						if (_activeRenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(i))
							hiddenLineCount++;
					}
			        
			        topInPixels = measurements.LineHeight * (collapsePoint.AppendToLineIndex - hiddenLineCount);
				}
			}
		}

		if (!shouldAppearAfterCollapsePoint)
		{
	        // Tab key column offset
	        {
	            var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
	                _activeRenderBatch.ViewModel.PrimaryCursor.LineIndex,
	                _activeRenderBatch.ViewModel.PrimaryCursor.ColumnIndex);
	
	            // 1 of the character width is already accounted for
	
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            leftInPixels += extraWidthPerTabKey *
	                tabsOnSameRowBeforeCursor *
	                measurements.CharacterWidth;
	        }
	        
	        leftInPixels += measurements.CharacterWidth * _activeRenderBatch.ViewModel.PrimaryCursor.ColumnIndex;
	        
	        foreach (var inlineUiTuple in _activeRenderBatch.ViewModel.InlineUiList)
			{
				var lineAndColumnIndices = _activeRenderBatch.Model.GetLineAndColumnIndicesFromPositionIndex(inlineUiTuple.InlineUi.PositionIndex);
				
				if (lineAndColumnIndices.lineIndex == _activeRenderBatch.ViewModel.PrimaryCursor.LineIndex)
				{
					if (lineAndColumnIndices.columnIndex == _activeRenderBatch.ViewModel.PrimaryCursor.ColumnIndex)
					{
						if (_activeRenderBatch.ViewModel.VirtualAssociativityKind == VirtualAssociativityKind.Right)
						{
							leftInPixels += measurements.CharacterWidth * 3;
						}
					}
					else if (lineAndColumnIndices.columnIndex <= _activeRenderBatch.ViewModel.PrimaryCursor.ColumnIndex)
					{
						leftInPixels += measurements.CharacterWidth * 3;
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
		{
			var hiddenLineCount = 0;
	
			for (int i = 0; i < _activeRenderBatch.ViewModel.PrimaryCursor.LineIndex; i++)
			{
				if (_activeRenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(i))
					hiddenLineCount++;
			}
			
			topInPixels = measurements.LineHeight * (_activeRenderBatch.ViewModel.PrimaryCursor.LineIndex - hiddenLineCount);
		}

        var topInPixelsInvariantCulture = topInPixels.ToCssValue();

		_uiStringBuilder.Append("top: ");
		_uiStringBuilder.Append(topInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        _uiStringBuilder.Append(_lineHeightStyleCssString);

        var widthInPixelsInvariantCulture = _activeRenderBatch.TextEditorRenderBatchConstants.TextEditorOptions.CursorWidthInPixels.ToCssValue();
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(widthInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        _uiStringBuilder.Append(((ITextEditorKeymap)_activeRenderBatch.TextEditorRenderBatchConstants.TextEditorOptions.Keymap).GetCursorCssStyleString(
            _activeRenderBatch.Model,
            _activeRenderBatch.ViewModel,
            _activeRenderBatch.TextEditorRenderBatchConstants.TextEditorOptions));
        
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
            (_activeRenderBatch.Model.MostCharactersOnASingleLineTuple.lineLength * measurements.CharacterWidth)
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
    	var scrollbarWidthInPixels = _activeRenderBatch.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        var scrollbarWidthInPixelsInvariantCulture = scrollbarWidthInPixels.ToCssValue();
        
        _uiStringBuilder.Clear();
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(scrollbarWidthInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        _previous_HORIZONTAL_GetScrollbarHorizontalStyleCss_Result = _uiStringBuilder.ToString();
    }

    public void HORIZONTAL_GetSliderHorizontalStyleCss()
    {
    	var scrollbarWidthInPixels = _activeRenderBatch.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;
        
        // Proportional Left
    	var sliderProportionalLeftInPixels = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft *
            scrollbarWidthInPixels /
            _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;

		var sliderProportionalLeftInPixelsInvariantCulture = sliderProportionalLeftInPixels.ToCssValue();

		_uiStringBuilder.Clear();
		
        _uiStringBuilder.Append("bottom: 0; height: ");
        _uiStringBuilder.Append(_scrollbarSizeInPixelsCssValue);
        _uiStringBuilder.Append("px; ");
        _uiStringBuilder.Append(_previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result);
        
        _uiStringBuilder.Append(" left: ");
        _uiStringBuilder.Append(sliderProportionalLeftInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");
        
        // Proportional Width
    	var pageWidth = _activeRenderBatch.ViewModel.TextEditorDimensions.Width;

        var sliderProportionalWidthInPixels = pageWidth *
            scrollbarWidthInPixels /
            _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;

        var sliderProportionalWidthInPixelsInvariantCulture = sliderProportionalWidthInPixels.ToCssValue();
        
        _uiStringBuilder.Append("width: ");
        _uiStringBuilder.Append(sliderProportionalWidthInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");
        
        _previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result = _uiStringBuilder.ToString();
    }
	
    public void VERTICAL_GetSliderVerticalStyleCss()
    {
    	var textEditorDimensions = _activeRenderBatch.ViewModel.TextEditorDimensions;
        var scrollBarDimensions = _activeRenderBatch.ViewModel.ScrollbarDimensions;

        var scrollbarHeightInPixels = textEditorDimensions.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Top
        var sliderProportionalTopInPixels = scrollBarDimensions.ScrollTop *
            scrollbarHeightInPixels /
            scrollBarDimensions.ScrollHeight;

        var sliderProportionalTopInPixelsInvariantCulture = sliderProportionalTopInPixels.ToCssValue();

		_uiStringBuilder.Clear();
		
		_uiStringBuilder.Append("left: 0; width: ");
		_uiStringBuilder.Append(_scrollbarSizeInPixelsCssValue);
		_uiStringBuilder.Append("px; ");
		_uiStringBuilder.Append(_previous_VERTICAL_GetSliderVerticalStyleCss_Result);
		
		_uiStringBuilder.Append("top: ");
		_uiStringBuilder.Append(sliderProportionalTopInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        // Proportional Height
        var pageHeight = textEditorDimensions.Height;

        var sliderProportionalHeightInPixels = pageHeight *
            scrollbarHeightInPixels /
            scrollBarDimensions.ScrollHeight;

        var sliderProportionalHeightInPixelsInvariantCulture = sliderProportionalHeightInPixels.ToCssValue();

		_uiStringBuilder.Append("height: ");
		_uiStringBuilder.Append(sliderProportionalHeightInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        _previous_VERTICAL_GetSliderVerticalStyleCss_Result = _uiStringBuilder.ToString();
    }
    
    public string PresentationGetCssStyleString(
        int position_LowerInclusiveIndex,
        int position_UpperExclusiveIndex,
        int rowIndex,
        int hiddenLineCount)
    {
        var charMeasurements = _activeRenderBatch.ViewModel.CharAndLineMeasurements;
		var textEditorDimensions = _activeRenderBatch.ViewModel.TextEditorDimensions;
        var scrollbarDimensions = _activeRenderBatch.ViewModel.ScrollbarDimensions;

        if (rowIndex >= _activeRenderBatch.Model.LineEndList.Count)
            return string.Empty;

        var line = _activeRenderBatch.Model.GetLineInformation(rowIndex);

        var startingColumnIndex = 0;
        var endingColumnIndex = line.Position_EndExclusiveIndex - 1;

        var fullWidthOfRowIsSelected = true;

        if (position_LowerInclusiveIndex > line.Position_StartInclusiveIndex)
        {
            startingColumnIndex = position_LowerInclusiveIndex - line.Position_StartInclusiveIndex;
            fullWidthOfRowIsSelected = false;
        }

        if (position_UpperExclusiveIndex < line.Position_EndExclusiveIndex)
        {
            endingColumnIndex = position_UpperExclusiveIndex - line.Position_StartInclusiveIndex;
            fullWidthOfRowIsSelected = false;
        }

        var topInPixelsInvariantCulture = ((rowIndex - hiddenLineCount) * charMeasurements.LineHeight).ToCssValue();
        
        _uiStringBuilder.Clear();
        _uiStringBuilder.Append("position: absolute; ");

		_uiStringBuilder.Append("top: ");
		_uiStringBuilder.Append(topInPixelsInvariantCulture);
		_uiStringBuilder.Append("px;");

        var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();

        _uiStringBuilder.Append("height: ");
        _uiStringBuilder.Append(heightInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");
        
        // This only happens when the 'EOF' position index is "inclusive"
        // as something to be drawn for the presentation.
        if (startingColumnIndex > line.LastValidColumnIndex)
        	startingColumnIndex = line.LastValidColumnIndex;

        var startInPixels = startingColumnIndex * charMeasurements.CharacterWidth;

        // startInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                rowIndex,
                startingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            startInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
        }

        var startInPixelsInvariantCulture = startInPixels.ToCssValue();
        _uiStringBuilder.Append("left: ");
        _uiStringBuilder.Append(startInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        var widthInPixels = endingColumnIndex * charMeasurements.CharacterWidth - startInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                rowIndex,
                line.LastValidColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            widthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
        }

        _uiStringBuilder.Append("width: ");

        var fullWidthValue = scrollbarDimensions.ScrollWidth;

        if (textEditorDimensions.Width > scrollbarDimensions.ScrollWidth)
            fullWidthValue = textEditorDimensions.Width; // If content does not fill the viewable width of the Text Editor User Interface

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();

        if (fullWidthOfRowIsSelected)
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
        if (_activeRenderBatch.ViewModel.VirtualizationResult.EntryList.Any())
        {
            var lowerLineIndexInclusive = _activeRenderBatch.ViewModel.VirtualizationResult.EntryList.First().LineIndex;
            var upperLineIndexInclusive = _activeRenderBatch.ViewModel.VirtualizationResult.EntryList.Last().LineIndex;

            var lowerLine = _activeRenderBatch.Model.GetLineInformation(lowerLineIndexInclusive);
            var upperLine = _activeRenderBatch.Model.GetLineInformation(upperLineIndexInclusive);

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
    
    public (int FirstRowToSelectDataInclusive, int LastRowToSelectDataExclusive) PresentationGetBoundsInRowIndexUnits(
    	(int StartInclusiveIndex, int EndExclusiveIndex) boundsInPositionIndexUnits)
    {
        var firstRowToSelectDataInclusive = _activeRenderBatch.Model
            .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.StartInclusiveIndex)
            .Index;

        var lastRowToSelectDataExclusive = _activeRenderBatch.Model
            .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.EndExclusiveIndex)
            .Index +
            1;

        return (firstRowToSelectDataInclusive, lastRowToSelectDataExclusive);
    }
    
    public string GetTextSelectionStyleCss(
        int position_LowerInclusiveIndex,
        int position_UpperExclusiveIndex,
        int rowIndex,
        int hiddenLineCount)
    {
        if (rowIndex >= _activeRenderBatch.Model.LineEndList.Count)
            return string.Empty;

        var line = _activeRenderBatch.Model.GetLineInformation(rowIndex);

        var selectionStartingColumnIndex = 0;
        var selectionEndingColumnIndex = line.Position_EndExclusiveIndex - 1;

        var fullWidthOfRowIsSelected = true;

        if (position_LowerInclusiveIndex > line.Position_StartInclusiveIndex)
        {
            selectionStartingColumnIndex = position_LowerInclusiveIndex - line.Position_StartInclusiveIndex;
            fullWidthOfRowIsSelected = false;
        }

        if (position_UpperExclusiveIndex < line.Position_EndExclusiveIndex)
        {
            selectionEndingColumnIndex = position_UpperExclusiveIndex - line.Position_StartInclusiveIndex;
            fullWidthOfRowIsSelected = false;
        }

        var charMeasurements = _activeRenderBatch.ViewModel.CharAndLineMeasurements;

        _uiStringBuilder.Clear();
        
        var topInPixelsInvariantCulture = ((rowIndex - hiddenLineCount) * charMeasurements.LineHeight).ToCssValue();
        _uiStringBuilder.Append("top: ");
        _uiStringBuilder.Append(topInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        _uiStringBuilder.Append(_lineHeightStyleCssString);

        var selectionStartInPixels = selectionStartingColumnIndex * charMeasurements.CharacterWidth;

        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                rowIndex,
                selectionStartingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionStartInPixels += 
                extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
        }

        var selectionStartInPixelsInvariantCulture = selectionStartInPixels.ToCssValue();
        _uiStringBuilder.Append("left: ");
        _uiStringBuilder.Append(selectionStartInPixelsInvariantCulture);
        _uiStringBuilder.Append("px;");

        var selectionWidthInPixels = 
            selectionEndingColumnIndex * charMeasurements.CharacterWidth - selectionStartInPixels;

        // Tab keys a width of many characters
        {
            var lineInformation = _activeRenderBatch.Model.GetLineInformation(rowIndex);

            selectionEndingColumnIndex = Math.Min(
                selectionEndingColumnIndex,
                lineInformation.LastValidColumnIndex);

            var tabsOnSameRowBeforeCursor = _activeRenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                rowIndex,
                selectionEndingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionWidthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
        }

        _uiStringBuilder.Append("width: ");
        var fullWidthValue = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;

        if (_activeRenderBatch.ViewModel.TextEditorDimensions.Width >
            _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth)
        {
            // If content does not fill the viewable width of the Text Editor User Interface
            fullWidthValue = _activeRenderBatch.ViewModel.TextEditorDimensions.Width;
        }

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        var selectionWidthInPixelsInvariantCulture = selectionWidthInPixels.ToCssValue();

        if (fullWidthOfRowIsSelected)
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
        	_uiStringBuilder.Append(selectionWidthInPixelsInvariantCulture);
        	_uiStringBuilder.Append("px;");
        }

        return _uiStringBuilder.ToString();
    }

    public void GetSelection()
    {
    	if (TextEditorSelectionHelper.HasSelectedText(_activeRenderBatch.ViewModel.PrimaryCursor.Selection) &&
	         _activeRenderBatch.ViewModel.VirtualizationResult.EntryList.Count > 0)
	    {
	    	SelectionStyleList.Clear();
	    
	        selectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
	            _activeRenderBatch.ViewModel.PrimaryCursor.Selection);
	
	        var selectionBoundsInLineIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToLineIndexUnits(
                _activeRenderBatch.Model,
                selectionBoundsInPositionIndexUnits);
	
	        var virtualLowerBoundInclusiveRowIndex = _activeRenderBatch.ViewModel.VirtualizationResult.EntryList.First().LineIndex;
	        var virtualUpperBoundExclusiveRowIndex = 1 + _activeRenderBatch.ViewModel.VirtualizationResult.EntryList.Last().LineIndex;
	
	        useLowerBoundInclusiveRowIndex = virtualLowerBoundInclusiveRowIndex >= selectionBoundsInLineIndexUnits.Line_LowerInclusiveIndex
	            ? virtualLowerBoundInclusiveRowIndex
	            : selectionBoundsInLineIndexUnits.Line_LowerInclusiveIndex;
	
	        useUpperBoundExclusiveRowIndex = virtualUpperBoundExclusiveRowIndex <= selectionBoundsInLineIndexUnits.Line_UpperExclusiveIndex
	            ? virtualUpperBoundExclusiveRowIndex
            	: selectionBoundsInLineIndexUnits.Line_UpperExclusiveIndex;
            
            var hiddenLineCount = 0;
			var checkHiddenLineIndex = 0;
            
            for (; checkHiddenLineIndex < useLowerBoundInclusiveRowIndex; checkHiddenLineIndex++)
            {
            	if (_activeRenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(checkHiddenLineIndex))
            		hiddenLineCount++;
            }
            
            for (var i = useLowerBoundInclusiveRowIndex; i < useUpperBoundExclusiveRowIndex; i++)
	        {
	        	checkHiddenLineIndex++;
	        
	        	if (_activeRenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(i))
	        	{
	        		hiddenLineCount++;
	        		continue;
	        	}
	        	
	        	SelectionStyleList.Add(GetTextSelectionStyleCss(
		     	   selectionBoundsInPositionIndexUnits.Position_LowerInclusiveIndex,
		     	   selectionBoundsInPositionIndexUnits.Position_UpperExclusiveIndex,
		     	   rowIndex: i,
		     	   hiddenLineCount));
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
    	if (_activeRenderBatch.ViewModel.VirtualizationResult.TotalWidth != _previousTotalWidth)
    	{
    		_previousTotalWidth = _activeRenderBatch.ViewModel.VirtualizationResult.TotalWidth;
    		
    		_uiStringBuilder.Clear();
	    	_uiStringBuilder.Append("width: ");
	    	_uiStringBuilder.Append(_activeRenderBatch.ViewModel.VirtualizationResult.TotalWidth.ToCssValue());
	    	_uiStringBuilder.Append("px;");
	        _horizontalVirtualizationBoundaryStyleCssString = _uiStringBuilder.ToString();
    	}
	    	
    	if (_activeRenderBatch.ViewModel.VirtualizationResult.TotalHeight != _previousTotalHeight)
    	{
    		_previousTotalHeight = _activeRenderBatch.ViewModel.VirtualizationResult.TotalHeight;
    	
    		_uiStringBuilder.Clear();
	    	_uiStringBuilder.Append("height: ");
	    	_uiStringBuilder.Append(_activeRenderBatch.ViewModel.VirtualizationResult.TotalHeight.ToCssValue());
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
    
    	foreach (var presentationKey in _activeRenderBatch.ViewModel.FirstPresentationLayerKeysList)
	    {
	    	var presentationLayer = _activeRenderBatch.Model.PresentationModelList.FirstOrDefault(
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

                var boundsInRowIndexUnits = PresentationGetBoundsInRowIndexUnits(boundsInPositionIndexUnits);
                
                for (; checkHiddenLineIndex < boundsInRowIndexUnits.FirstRowToSelectDataInclusive; checkHiddenLineIndex++)
                {
                	if (_activeRenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(checkHiddenLineIndex))
                		hiddenLineCount++;
                }

                for (var i = boundsInRowIndexUnits.FirstRowToSelectDataInclusive;
                     i < boundsInRowIndexUnits.LastRowToSelectDataExclusive;
                     i++)
                {
                	checkHiddenLineIndex++;
                	
                	if (_activeRenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(i))
                	{
                		hiddenLineCount++;
                		continue;
                	}
                		
                	presentationLayerTextSpanList.Add((
                		PresentationGetCssClass(presentationLayer, textSpan.DecorationByte),
                		PresentationGetCssStyleString(
                            boundsInPositionIndexUnits.StartInclusiveIndex,
                            boundsInPositionIndexUnits.EndExclusiveIndex,
                            rowIndex: i,
                            hiddenLineCount)));
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
    
    public void CreateUi()
    {
    	if (!_activeRenderBatch.ConstructorWasInvoked)
    		return;
    
    	try
    	{
	        GetCursorAndCaretRowStyleCss();
	        GetSelection();
	        
	        GetPresentationLayer(firstPresentationLayerGroupList, firstPresentationLayerTextSpanList);
	        GetPresentationLayer(lastPresentationLayerGroupList, lastPresentationLayerTextSpanList);
        }
        catch (Exception e)
        {
        	Console.WriteLine("inner " + e);
        }
        
        if (_activeRenderBatch.ViewModel is null)
        	return;
        
        // Check if the gutter width changed. If so, re-measure text editor.
		if (_activeRenderBatch.GutterWidthInPixels >= 0)
		{
        	if (Math.Abs(_previousGutterWidthInPixels - _activeRenderBatch.GutterWidthInPixels) >= 0.2)
        	{
        		_previousGutterWidthInPixels = _activeRenderBatch.GutterWidthInPixels;
        		var widthInPixelsInvariantCulture = _activeRenderBatch.GutterWidthInPixels.ToCssValue();
        		
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

        		_activeRenderBatch.ViewModel.DisplayTracker.PostScrollAndRemeasure();
        		return;
        	}
		}
		
		// NOTE: The 'gutterWidth' version of this will do a re-measure,...
		// ...and therefore will return if its condition branch was entered.
		if (_activeRenderBatch.ViewModel.CharAndLineMeasurements.LineHeight >= 0)
		{
        	if (Math.Abs(_previousLineHeightInPixels - _activeRenderBatch.ViewModel.CharAndLineMeasurements.LineHeight) >= 0.2)
        	{
        		_previousLineHeightInPixels = _activeRenderBatch.ViewModel.CharAndLineMeasurements.LineHeight;
				
				_uiStringBuilder.Clear();
        		_uiStringBuilder.Append("height: ");
		        _uiStringBuilder.Append(_activeRenderBatch.ViewModel.CharAndLineMeasurements.LineHeight.ToCssValue());
		        _uiStringBuilder.Append("px;");
		        _lineHeightStyleCssString = _uiStringBuilder.ToString();
		        
		        _uiStringBuilder.Clear();
        		_uiStringBuilder.Append(_lineHeightStyleCssString);
		        _uiStringBuilder.Append(_gutterWidthStyleCssString);
		        _uiStringBuilder.Append(_gutterPaddingStyleCssString);
        		_gutterHeightWidthPaddingStyleCssString = _uiStringBuilder.ToString();
    		}
		}
		
		bool shouldCalculateVerticalSlider = false;
		bool shouldCalculateHorizontalSlider = false;
		bool shouldCalculateHorizontalScrollbar = false;
		
		if (_activeRenderBatch.ViewModel.TextEditorDimensions.Height >= 0)
		{
        	if (Math.Abs(_previousTextEditorHeightInPixels - _activeRenderBatch.ViewModel.TextEditorDimensions.Height) >= 0.2)
        	{
        		_previousTextEditorHeightInPixels = _activeRenderBatch.ViewModel.TextEditorDimensions.Height;
        		shouldCalculateVerticalSlider = true;
		    }
		}
		
		if (_activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollHeight >= 0)
		{
        	if (Math.Abs(_previousScrollHeightInPixels - _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollHeight) >= 0.2)
        	{
        		_previousScrollHeightInPixels = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollHeight;
        		shouldCalculateVerticalSlider = true;
		    }
		}
		
		if (_activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollTop >= 0)
		{
        	if (Math.Abs(_previousScrollTopInPixels - _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollTop) >= 0.2)
        	{
        		_previousScrollTopInPixels = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollTop;
        		shouldCalculateVerticalSlider = true;
		    }
		}
		
		if (_activeRenderBatch.ViewModel.TextEditorDimensions.Width >= 0)
		{
        	if (Math.Abs(_previousTextEditorWidthInPixels - _activeRenderBatch.ViewModel.TextEditorDimensions.Width) >= 0.2)
        	{
        		_previousTextEditorWidthInPixels = _activeRenderBatch.ViewModel.TextEditorDimensions.Width;
        		shouldCalculateHorizontalSlider = true;
        		shouldCalculateHorizontalScrollbar = true;
		    }
		}
		
		if (_activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth >= 0)
		{
        	if (Math.Abs(_previousScrollWidthInPixels - _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth) >= 0.2)
        	{
        		_previousScrollWidthInPixels = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;
        		shouldCalculateHorizontalSlider = true;
		    }
		}
		
		if (_activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft >= 0)
		{
        	if (Math.Abs(_previousScrollLeftInPixels - _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft) >= 0.2)
        	{
        		_previousScrollLeftInPixels = _activeRenderBatch.ViewModel.ScrollbarDimensions.ScrollLeft;
        		shouldCalculateHorizontalSlider = true;
		    }
		}

		if (shouldCalculateVerticalSlider)
			VERTICAL_GetSliderVerticalStyleCss();
		
		if (shouldCalculateHorizontalSlider)
			HORIZONTAL_GetSliderHorizontalStyleCss();
		
		if (shouldCalculateHorizontalScrollbar)
			HORIZONTAL_GetScrollbarHorizontalStyleCss();
    
    	ConstructVirtualizationStyleCssStrings();
    	
    	if (_activeRenderBatch.ViewModel.TooltipViewModel is not null)
		{
			if (valueTooltipRelativeX != _activeRenderBatch.ViewModel.TooltipViewModel.RelativeCoordinates.RelativeX)
			{
				valueTooltipRelativeX = _activeRenderBatch.ViewModel.TooltipViewModel.RelativeCoordinates.RelativeX;
				tooltipRelativeX = _activeRenderBatch.ViewModel.TooltipViewModel.RelativeCoordinates.RelativeX.ToCssValue();
			}
		
			if (valueTooltipRelativeY != _activeRenderBatch.ViewModel.TooltipViewModel.RelativeCoordinates.RelativeY)
			{
				valueTooltipRelativeY = _activeRenderBatch.ViewModel.TooltipViewModel.RelativeCoordinates.RelativeY;
				tooltipRelativeY = _activeRenderBatch.ViewModel.TooltipViewModel.RelativeCoordinates.RelativeY.ToCssValue();
			}
		}
    }
}
