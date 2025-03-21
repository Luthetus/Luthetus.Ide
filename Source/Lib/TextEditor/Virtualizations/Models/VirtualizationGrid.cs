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
public record VirtualizationGrid
{
	public static VirtualizationGrid Empty { get; } = new(
        Array.Empty<VirtualizationLine>(),
        new List<VirtualizationSpan>(),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0));

    public VirtualizationGrid(
        VirtualizationLine[] entries,
        List<VirtualizationSpan> virtualizationSpanList,
        VirtualizationBoundary leftVirtualizationBoundary,
        VirtualizationBoundary rightVirtualizationBoundary,
        VirtualizationBoundary topVirtualizationBoundary,
        VirtualizationBoundary bottomVirtualizationBoundary)
    {
        EntryList = entries;
        VirtualizationSpanList = virtualizationSpanList;
        LeftVirtualizationBoundary = leftVirtualizationBoundary;
        RightVirtualizationBoundary = rightVirtualizationBoundary;
        TopVirtualizationBoundary = topVirtualizationBoundary;
        BottomVirtualizationBoundary = bottomVirtualizationBoundary;
    }

    public VirtualizationLine[] EntryList { get; init; }
    public List<VirtualizationSpan> VirtualizationSpanList { get; init; }
    
    public VirtualizationBoundary LeftVirtualizationBoundary { get; init; }
    public VirtualizationBoundary RightVirtualizationBoundary { get; init; }
    public VirtualizationBoundary TopVirtualizationBoundary { get; init; }
    public VirtualizationBoundary BottomVirtualizationBoundary { get; init; }

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
    public void CreateCache(ITextEditorService textEditorService, ITextEditorModel model, TextEditorViewModel viewModel)
    {
    	#if DEBUG
    	var startTime = Stopwatch.GetTimestamp();
    	#endif
    
    	if (viewModel.VirtualizationResult.EntryList.Length == 0)
			return;
		
		var tabKeyOutput = "&nbsp;&nbsp;&nbsp;&nbsp;";
	    var spaceKeyOutput = "&nbsp;";

		if (textEditorService.OptionsApi.GetTextEditorOptionsState().Options.ShowWhitespace)
	    {
	        tabKeyOutput = "--->";
	        spaceKeyOutput = "·";
	    }
		
		textEditorService.__StringBuilder.Clear();
		
		for (int entryIndex = 0; entryIndex < viewModel.VirtualizationResult.EntryList.Length; entryIndex++)
		{
			var virtualizationEntry = viewModel.VirtualizationResult.EntryList[entryIndex];
			
			if (virtualizationEntry.PositionIndexExclusiveEnd - virtualizationEntry.PositionIndexInclusiveStart <= 0)
				continue;
				
			virtualizationEntry.VirtualizationSpanIndexInclusiveStart = viewModel.VirtualizationResult.VirtualizationSpanList.Count;
			
			var currentDecorationByte = model.RichCharacterList[virtualizationEntry.PositionIndexInclusiveStart].DecorationByte;
		    
		    for (int i = virtualizationEntry.PositionIndexInclusiveStart; i < virtualizationEntry.PositionIndexExclusiveEnd; i++)
		    {
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
			
			virtualizationEntry.VirtualizationSpanIndexExclusiveEnd = viewModel.VirtualizationResult.VirtualizationSpanList.Count;
		    
			viewModel.VirtualizationResult.EntryList[entryIndex] = virtualizationEntry;
		}
		
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