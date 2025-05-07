using System.Text;
using System.Diagnostics;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

/// <summary>
/// This type is used to render the text editor's text on the UI.
///
/// A single List of 'VirtualizationEntry' is allocated each time
/// the UI is calculated.
///
/// Each 'VirtualizationEntry' is a representation of each line of text to display.
///
/// The text to display for each line is not stored in the 'VirtualizationEntry'.
///
/// The 'VirtualizationEntry' is only describing the position index span
/// of characters that should be rendered by mapping from the 'TextEditorModel'.
///
/// The 'VirtualizationBoundary's will ensure that the horizontal
/// and vertical scrollbars stay consistent, regardless of how much
/// text is "virtually" not being rendered.
///
/// ===============================================================================
/// 
/// Idea: look in the previous VirtualizationGrid to see if a line of text has already been calculated.
///       If it was already calculated then re-use the previous calculation.
///       Otherwise calculate it.
/// 
/// Remarks: this idea is in reference to a 'partially' changed virtualization grid.
///          Not to be confused with how the code currently will 're-use' the previous virtualization grid
///          if a new virtualization grid was not deemed necessary.
///          |
///          This is re-using the virtualization lines that are common between
///          two separate virtualization grids, that have some overlapping lines.
///
/// i.e.:
/// - Use for scrolling vertically, only calculate the lines that were not previously visible.
/// - Use for editing text, in the case that only 1 line of text is being edited,
///   	this would permit re-use of the other 29 lines (relative to the height of the text editor and font-size;
///       30 lines is what I see currently in mine.).
/// - Given how the UI is written, all 30 lines have to re-drawn yes.
///   	But, we still get to avoid the overhead of 'calculating what is to be drawn'.
///       i.e.: contiguous decoration bytes being grouped in the same '<span>'.
/// 
/// </summary>
public struct VirtualizationGrid
{
	public static VirtualizationGrid Empty { get; } = new(
        new(),
        new List<VirtualizationSpan>(),
        totalWidth: 0,
        totalHeight: 0,
        resultWidth: 0,
        resultHeight: 0,
        left: 0,
        top: 0,
        collapsedLineCount: 0);

	/// <summary>Measurements are in pixels</summary>
    public VirtualizationGrid(
        List<VirtualizationLine> entries,
        List<VirtualizationSpan> virtualizationSpanList,
        double totalWidth,
        double totalHeight,
        double resultWidth,
        double resultHeight,
        double left,
        double top,
        int collapsedLineCount)
    {
        EntryList = entries;
        VirtualizationSpanList = virtualizationSpanList;
        TotalWidth = totalWidth;
        TotalHeight = totalHeight;
        VirtualWidth = resultWidth;
        VirtualHeight = resultHeight;
        VirtualLeft = left;
        VirtualTop = top;
        CollapsedLineCount = collapsedLineCount;
    }

    public List<VirtualizationLine> EntryList { get; init; }
    public List<VirtualizationSpan> VirtualizationSpanList { get; init; }
    
    /// <summary>
    /// Measurements are in pixels.
    ///
    /// Width (including non-rendered elements).
    /// </summary>
    public double TotalWidth { get; init; }
    /// <summary>
    /// Measurements are in pixels
    ///
    /// Height (including non-rendered elements).
    /// </summary>
    public double TotalHeight { get; init; }
    /// <summary>
    /// Measurements are in pixels
    ///
    /// Width (only rendered elements).
    /// </summary>
    public double VirtualWidth { get; init; }
    /// <summary>
    /// Measurements are in pixels
    ///
    /// Height (only rendered elements).
    /// </summary>
    public double VirtualHeight { get; init; }
    /// <summary>
    /// Measurements are in pixels
    ///
    /// Lowest 'left' point where a rendered element is displayed.
    /// </summary>
    public double VirtualLeft { get; init; }
    /// <summary>
    /// Measurements are in pixels
    ///
    /// Lowest 'top' point where a rendered element is displayed.
    /// </summary>
    public double VirtualTop { get; init; }
    
    public int CollapsedLineCount { get; init; }

    /// <summary>
    ///
    /// Do not invoke this method from outside an edit context.
    /// It makes use of a shared instance of a StringBuilder
    /// on the ITextEditorService.
    ///
    /// The memory cost of rendering the text is currently being optimized.
    /// One side effect of this is an increased CPU cost whenever the text is rendered.
    ///
    /// Because, now the data is stored in a more "compact" but "needs to be computed" way.
    ///
    /// The question is whether one could pre-emptively generate
    /// the UI such that a single 'foreach' loop could render it correctly.
    /// 
    /// Another wording: "what can I do here (off the UI thread) prior to the .razor markup"?
    /// Because this method is invoked fom the IBackgroundTaskService.
    ///
    /// =====================================================================================
    ///
    /// This method might be better off being 'static' because 'this' is purposefully not used,
    /// instead accessed through 'viewModel.VirtualizationResult'.
    ///
    /// There is no confirmed reasoning behind this.
    /// It is just that modifying a List of structs is giving
    /// me the heebie-jeebies.
    ///
    /// I want to make sure the List is written over.
    /// I have not tested if referencing the properties through an implicit/explicit 'this' keyword
    /// would correctly update the List that the struct is contained in.
    /// </summary>
    public void CreateCache(ITextEditorService textEditorService, TextEditorModel model, TextEditorViewModel viewModel)
    {
    	#if DEBUG
    	var startTime = Stopwatch.GetTimestamp();
    	#endif
    	
    	viewModel.CreateCacheWasInvoked = true;
    
    	if (viewModel.VirtualizationResult.EntryList.Count == 0)
			return;
			
		/*
	    Dictionary<int, VirtualizationLine> VirtualizedLineCacheEntryMap;
	    int VirtualizedLineCacheCreatedWithScrollLeft;
	    List<VirtualizationSpan> VirtualizedLineCacheSpanList;
	    HashSet<int> VirtualizedLineCacheUsageHashSet;
	    List<int> VirtualizedLineIndexKeyList;
	    List<int> VirtualizedLineLineIndexWithModificationList;
		*/
		
		var componentData = viewModel.DisplayTracker.ComponentData;
		
		var absDiffScrollLeft = Math.Abs(componentData.VirtualizedLineCacheCreatedWithScrollLeft - viewModel.ScrollbarDimensions.ScrollLeft);
		var useAll = absDiffScrollLeft < 0.01;
		Console.Write($"useAll: {useAll}; ");
		
		var tabKeyOutput = "&nbsp;&nbsp;&nbsp;&nbsp;";
	    var spaceKeyOutput = "&nbsp;";
	    
	    var reUsedLines = 0;

		if (textEditorService.OptionsApi.GetTextEditorOptionsState().Options.ShowWhitespace)
	    {
	        tabKeyOutput = "--->";
	        spaceKeyOutput = "·";
	    }
		
		textEditorService.__StringBuilder.Clear();
		
		for (int entryIndex = 0; entryIndex < viewModel.VirtualizationResult.EntryList.Count; entryIndex++)
		{
			var virtualizationEntry = viewModel.VirtualizationResult.EntryList[entryIndex];
			
			if (virtualizationEntry.Position_EndExclusiveIndex - virtualizationEntry.Position_StartInclusiveIndex <= 0)
				continue;
			
			(int lineIndex, int columnIndex) lineAndColumnIndices = (0, 0);
			var inlineUi = new InlineUi(0, InlineUiKind.None);
			
			foreach (var inlineUiTuple in viewModel.InlineUiList)
			{
				lineAndColumnIndices = model.GetLineAndColumnIndicesFromPositionIndex(inlineUiTuple.InlineUi.PositionIndex);
				
				if (lineAndColumnIndices.lineIndex == virtualizationEntry.LineIndex)
					inlineUi = inlineUiTuple.InlineUi;
			}
			
			virtualizationEntry.VirtualizationSpan_StartInclusiveIndex = viewModel.VirtualizationResult.VirtualizationSpanList.Count;
			
			if (useAll && inlineUi.InlineUiKind == InlineUiKind.None)
			{
				var useThis = componentData.VirtualizedLineCacheEntryMap.ContainsKey(virtualizationEntry.LineIndex) &&
							  !componentData.VirtualizedLineLineIndexWithModificationList.Contains(virtualizationEntry.LineIndex);
				
				if (useThis)
				{
					var previous = componentData.VirtualizedLineCacheEntryMap[virtualizationEntry.LineIndex];
				
					for (int i = previous.VirtualizationSpan_StartInclusiveIndex; i < previous.VirtualizationSpan_EndExclusiveIndex; i++)
					{
						viewModel.VirtualizationResult.VirtualizationSpanList.Add(componentData.VirtualizedLineCacheSpanList[i]);
					}
					
					// WARNING CODE DUPLICATION (this also exists at the bottom of this for loop).
					virtualizationEntry.VirtualizationSpan_EndExclusiveIndex = viewModel.VirtualizationResult.VirtualizationSpanList.Count;
					viewModel.VirtualizationResult.EntryList[entryIndex] = virtualizationEntry;
					
					componentData.VirtualizedLineCacheEntryMap[virtualizationEntry.LineIndex] = virtualizationEntry;
					
					reUsedLines++;
					
					continue;
				}
			}
			
			var currentDecorationByte = model.RichCharacterList[virtualizationEntry.Position_StartInclusiveIndex].DecorationByte;
		    
		    for (int i = virtualizationEntry.Position_StartInclusiveIndex; i < virtualizationEntry.Position_EndExclusiveIndex; i++)
		    {
		    	if (inlineUi.InlineUiKind != InlineUiKind.None && inlineUi.PositionIndex == i)
		    	{
		    		textEditorService.__StringBuilder.Append("&nbsp;&nbsp;&nbsp;");
		    	}
		    	
		    	var richCharacter = model.RichCharacterList[i];
		    	 
				if (currentDecorationByte == richCharacter.DecorationByte)
			    {
			        // AppendTextEscaped(textEditorService.__StringBuilder, richCharacter, tabKeyOutput, spaceKeyOutput);
			        switch (richCharacter.Value)
			        {
			            case '\t':
			                textEditorService.__StringBuilder.Append(tabKeyOutput);
			                break;
			            case ' ':
			                textEditorService.__StringBuilder.Append(spaceKeyOutput);
			                break;
			            case '\r':
			                break;
			            case '\n':
			                break;
			            case '<':
			                textEditorService.__StringBuilder.Append("&lt;");
			                break;
			            case '>':
			                textEditorService.__StringBuilder.Append("&gt;");
			                break;
			            case '"':
			                textEditorService.__StringBuilder.Append("&quot;");
			                break;
			            case '\'':
			                textEditorService.__StringBuilder.Append("&#39;");
			                break;
			            case '&':
			                textEditorService.__StringBuilder.Append("&amp;");
			                break;
			            default:
			                textEditorService.__StringBuilder.Append(richCharacter.Value);
			                break;
			        }
			        // END OF INLINING AppendTextEscaped
			    }
			    else
			    {
			    	viewModel.VirtualizationResult.VirtualizationSpanList.Add(new VirtualizationSpan(
			    		cssClass: model.DecorationMapper.Map(currentDecorationByte),
			    		text: textEditorService.__StringBuilder.ToString()));
			        textEditorService.__StringBuilder.Clear();
			        
			        // AppendTextEscaped(textEditorService.__StringBuilder, richCharacter, tabKeyOutput, spaceKeyOutput);
			        switch (richCharacter.Value)
			        {
			            case '\t':
			                textEditorService.__StringBuilder.Append(tabKeyOutput);
			                break;
			            case ' ':
			                textEditorService.__StringBuilder.Append(spaceKeyOutput);
			                break;
			            case '\r':
			                break;
			            case '\n':
			                break;
			            case '<':
			                textEditorService.__StringBuilder.Append("&lt;");
			                break;
			            case '>':
			                textEditorService.__StringBuilder.Append("&gt;");
			                break;
			            case '"':
			                textEditorService.__StringBuilder.Append("&quot;");
			                break;
			            case '\'':
			                textEditorService.__StringBuilder.Append("&#39;");
			                break;
			            case '&':
			                textEditorService.__StringBuilder.Append("&amp;");
			                break;
			            default:
			                textEditorService.__StringBuilder.Append(richCharacter.Value);
			                break;
			        }
			        // END OF INLINING AppendTextEscaped
			        
					currentDecorationByte = richCharacter.DecorationByte;
			    }
		    }
		    
			/* Final grouping of contiguous characters */
			viewModel.VirtualizationResult.VirtualizationSpanList.Add(new VirtualizationSpan(
	    		cssClass: model.DecorationMapper.Map(currentDecorationByte),
	    		text: textEditorService.__StringBuilder.ToString()));
			textEditorService.__StringBuilder.Clear();
			
			// WARNING CODE DUPLICATION (this also exists when copying a virtualizationEntry from cache).
			virtualizationEntry.VirtualizationSpan_EndExclusiveIndex = viewModel.VirtualizationResult.VirtualizationSpanList.Count;
			viewModel.VirtualizationResult.EntryList[entryIndex] = virtualizationEntry;
			
			if (componentData.VirtualizedLineCacheEntryMap.ContainsKey(virtualizationEntry.LineIndex))
			{
				componentData.VirtualizedLineCacheEntryMap[virtualizationEntry.LineIndex] = virtualizationEntry;
			}
			else
			{
				componentData.VirtualizedLineCacheEntryMap.Add(virtualizationEntry.LineIndex, virtualizationEntry);
			}
		}
		
		componentData.VirtualizedLineCacheViewModelKey = viewModel.ViewModelKey;
		componentData.VirtualizedLineCacheSpanList = viewModel.VirtualizationResult.VirtualizationSpanList;
		componentData.VirtualizedLineCacheCreatedWithScrollLeft = viewModel.ScrollbarDimensions.ScrollLeft;
		
		Console.WriteLine($"reUsedLines: {reUsedLines}");
		
		#if DEBUG
		LuthetusDebugSomething.SetTextEditorVirtualizationGrid(Stopwatch.GetElapsedTime(startTime));
		#endif
    }
    
    /// <summary>
    /// Inlining this instead of invoking the function definition just to see what happens.
    /// </summary>
    /*private void AppendTextEscaped(
        StringBuilder spanBuilder,
        RichCharacter richCharacter,
        string tabKeyOutput,
        string spaceKeyOutput)
    {
        switch (richCharacter.Value)
        {
            case '\t':
                spanBuilder.Append(tabKeyOutput);
                break;
            case ' ':
                spanBuilder.Append(spaceKeyOutput);
                break;
            case '\r':
                break;
            case '\n':
                break;
            case '<':
                spanBuilder.Append("&lt;");
                break;
            case '>':
                spanBuilder.Append("&gt;");
                break;
            case '"':
                spanBuilder.Append("&quot;");
                break;
            case '\'':
                spanBuilder.Append("&#39;");
                break;
            case '&':
                spanBuilder.Append("&amp;");
                break;
            default:
                spanBuilder.Append(richCharacter.Value);
                break;
        }
    }*/
}