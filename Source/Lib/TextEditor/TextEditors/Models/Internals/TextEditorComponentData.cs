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
	
	public string? InlineUiWidthStyleCssString { get; set; }
	
	public bool CursorIsOnHiddenLine { get; set; } = false;

	// Active is the one given to the UI after the current was validated and found to be valid.
    public TextEditorRenderBatch RenderBatch { get; set; }
    
    public int ShouldScroll { get; set; }
    
	public string RowSectionElementId { get; }
	public string PrimaryCursorContentId { get; }
	public string GutterElementId { get; }
	public string FindOverlayId { get; }
	
	public Dictionary<int, TextEditorLineIndexCacheEntry> LineIndexCacheEntryMap { get; set; } = new();
    private HashSet<int> LineIndexCacheUsageHashSet { get; set; } = new();
    private List<int> LineIndexKeyList { get; set; } = new();

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
    
    public int UseLowerBoundInclusiveLineIndex { get; set; }
    public int UseUpperBoundExclusiveLineIndex { get; set; }
    public (int Position_LowerInclusiveIndex, int Position_UpperExclusiveIndex) SelectionBoundsInPositionIndexUnits { get; set; }
    
    public List<(string CssClassString, int StartInclusiveIndex, int EndExclusiveIndex)> FirstPresentationLayerGroupList { get; set; } = new();
	public List<(string PresentationCssClass, string PresentationCssStyle)> FirstPresentationLayerTextSpanList { get; set; } = new();
	
    public List<(string CssClassString, int StartInclusiveIndex, int EndExclusiveIndex)> LastPresentationLayerGroupList { get; set; } = new();
	public List<(string PresentationCssClass, string PresentationCssStyle)> LastPresentationLayerTextSpanList { get; set; } = new();
	
	public List<string> InlineUiStyleList { get; set; } = new();
    
    public List<string> SelectionStyleList { get; set; } = new List<string>();
    
    private List<TextEditorTextSpan> VirtualizedTextSpanList { get; set; } = new();
    private List<TextEditorTextSpan> OutTextSpansList { get; set; } = new();

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
    
	public double PreviousLineHeightInPixels { get; set; } = 0;
	
	public double PreviousTextEditorHeightInPixels { get; set; } = 0;
	public double PreviousScrollHeightInPixels { get; set; } = 0;
	public double PreviousScrollTopInPixels { get; set; } = 0;
	public double PreviousTextEditorWidthInPixels { get; set; } = 0;
	public double PreviousScrollWidthInPixels { get; set; } = 0;
	public double PreviousScrollLeftInPixels { get; set; } = 0;
	
	public string GutterPaddingStyleCssString { get; set; }
    public string GutterWidthStyleCssString { get; set; }
    
    public string ScrollbarSectionLeftCssString { get; set; }
    
    /// <summary>If the scroll left changes you have to discard the virtualized line cache.</summary>
    public double VirtualizedLineCacheCreatedWithScrollLeft { get; set; } = -1;
    /// <summary></summary>
    public Dictionary<int, VirtualizationLine> VirtualizedLineCacheEntryMap { get; set; } = new();
    /// <summary>
    /// Every virtualized line has its "spans" stored in this flat list.
    ///
    /// Then, 'virtualizationSpan_StartInclusiveIndex' and 'virtualizationSpan_EndExclusiveIndex'
    /// indicate the section of the flat list that relates to each individual line.
    ///
    /// This points to a TextEditorViewModel('s) VirtualizationGrid('s) list directly.
	/// If you clear it that'll cause a UI race condition exception.
    /// </summary>
    public List<VirtualizationSpan> VirtualizedLineCacheSpanList { get; set; } = new();
    private List<VirtualizationSpan> EmptyVirtualizedLineCacheSpanList { get; set; } = new();
    public HashSet<int> VirtualizedLineCacheUsageHashSet { get; set; } = new();
    public List<int> VirtualizedLineIndexKeyList { get; set; } = new();
    public Key<TextEditorViewModel> VirtualizedLineCacheViewModelKey { get; set; } = Key<TextEditorViewModel>.Empty;
    public bool VisualizationLineCacheIsInvalid { get; set; }
    /// <summary>
    /// If a line index is in the cache, but also in this list, then you need to throw away
    /// the cached result for that particular line.
    ///
    /// Any edit that changes the line endings in terms of "existence"
    /// will require throwing away of all cached results (it just won't initially be supported).
    /// </summary>
    public List<int> VirtualizedLineLineIndexWithModificationList { get; set; } = new();
    
    private int Counter { get; set; }
    
    private double PreviousViewModelGutterWidth { get; set; } = 0;
    private double PreviousViewModelScrollLeft { get; set; } = 0;
    
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
    public string GutterHeightWidthPaddingStyleCssString { get; set; }
    
    public string LineHeightStyleCssString { get; set; }
    
    public string Previous_VERTICAL_GetSliderVerticalStyleCss_Result { get; set; }
    public string Previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result { get; set; }
    public string Previous_HORIZONTAL_GetScrollbarHorizontalStyleCss_Result { get; set; }
    
    public string BodyStyle { get; set; } = $"width: 100%; left: 0;";
    
    public string ScrollbarSizeInPixelsCssValue { get; set; }
    
    public bool PreviousIncludeHeader { get; set; }
    public bool PreviousIncludeFooter { get; set; }
    public string PreviousGetHeightCssStyleResult { get; set; } = "height: calc(100%);";
    
    public string VerticalVirtualizationBoundaryStyleCssString { get; set; } = "height: 0px;";
	public string HorizontalVirtualizationBoundaryStyleCssString { get; set; } = "width: 0px;";
	
	public double PreviousTotalWidth { get; set; }
	public double PreviousTotalHeight { get; set; }
    
    public string PersonalWrapperCssClass { get; set; }
    public string PersonalWrapperCssStyle { get; set; }
    
    public string PreviousGetCursorStyleCss { get; set; } = string.Empty;
    public string PreviousGetCaretRowStyleCss { get; set; } = string.Empty;
    
    public string BlinkAnimationCssClassOn { get; set; }
    public string BlinkAnimationCssClassOff { get; set; }
    
    public double ValueTooltipRelativeX { get; set; }
    public double ValueTooltipRelativeY { get; set; }
	
	public string TooltipRelativeX { get; set; } = string.Empty;
	public string TooltipRelativeY { get; set; } = string.Empty;
	
	// _ = "luth_te_text-editor-cursor " + BlinkAnimationCssClass + " " + _activeRenderBatch.Options.Keymap.GetCursorCssClassString();
	public string BlinkAnimationCssClass => TextEditorService.ViewModelApi.CursorShouldBlink
        ? BlinkAnimationCssClassOn
        : BlinkAnimationCssClassOff;

	public string WrapperCssClass { get; private set; }
    public string WrapperCssStyle { get; private set; }
    
    /// <summary>
    /// Share this StringBuilder when used for rendering and no other function is currently using it.
    /// (i.e.: only use this for methods that were invoked from the .razor file)
    /// </summary>
    public StringBuilder UiStringBuilder { get; set; } = new();
    
    public string GetGutterStyleCss(string topCssValue)
    {
    	UiStringBuilder.Clear();
    
    	UiStringBuilder.Append("top: ");
        UiStringBuilder.Append(topCssValue);
        UiStringBuilder.Append("px;");
    
        UiStringBuilder.Append(GutterHeightWidthPaddingStyleCssString);

        return UiStringBuilder.ToString();
    }
    
    public string GetGutterStyleCssImaginary()
    {
    	int lastIndex;
					
		if (RenderBatch.ViewModel.VirtualizationResult.EntryList.Count > 0)
		{
			lastIndex = RenderBatch.ViewModel.VirtualizationResult.EntryList.Last().LineIndex;
		}
		else
		{
			lastIndex = -1;
		}
		
		UiStringBuilder.Clear();
		
		if (lastIndex == -1)
		{
			int topValue = 0;
	        UiStringBuilder.Append("top: ");
	        UiStringBuilder.Append(topValue);
	        UiStringBuilder.Append("px;");
		}
    	else
    	{
    		var lastLineCacheEntry = LineIndexCacheEntryMap[lastIndex];
	
	        var topInPixelsInvariantCulture =
	        	((lastIndex + 1 - lastLineCacheEntry.HiddenLineCount) * RenderBatch.ViewModel.CharAndLineMeasurements.LineHeight)
	        	.ToCssValue();
	        	
	        UiStringBuilder.Append("top: ");
	        UiStringBuilder.Append(topInPixelsInvariantCulture);
	        UiStringBuilder.Append("px;");
    	}
    	
        UiStringBuilder.Append(GutterHeightWidthPaddingStyleCssString);

        return UiStringBuilder.ToString();
    }

    public string GetGutterSectionStyleCss()
    {
        return GutterWidthStyleCssString;
    }
    
    /* RowSection.razor Open */
    public string RowSection_GetRowStyleCss(int lineIndex)
    {
    	UiStringBuilder.Clear();
    
        UiStringBuilder.Append("top: ");
        UiStringBuilder.Append(LineIndexCacheEntryMap[lineIndex].TopCssValue);
        UiStringBuilder.Append("px;");

        UiStringBuilder.Append(LineHeightStyleCssString);

        UiStringBuilder.Append("left: ");
        UiStringBuilder.Append(LineIndexCacheEntryMap[lineIndex].LeftCssValue);
        UiStringBuilder.Append("px;");

        return UiStringBuilder.ToString();
    }
        
    public void GetCursorAndCaretRowStyleCss()
    {
    	var shouldAppearAfterCollapsePoint = CursorIsOnHiddenLine;
    	
    	var leftInPixels = RenderBatch.ViewModel.GutterWidthInPixels;
    	var topInPixelsInvariantCulture = string.Empty;
	
		if (CursorIsOnHiddenLine)
		{
			foreach (var collapsePoint in RenderBatch.ViewModel.AllCollapsePointList)
			{
				if (!collapsePoint.IsCollapsed)
					continue;
			
				var lastLineIndex = collapsePoint.EndExclusiveLineIndex - 1;
				
				if (lastLineIndex == RenderBatch.ViewModel.LineIndex)
				{
					var lastLineInformation = RenderBatch.Model.GetLineInformation(lastLineIndex);
					
					var appendToLineInformation = RenderBatch.Model.GetLineInformation(collapsePoint.AppendToLineIndex);
					
					// Tab key column offset
			        {
			            var tabsOnSameLineBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
			                collapsePoint.AppendToLineIndex,
			                appendToLineInformation.LastValidColumnIndex);
			
			            // 1 of the character width is already accounted for
			
			            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
			
			            leftInPixels += extraWidthPerTabKey *
			                tabsOnSameLineBeforeCursor *
			                RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
			        }
			        
			        // +3 for the 3 dots: '[...]'
			        leftInPixels += RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth * (appendToLineInformation.LastValidColumnIndex + 3);
			        
			        if (LineIndexCacheEntryMap.ContainsKey(collapsePoint.AppendToLineIndex))
			        {
			        	topInPixelsInvariantCulture = LineIndexCacheEntryMap[collapsePoint.AppendToLineIndex].TopCssValue;
			        }
			        else
			        {
			        	if (RenderBatch.ViewModel.VirtualizationResult.EntryList.Count > 0)
			        	{
			        		var firstEntry = RenderBatch.ViewModel.VirtualizationResult.EntryList.First();
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
	            var tabsOnSameLineBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
	                RenderBatch.ViewModel.LineIndex,
	                RenderBatch.ViewModel.ColumnIndex);
	
	            // 1 of the character width is already accounted for
	
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            leftInPixels += extraWidthPerTabKey *
	                tabsOnSameLineBeforeCursor *
	                RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
	        }
	        
	        leftInPixels += RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth * RenderBatch.ViewModel.ColumnIndex;
	        
	        foreach (var inlineUiTuple in RenderBatch.ViewModel.InlineUiList)
			{
				var lineAndColumnIndices = RenderBatch.Model.GetLineAndColumnIndicesFromPositionIndex(inlineUiTuple.InlineUi.PositionIndex);
				
				if (lineAndColumnIndices.lineIndex == RenderBatch.ViewModel.LineIndex)
				{
					if (lineAndColumnIndices.columnIndex == RenderBatch.ViewModel.ColumnIndex)
					{
						if (RenderBatch.ViewModel.PersistentState.VirtualAssociativityKind == VirtualAssociativityKind.Right)
						{
							leftInPixels += RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth * 3;
						}
					}
					else if (lineAndColumnIndices.columnIndex <= RenderBatch.ViewModel.ColumnIndex)
					{
						leftInPixels += RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth * 3;
					}
				}
			}
	    }
        
        UiStringBuilder.Clear();

        var leftInPixelsInvariantCulture = leftInPixels.ToCssValue();
        UiStringBuilder.Append("left: ");
        UiStringBuilder.Append(leftInPixelsInvariantCulture);
        UiStringBuilder.Append("px;");

		if (!shouldAppearAfterCollapsePoint)
			topInPixelsInvariantCulture = LineIndexCacheEntryMap[RenderBatch.ViewModel.LineIndex].TopCssValue;

		UiStringBuilder.Append("top: ");
		UiStringBuilder.Append(topInPixelsInvariantCulture);
		UiStringBuilder.Append("px;");

        UiStringBuilder.Append(LineHeightStyleCssString);

        var widthInPixelsInvariantCulture = RenderBatch.TextEditorRenderBatchConstants.TextEditorOptions.CursorWidthInPixels.ToCssValue();
        UiStringBuilder.Append("width: ");
        UiStringBuilder.Append(widthInPixelsInvariantCulture);
        UiStringBuilder.Append("px;");

        UiStringBuilder.Append(((ITextEditorKeymap)RenderBatch.TextEditorRenderBatchConstants.TextEditorOptions.Keymap).GetCursorCssStyleString(
            RenderBatch.Model,
            RenderBatch.ViewModel,
            RenderBatch.TextEditorRenderBatchConstants.TextEditorOptions));
        
        // This feels a bit hacky, exceptions are happening because the UI isn't accessing
        // the text editor in a thread safe way.
        //
        // When an exception does occur though, the cursor should receive a 'text editor changed'
        // event and re-render anyhow however.
        // 
        // So store the result of this method incase an exception occurs in future invocations,
        // to keep the cursor on screen while the state works itself out.
        PreviousGetCursorStyleCss = UiStringBuilder.ToString();
    
    	/////////////////////
    	/////////////////////
    	
    	// CaretRow starts here
    	
    	/////////////////////
    	/////////////////////
		
		UiStringBuilder.Clear();
		
		UiStringBuilder.Append("top: ");
		UiStringBuilder.Append(topInPixelsInvariantCulture);
		UiStringBuilder.Append("px;");

        UiStringBuilder.Append(LineHeightStyleCssString);

        var widthOfBodyInPixelsInvariantCulture =
            (RenderBatch.Model.MostCharactersOnASingleLineTuple.lineLength * RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth)
            .ToCssValue();

		UiStringBuilder.Append("width: ");
		UiStringBuilder.Append(widthOfBodyInPixelsInvariantCulture);
		UiStringBuilder.Append("px;");

        // This feels a bit hacky, exceptions are happening because the UI isn't accessing
        // the text editor in a thread safe way.
        //
        // When an exception does occur though, the cursor should receive a 'text editor changed'
        // event and re-render anyhow however.
        // 
        // So store the result of this method incase an exception occurs in future invocations,
        // to keep the cursor on screen while the state works itself out.
        PreviousGetCaretRowStyleCss = UiStringBuilder.ToString();
    }

	public void HORIZONTAL_GetScrollbarHorizontalStyleCss()
    {
    	var scrollbarWidthInPixels = RenderBatch.ViewModel.TextEditorDimensions.Width -
                                     ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS -
                                     RenderBatch.ViewModel.GutterWidthInPixels;
        
        UiStringBuilder.Clear();
        UiStringBuilder.Append("width: ");
        UiStringBuilder.Append(scrollbarWidthInPixels.ToCssValue());
        UiStringBuilder.Append("px;");

        Previous_HORIZONTAL_GetScrollbarHorizontalStyleCss_Result = UiStringBuilder.ToString();
    }

    public void HORIZONTAL_GetSliderHorizontalStyleCss()
    {
    	var scrollbarWidthInPixels = RenderBatch.ViewModel.TextEditorDimensions.Width -
						             ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS -
						             RenderBatch.ViewModel.GutterWidthInPixels;
        
        // Proportional Left
    	var sliderProportionalLeftInPixels = RenderBatch.ViewModel.ScrollLeft *
            scrollbarWidthInPixels /
            RenderBatch.ViewModel.ScrollWidth;

		UiStringBuilder.Clear();
		
        UiStringBuilder.Append("bottom: 0; height: ");
        UiStringBuilder.Append(ScrollbarSizeInPixelsCssValue);
        UiStringBuilder.Append("px; ");
        UiStringBuilder.Append(Previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result);
        
        UiStringBuilder.Append(" left: ");
        UiStringBuilder.Append(sliderProportionalLeftInPixels.ToCssValue());
        UiStringBuilder.Append("px;");
        
        // Proportional Width
    	var pageWidth = RenderBatch.ViewModel.TextEditorDimensions.Width;

        var sliderProportionalWidthInPixels = pageWidth *
            scrollbarWidthInPixels /
            RenderBatch.ViewModel.ScrollWidth;
        
        UiStringBuilder.Append("width: ");
        UiStringBuilder.Append(sliderProportionalWidthInPixels.ToCssValue());
        UiStringBuilder.Append("px;");
        
        Previous_HORIZONTAL_GetSliderHorizontalStyleCss_Result = UiStringBuilder.ToString();
    }
	
    public void VERTICAL_GetSliderVerticalStyleCss()
    {
        var scrollbarHeightInPixels = RenderBatch.ViewModel.TextEditorDimensions.Height - ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        // Proportional Top
        var sliderProportionalTopInPixels = RenderBatch.ViewModel.ScrollTop *
            scrollbarHeightInPixels /
            RenderBatch.ViewModel.ScrollHeight;

		UiStringBuilder.Clear();
		
		UiStringBuilder.Append("left: 0; width: ");
		UiStringBuilder.Append(ScrollbarSizeInPixelsCssValue);
		UiStringBuilder.Append("px; ");
		UiStringBuilder.Append(Previous_VERTICAL_GetSliderVerticalStyleCss_Result);
		
		UiStringBuilder.Append("top: ");
		UiStringBuilder.Append(sliderProportionalTopInPixels.ToCssValue());
		UiStringBuilder.Append("px;");

        // Proportional Height
        var pageHeight = RenderBatch.ViewModel.TextEditorDimensions.Height;

        var sliderProportionalHeightInPixels = pageHeight *
            scrollbarHeightInPixels /
            RenderBatch.ViewModel.ScrollHeight;

        var sliderProportionalHeightInPixelsInvariantCulture = sliderProportionalHeightInPixels.ToCssValue();

		UiStringBuilder.Append("height: ");
		UiStringBuilder.Append(sliderProportionalHeightInPixelsInvariantCulture);
		UiStringBuilder.Append("px;");

        Previous_VERTICAL_GetSliderVerticalStyleCss_Result = UiStringBuilder.ToString();
    }
    
    public string PresentationGetCssStyleString(
        int position_LowerInclusiveIndex,
        int position_UpperExclusiveIndex,
        int lineIndex)
    {
        if (lineIndex >= RenderBatch.Model.LineEndList.Count)
            return string.Empty;

        var line = RenderBatch.Model.GetLineInformation(lineIndex);

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
        
        UiStringBuilder.Clear();
        UiStringBuilder.Append("position: absolute; ");

		UiStringBuilder.Append("top: ");
		UiStringBuilder.Append(topInPixelsInvariantCulture);
		UiStringBuilder.Append("px;");

        UiStringBuilder.Append("height: ");
        UiStringBuilder.Append(RenderBatch.ViewModel.CharAndLineMeasurements.LineHeight.ToCssValue());
        UiStringBuilder.Append("px;");
        
        // This only happens when the 'EOF' position index is "inclusive"
        // as something to be drawn for the presentation.
        if (startingColumnIndex > line.LastValidColumnIndex)
        	startingColumnIndex = line.LastValidColumnIndex;

        var startInPixels = RenderBatch.ViewModel.GutterWidthInPixels + startingColumnIndex * RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;

        // startInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameLineBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                lineIndex,
                startingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            startInPixels += extraWidthPerTabKey * tabsOnSameLineBeforeCursor * RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
        }

        var startInPixelsInvariantCulture = startInPixels.ToCssValue();
        UiStringBuilder.Append("left: ");
        UiStringBuilder.Append(startInPixelsInvariantCulture);
        UiStringBuilder.Append("px;");

        var widthInPixels = endingColumnIndex * RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth - startInPixels + RenderBatch.ViewModel.GutterWidthInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameLineBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                lineIndex,
                line.LastValidColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            widthInPixels += extraWidthPerTabKey * tabsOnSameLineBeforeCursor * RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
        }

        UiStringBuilder.Append("width: ");

        var fullWidthValue = RenderBatch.ViewModel.ScrollWidth;

        if (RenderBatch.ViewModel.TextEditorDimensions.Width > RenderBatch.ViewModel.ScrollWidth)
            fullWidthValue = RenderBatch.ViewModel.TextEditorDimensions.Width; // If content does not fill the viewable width of the Text Editor User Interface

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();

        if (fullWidthOfLineIsSelected)
        {
            UiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
            UiStringBuilder.Append("px;");
        }
        else if (startingColumnIndex != 0 && position_UpperExclusiveIndex > line.Position_EndExclusiveIndex - 1)
        {
        	UiStringBuilder.Append("calc(");
        	UiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
        	UiStringBuilder.Append("px - ");
        	UiStringBuilder.Append(startInPixelsInvariantCulture);
        	UiStringBuilder.Append("px);");
        }
        else
        {
        	UiStringBuilder.Append(widthInPixelsInvariantCulture);
        	UiStringBuilder.Append("px;");
        }

        return UiStringBuilder.ToString();
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
    	
    	VirtualizedTextSpanList.Clear();
    	OutTextSpansList.Clear();
    
        // Virtualize the text spans
        if (RenderBatch.ViewModel.VirtualizationResult.EntryList.Any())
        {
            var lowerLineIndexInclusive = RenderBatch.ViewModel.VirtualizationResult.EntryList.First().LineIndex;
            var upperLineIndexInclusive = RenderBatch.ViewModel.VirtualizationResult.EntryList.Last().LineIndex;

            var lowerLine = RenderBatch.Model.GetLineInformation(lowerLineIndexInclusive);
            var upperLine = RenderBatch.Model.GetLineInformation(upperLineIndexInclusive);

			// Awkward enumeration was modified 'for loop' (2025-01-22)
			// Also, this shouldn't be done here, it should be done during the editContext.
			var count = inTextSpanList.Count;
            for (int i = 0; i < count; i++)
            {
            	var textSpan = inTextSpanList[i];
            	
                if (lowerLine.Position_StartInclusiveIndex <= textSpan.StartInclusiveIndex &&
                    upperLine.Position_EndExclusiveIndex >= textSpan.StartInclusiveIndex)
                {
                	VirtualizedTextSpanList.Add(textSpan);
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
            foreach (var textSpan in VirtualizedTextSpanList)
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

                OutTextSpansList.Add(textSpan with
                {
                    StartInclusiveIndex = startingIndexInclusive,
                    EndExclusiveIndex = endingIndexExclusive
                });
            }
        }

        return OutTextSpansList;
    }
    
    public (int FirstLineToSelectDataInclusive, int LastLineToSelectDataExclusive) PresentationGetBoundsInLineIndexUnits(
    	(int StartInclusiveIndex, int EndExclusiveIndex) boundsInPositionIndexUnits)
    {
        var firstLineToSelectDataInclusive = RenderBatch.Model
            .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.StartInclusiveIndex)
            .Index;

        var lastLineToSelectDataExclusive = RenderBatch.Model
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
        if (lineIndex >= RenderBatch.Model.LineEndList.Count)
            return string.Empty;

        var line = RenderBatch.Model.GetLineInformation(lineIndex);

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

        var charMeasurements = RenderBatch.ViewModel.CharAndLineMeasurements;

        UiStringBuilder.Clear();
        
        var topInPixelsInvariantCulture = LineIndexCacheEntryMap[lineIndex].TopCssValue;
        UiStringBuilder.Append("top: ");
        UiStringBuilder.Append(topInPixelsInvariantCulture);
        UiStringBuilder.Append("px;");

        UiStringBuilder.Append(LineHeightStyleCssString);

        var selectionStartInPixels = RenderBatch.ViewModel.GutterWidthInPixels + selectionStartingColumnIndex * charMeasurements.CharacterWidth;

        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameLineBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                lineIndex,
                selectionStartingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionStartInPixels += 
                extraWidthPerTabKey * tabsOnSameLineBeforeCursor * charMeasurements.CharacterWidth;
        }

        var selectionStartInPixelsInvariantCulture = selectionStartInPixels.ToCssValue();
        UiStringBuilder.Append("left: ");
        UiStringBuilder.Append(selectionStartInPixelsInvariantCulture);
        UiStringBuilder.Append("px;");

        var selectionWidthInPixels = 
            selectionEndingColumnIndex * charMeasurements.CharacterWidth - selectionStartInPixels + RenderBatch.ViewModel.GutterWidthInPixels;

        // Tab keys a width of many characters
        {
            var lineInformation = RenderBatch.Model.GetLineInformation(lineIndex);

            selectionEndingColumnIndex = Math.Min(
                selectionEndingColumnIndex,
                lineInformation.LastValidColumnIndex);

            var tabsOnSameLineBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                lineIndex,
                selectionEndingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionWidthInPixels += extraWidthPerTabKey * tabsOnSameLineBeforeCursor * charMeasurements.CharacterWidth;
        }

        UiStringBuilder.Append("width: ");
        var fullWidthValue = RenderBatch.ViewModel.ScrollWidth;

        if (RenderBatch.ViewModel.TextEditorDimensions.Width >
            RenderBatch.ViewModel.ScrollWidth)
        {
            // If content does not fill the viewable width of the Text Editor User Interface
            fullWidthValue = RenderBatch.ViewModel.TextEditorDimensions.Width;
        }

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        if (fullWidthOfLineIsSelected)
        {
        	UiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
        	UiStringBuilder.Append("px;");
        }
        else if (selectionStartingColumnIndex != 0 &&
                 position_UpperExclusiveIndex > line.Position_EndExclusiveIndex - 1)
        {
        	UiStringBuilder.Append("calc(");
        	UiStringBuilder.Append(fullWidthValueInPixelsInvariantCulture);
        	UiStringBuilder.Append("px - ");
        	UiStringBuilder.Append(selectionStartInPixelsInvariantCulture);
        	UiStringBuilder.Append("px);");
        }
        else
        {
        	UiStringBuilder.Append(selectionWidthInPixels.ToCssValue());
        	UiStringBuilder.Append("px;");
        }

        return UiStringBuilder.ToString();
    }

    public void GetSelection()
    {
    	if (TextEditorSelectionHelper.HasSelectedText(RenderBatch.ViewModel) &&
	         RenderBatch.ViewModel.VirtualizationResult.EntryList.Count > 0)
	    {
	    	SelectionStyleList.Clear();
	    
	        SelectionBoundsInPositionIndexUnits = TextEditorSelectionHelper.GetSelectionBounds(
	            RenderBatch.ViewModel);
	
	        var selectionBoundsInLineIndexUnits = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToLineIndexUnits(
                RenderBatch.Model,
                SelectionBoundsInPositionIndexUnits);
	
	        var virtualLowerBoundInclusiveLineIndex = RenderBatch.ViewModel.VirtualizationResult.EntryList.First().LineIndex;
	        var virtualUpperBoundExclusiveLineIndex = 1 + RenderBatch.ViewModel.VirtualizationResult.EntryList.Last().LineIndex;
	
	        UseLowerBoundInclusiveLineIndex = virtualLowerBoundInclusiveLineIndex >= selectionBoundsInLineIndexUnits.Line_LowerInclusiveIndex
	            ? virtualLowerBoundInclusiveLineIndex
	            : selectionBoundsInLineIndexUnits.Line_LowerInclusiveIndex;
	
	        UseUpperBoundExclusiveLineIndex = virtualUpperBoundExclusiveLineIndex <= selectionBoundsInLineIndexUnits.Line_UpperExclusiveIndex
	            ? virtualUpperBoundExclusiveLineIndex
            	: selectionBoundsInLineIndexUnits.Line_UpperExclusiveIndex;
            
            var hiddenLineCount = 0;
			var checkHiddenLineIndex = 0;
            
            for (; checkHiddenLineIndex < UseLowerBoundInclusiveLineIndex; checkHiddenLineIndex++)
            {
            	if (RenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(checkHiddenLineIndex))
            		hiddenLineCount++;
            }
            
            for (var i = UseLowerBoundInclusiveLineIndex; i < UseUpperBoundExclusiveLineIndex; i++)
	        {
	        	checkHiddenLineIndex++;
	        
	        	if (RenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(i))
	        	{
	        		hiddenLineCount++;
	        		continue;
	        	}
	        	
	        	SelectionStyleList.Add(GetTextSelectionStyleCss(
		     	   SelectionBoundsInPositionIndexUnits.Position_LowerInclusiveIndex,
		     	   SelectionBoundsInPositionIndexUnits.Position_UpperExclusiveIndex,
		     	   lineIndex: i));
	        }
	    }
    }

	/// <summary>
	/// WARNING: Do not use 'UiStringBuilder' in this method. This method can be invoked from outside the UI thread via events.
	/// </summary>
    public void SetWrapperCssAndStyle()
    {
    	var stringBuilder = new StringBuilder();
    	
    	WrapperCssClass = TextEditorService.ThemeCssClassString;
    	
    	stringBuilder.Append("luth_te_text-editor luth_unselectable luth_te_text-editor-css-wrapper ");
    	stringBuilder.Append(WrapperCssClass);
    	stringBuilder.Append(" ");
    	stringBuilder.Append(ViewModelDisplayOptions.TextEditorClassCssString);
    	PersonalWrapperCssClass = stringBuilder.ToString();
    	
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
	    	if (PreviousIncludeHeader != ViewModelDisplayOptions.HeaderComponentType is not null ||
	    	    PreviousIncludeFooter != ViewModelDisplayOptions.FooterComponentType is not null)
	    	{
	    		// Start with a calc statement and a value of 100%
		        stringBuilder.Append("height: calc(100%");
		
		        if (ViewModelDisplayOptions.HeaderComponentType is not null)
		            stringBuilder.Append(" - var(--luth_te_text-editor-header-height)");
		
		        if (ViewModelDisplayOptions.FooterComponentType is not null)
		            stringBuilder.Append(" - var(--luth_te_text-editor-footer-height)");
		
		        // Close the calc statement, and the height style attribute
		        stringBuilder.Append(");");
		        
		        PreviousGetHeightCssStyleResult = stringBuilder.ToString();
	    	}
	    }
    	PersonalWrapperCssStyle = stringBuilder.ToString();
    	
    	TextEditorViewModelSlimDisplay.SetRenderBatchConstants();
    }
    
    public void ConstructVirtualizationStyleCssStrings()
    {
    	if (Math.Abs(PreviousTotalWidth - RenderBatch.ViewModel.VirtualizationResult.TotalWidth) > 0.1)
    	{
    		PreviousTotalWidth = RenderBatch.ViewModel.VirtualizationResult.TotalWidth;
    		
    		UiStringBuilder.Clear();
	    	UiStringBuilder.Append("width: ");
	    	UiStringBuilder.Append(RenderBatch.ViewModel.VirtualizationResult.TotalWidth.ToCssValue());
	    	UiStringBuilder.Append("px;");
	        HorizontalVirtualizationBoundaryStyleCssString = UiStringBuilder.ToString();
    	}
	    	
    	if (Math.Abs(PreviousTotalHeight - RenderBatch.ViewModel.VirtualizationResult.TotalHeight) > 0.1)
    	{
    		PreviousTotalHeight = RenderBatch.ViewModel.VirtualizationResult.TotalHeight;
    	
    		UiStringBuilder.Clear();
	    	UiStringBuilder.Append("height: ");
	    	UiStringBuilder.Append(RenderBatch.ViewModel.VirtualizationResult.TotalHeight.ToCssValue());
	    	UiStringBuilder.Append("px;");
	    	VerticalVirtualizationBoundaryStyleCssString = UiStringBuilder.ToString();
    	}
    }
    
    private void GetPresentationLayer(
    	List<(string CssClassString, int StartInclusiveIndex, int EndExclusiveIndex)> presentationLayerGroupList,
    	List<(string PresentationCssClass, string PresentationCssStyle)> presentationLayerTextSpanList)
    {
    	presentationLayerGroupList.Clear();
    	presentationLayerTextSpanList.Clear();
    
    	foreach (var presentationKey in RenderBatch.ViewModel.PersistentState.FirstPresentationLayerKeysList)
	    {
	    	var presentationLayer = RenderBatch.Model.PresentationModelList.FirstOrDefault(
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
                	if (RenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(checkHiddenLineIndex))
                		hiddenLineCount++;
                }

                for (var i = boundsInLineIndexUnits.FirstLineToSelectDataInclusive;
                     i < boundsInLineIndexUnits.LastLineToSelectDataExclusive;
                     i++)
                {
                	checkHiddenLineIndex++;
                	
                	if (RenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(i))
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
    	if (InlineUiWidthStyleCssString is null)
    	{
	    	var widthPixels = RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth * 3;
			var widthCssValue = widthPixels.ToCssValue();
			InlineUiWidthStyleCssString = $"width: {widthCssValue}px;";
			// width: @(widthCssValue)px;
		}
    
    	InlineUiStyleList.Clear();
    	
    	foreach (var entry in RenderBatch.ViewModel.InlineUiList)
    	{
    		var lineAndColumnIndices = RenderBatch.Model.GetLineAndColumnIndicesFromPositionIndex(entry.InlineUi.PositionIndex);
    		
    		if (!LineIndexCacheEntryMap.ContainsKey(lineAndColumnIndices.lineIndex))
    			continue;
    		
    		var leftInPixels = RenderBatch.ViewModel.GutterWidthInPixels + lineAndColumnIndices.columnIndex * RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
    		
    		// Tab key column offset
    		{
	    		var tabsOnSameLineBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
				    lineAndColumnIndices.lineIndex,
				    lineAndColumnIndices.columnIndex);
				
				// 1 of the character width is already accounted for
				var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
				
				leftInPixels += extraWidthPerTabKey *
				    tabsOnSameLineBeforeCursor *
				    RenderBatch.ViewModel.CharAndLineMeasurements.CharacterWidth;
			}
    		
    		var topCssValue = LineIndexCacheEntryMap[lineAndColumnIndices.lineIndex].TopCssValue;

    		UiStringBuilder.Clear();
    		
    		UiStringBuilder.Append("position: absolute;");
    		
    		UiStringBuilder.Append("left: ");
    		UiStringBuilder.Append(leftInPixels.ToCssValue());
    		UiStringBuilder.Append("px;");
    		
    		UiStringBuilder.Append("top: ");
    		UiStringBuilder.Append(topCssValue);
    		UiStringBuilder.Append("px;");
    		
    		UiStringBuilder.Append(InlineUiWidthStyleCssString);
    		
    		UiStringBuilder.Append(LineHeightStyleCssString);
    		
    		InlineUiStyleList.Add(UiStringBuilder.ToString());
    	}
    }
    
    private void ClearLineIndexCache()
    {
	   LineIndexCacheEntryMap.Clear();
	   LineIndexCacheUsageHashSet.Clear();
	   LineIndexKeyList.Clear();
    }
    
    private void CreateLineIndexCache()
    {
    	if (Math.Abs(PreviousViewModelGutterWidth - RenderBatch.ViewModel.GutterWidthInPixels) > 0.1)
    	{
    		PreviousViewModelGutterWidth = RenderBatch.ViewModel.GutterWidthInPixels;
    		ClearLineIndexCache();
    		
    		var widthInPixelsInvariantCulture = RenderBatch.ViewModel.GutterWidthInPixels.ToCssValue();
    		
    		UiStringBuilder.Clear();
    		UiStringBuilder.Append("width: ");
    		UiStringBuilder.Append(widthInPixelsInvariantCulture);
    		UiStringBuilder.Append("px;");
    		GutterWidthStyleCssString = UiStringBuilder.ToString();
    		
    		UiStringBuilder.Clear();
    		UiStringBuilder.Append(LineHeightStyleCssString);
	        UiStringBuilder.Append(GutterWidthStyleCssString);
	        UiStringBuilder.Append(GutterPaddingStyleCssString);
    		GutterHeightWidthPaddingStyleCssString = UiStringBuilder.ToString();
    		
    		UiStringBuilder.Clear();
    		UiStringBuilder.Append("width: calc(100% - ");
	        UiStringBuilder.Append(widthInPixelsInvariantCulture);
	        UiStringBuilder.Append("px); left: ");
	        UiStringBuilder.Append(widthInPixelsInvariantCulture);
	        UiStringBuilder.Append("px;");
    		BodyStyle = UiStringBuilder.ToString();

    		RenderBatch.ViewModel.PersistentState.DisplayTracker.PostScrollAndRemeasure();
    		
    		HORIZONTAL_GetScrollbarHorizontalStyleCss();
    		HORIZONTAL_GetSliderHorizontalStyleCss();
    		
    		UiStringBuilder.Clear();
    		UiStringBuilder.Append("left: ");
    		UiStringBuilder.Append(widthInPixelsInvariantCulture);
    		UiStringBuilder.Append("px;");
    		ScrollbarSectionLeftCssString = UiStringBuilder.ToString();
    	}
    	else if (Math.Abs(PreviousViewModelScrollLeft - RenderBatch.ViewModel.ScrollLeft) > 0.1)
    	{
    		PreviousViewModelScrollLeft = RenderBatch.ViewModel.ScrollLeft;
    		ClearLineIndexCache();
    	}
    
    	var hiddenLineCount = 0;
    	var checkHiddenLineIndex = 0;
    	var handledCursor = false;
    	var isHandlingCursor = false;
    	
    	for (int i = 0; i < RenderBatch.ViewModel.VirtualizationResult.EntryList.Count; i++)
    	{
    		int lineIndex = RenderBatch.ViewModel.VirtualizationResult.EntryList[i].LineIndex;
    		
    		if (lineIndex >= RenderBatch.ViewModel.LineIndex && !handledCursor)
    		{
    		 	isHandlingCursor = true;
    		 	lineIndex = RenderBatch.ViewModel.LineIndex;
			}
    		
    		for (; checkHiddenLineIndex < lineIndex; checkHiddenLineIndex++)
            {
            	if (RenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(checkHiddenLineIndex))
            		hiddenLineCount++;
            }
            
            LineIndexCacheUsageHashSet.Add(lineIndex);
            
            if (LineIndexCacheEntryMap.ContainsKey(lineIndex))
	    	{
	    		var cacheEntry = LineIndexCacheEntryMap[lineIndex];
	    		
	    		if (hiddenLineCount != cacheEntry.HiddenLineCount)
	            {
	            	cacheEntry.TopCssValue = ((lineIndex - hiddenLineCount) * RenderBatch.ViewModel.CharAndLineMeasurements.LineHeight)
	            		.ToCssValue();
	            		
	            	cacheEntry.HiddenLineCount = hiddenLineCount;
	            	
	            	LineIndexCacheEntryMap[lineIndex] = cacheEntry;
	            }
	    	}
	    	else
	    	{
	    		LineIndexKeyList.Add(lineIndex);
	    		
	    		LineIndexCacheEntryMap.Add(lineIndex, new TextEditorLineIndexCacheEntry(
	    			topCssValue: ((lineIndex - hiddenLineCount) * RenderBatch.ViewModel.CharAndLineMeasurements.LineHeight).ToCssValue(),
	    			leftCssValue: RenderBatch.ViewModel.VirtualizationResult.EntryList[i].LeftInPixels.ToCssValue(),
					lineNumberString: (lineIndex + 1).ToString(),
					hiddenLineCount: hiddenLineCount));
	    	}
	    	
	    	if (isHandlingCursor)
	    	{
	    		isHandlingCursor = false;
	    		handledCursor = true;
	    		i--;
	    		
	    		if (RenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(RenderBatch.ViewModel.LineIndex))
	    			CursorIsOnHiddenLine = true;
	    	}
    	}
    	
    	if (!handledCursor)
    	{
    		LineIndexCacheUsageHashSet.Add(RenderBatch.ViewModel.LineIndex);
    		
    		if (LineIndexCacheEntryMap.ContainsKey(RenderBatch.ViewModel.LineIndex))
	    	{
	    		var cacheEntry = LineIndexCacheEntryMap[RenderBatch.ViewModel.LineIndex];
	    		
	    		if (hiddenLineCount != cacheEntry.HiddenLineCount)
	            {
	            	cacheEntry.TopCssValue = (RenderBatch.ViewModel.LineIndex * RenderBatch.ViewModel.CharAndLineMeasurements.LineHeight)
	            		.ToCssValue();
	            		
	            	cacheEntry.HiddenLineCount = 0;
	            	
	            	LineIndexCacheEntryMap[RenderBatch.ViewModel.LineIndex] = cacheEntry;
	            }
	    	}
	    	else
	    	{
	    		LineIndexKeyList.Add(RenderBatch.ViewModel.LineIndex);
	    		
	    		LineIndexCacheEntryMap.Add(RenderBatch.ViewModel.LineIndex, new TextEditorLineIndexCacheEntry(
	    			topCssValue: (RenderBatch.ViewModel.LineIndex * RenderBatch.ViewModel.CharAndLineMeasurements.LineHeight).ToCssValue(),
					lineNumberString: (RenderBatch.ViewModel.LineIndex + 1).ToString(),
					// TODO: This will cause a bug, this declares a lines left but in reality its trying to just describe the cursor and this value is placeholder.
					// But, since this placeholder is cached, if this line comes up in a future render it may or may not be positioned correctly.
					leftCssValue: RenderBatch.ViewModel.GutterWidthInPixels.ToCssValue(),
					hiddenLineCount: 0));
	    	}
	    		
    		if (RenderBatch.ViewModel.HiddenLineIndexHashSet.Contains(RenderBatch.ViewModel.LineIndex))
	    		CursorIsOnHiddenLine = true;
    	}
    }
    
    public void VirtualizationLineCacheClear()
    {
	    VirtualizedLineCacheCreatedWithScrollLeft = -1;
	    VirtualizedLineCacheEntryMap.Clear();
	    
	    // This points to a TextEditorViewModel('s) VirtualizationGrid('s) list directly.
	    // If you clear it that'll cause a UI race condition exception.
	    VirtualizedLineCacheSpanList = EmptyVirtualizedLineCacheSpanList;
	    
	    VirtualizedLineCacheUsageHashSet.Clear();
	    VirtualizedLineIndexKeyList.Clear();
	    VirtualizedLineCacheViewModelKey = Key<TextEditorViewModel>.Empty;
	    VisualizationLineCacheIsInvalid = false;
	    VirtualizedLineLineIndexWithModificationList.Clear();
    }
    
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
    
        RenderBatch = new TextEditorRenderBatch(
            model,
            viewModel,
            TextEditorViewModelSlimDisplay._textEditorRenderBatchConstants);
        
        if (!RenderBatch.IsValid)
        {
        	DiagnoseIssues(model, viewModel);
        	return;
        }
    		
    	CursorIsOnHiddenLine = false;
    		
    	LineIndexCacheUsageHashSet.Clear();
    	
    	CreateLineIndexCache();
    
    	// Somewhat hacky second try-catch so the presentations
    	// don't clobber the text editor's default behavior when they throw an exception.
    	try
    	{
	        GetCursorAndCaretRowStyleCss();
	        GetSelection();
	        
	        GetPresentationLayer(FirstPresentationLayerGroupList, FirstPresentationLayerTextSpanList);
	        GetPresentationLayer(LastPresentationLayerGroupList, LastPresentationLayerTextSpanList);
	        
	        GetInlineUiStyleList();
        }
        catch (Exception e)
        {
        	Console.WriteLine("inner " + e);
        }
        
    	if (Math.Abs(PreviousLineHeightInPixels - RenderBatch.ViewModel.CharAndLineMeasurements.LineHeight) >= 0.1)
    	{
    		PreviousLineHeightInPixels = RenderBatch.ViewModel.CharAndLineMeasurements.LineHeight;
			
			UiStringBuilder.Clear();
    		UiStringBuilder.Append("height: ");
	        UiStringBuilder.Append(RenderBatch.ViewModel.CharAndLineMeasurements.LineHeight.ToCssValue());
	        UiStringBuilder.Append("px;");
	        LineHeightStyleCssString = UiStringBuilder.ToString();
	        
	        UiStringBuilder.Clear();
    		UiStringBuilder.Append(LineHeightStyleCssString);
	        UiStringBuilder.Append(GutterWidthStyleCssString);
	        UiStringBuilder.Append(GutterPaddingStyleCssString);
    		GutterHeightWidthPaddingStyleCssString = UiStringBuilder.ToString();
		}
		
		bool shouldCalculateVerticalSlider = false;
		bool shouldCalculateHorizontalSlider = false;
		bool shouldCalculateHorizontalScrollbar = false;
		
    	if (Math.Abs(PreviousTextEditorHeightInPixels - RenderBatch.ViewModel.TextEditorDimensions.Height) >= 0.1)
    	{
    		PreviousTextEditorHeightInPixels = RenderBatch.ViewModel.TextEditorDimensions.Height;
    		shouldCalculateVerticalSlider = true;
	    }
		
    	if (Math.Abs(PreviousScrollHeightInPixels - RenderBatch.ViewModel.ScrollHeight) >= 0.1)
    	{
    		PreviousScrollHeightInPixels = RenderBatch.ViewModel.ScrollHeight;
    		shouldCalculateVerticalSlider = true;
	    }
		
    	if (Math.Abs(PreviousScrollTopInPixels - RenderBatch.ViewModel.ScrollTop) >= 0.1)
    	{
    		PreviousScrollTopInPixels = RenderBatch.ViewModel.ScrollTop;
    		shouldCalculateVerticalSlider = true;
	    }
		
    	if (Math.Abs(PreviousTextEditorWidthInPixels - RenderBatch.ViewModel.TextEditorDimensions.Width) >= 0.1)
    	{
    		PreviousTextEditorWidthInPixels = RenderBatch.ViewModel.TextEditorDimensions.Width;
    		shouldCalculateHorizontalSlider = true;
    		shouldCalculateHorizontalScrollbar = true;
	    }
		
    	if (Math.Abs(PreviousScrollWidthInPixels - RenderBatch.ViewModel.ScrollWidth) >= 0.1)
    	{
    		PreviousScrollWidthInPixels = RenderBatch.ViewModel.ScrollWidth;
    		shouldCalculateHorizontalSlider = true;
	    }
		
    	if (Math.Abs(PreviousScrollLeftInPixels - RenderBatch.ViewModel.ScrollLeft) >= 0.1)
    	{
    		PreviousScrollLeftInPixels = RenderBatch.ViewModel.ScrollLeft;
    		shouldCalculateHorizontalSlider = true;
	    }

		if (shouldCalculateVerticalSlider)
			VERTICAL_GetSliderVerticalStyleCss();
		
		if (shouldCalculateHorizontalSlider)
			HORIZONTAL_GetSliderHorizontalStyleCss();
		
		if (shouldCalculateHorizontalScrollbar)
			HORIZONTAL_GetScrollbarHorizontalStyleCss();
    
    	ConstructVirtualizationStyleCssStrings();
    	
    	if (RenderBatch.ViewModel.PersistentState.TooltipViewModel is not null)
		{
			var x = RenderBatch.ViewModel.PersistentState.TooltipViewModel.RelativeCoordinates.RelativeX +
					RenderBatch.ViewModel.PersistentState.TooltipViewModel.RelativeCoordinates.RelativeScrollLeft;
					
			var y = RenderBatch.ViewModel.PersistentState.TooltipViewModel.RelativeCoordinates.RelativeY +
					RenderBatch.ViewModel.PersistentState.TooltipViewModel.RelativeCoordinates.RelativeScrollTop;
		
			if (Math.Abs(ValueTooltipRelativeX - x) >= 0.1)
			{
				ValueTooltipRelativeX = x;
				TooltipRelativeX = x.ToCssValue();
			}
		
			if (Math.Abs(ValueTooltipRelativeY - y) >= 0.1)
			{
				ValueTooltipRelativeY = y;
				TooltipRelativeY = y.ToCssValue();
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
    	if (RenderBatch.ViewModel is null)
    	{
    		return;
    	}
    }
}
