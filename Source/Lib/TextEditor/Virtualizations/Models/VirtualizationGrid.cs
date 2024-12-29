using System.Text;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

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
/// </summary>
public record struct VirtualizationGrid
{
	public static VirtualizationGrid Empty { get; } = new(
        Array.Empty<VirtualizationLine>(),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0),
        new VirtualizationBoundary(0, 0, 0, 0));

    public VirtualizationGrid(
        VirtualizationLine[] entries,
        VirtualizationBoundary leftVirtualizationBoundary,
        VirtualizationBoundary rightVirtualizationBoundary,
        VirtualizationBoundary topVirtualizationBoundary,
        VirtualizationBoundary bottomVirtualizationBoundary)
    {
        EntryList = entries;
        LeftVirtualizationBoundary = leftVirtualizationBoundary;
        RightVirtualizationBoundary = rightVirtualizationBoundary;
        TopVirtualizationBoundary = topVirtualizationBoundary;
        BottomVirtualizationBoundary = bottomVirtualizationBoundary;
    }

    public VirtualizationLine[] EntryList { get; init; }
    
    public VirtualizationBoundary LeftVirtualizationBoundary { get; init; }
    public VirtualizationBoundary RightVirtualizationBoundary { get; init; }
    public VirtualizationBoundary TopVirtualizationBoundary { get; init; }
    public VirtualizationBoundary BottomVirtualizationBoundary { get; init; }

    /// <summary>
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
    /// </summary>
    public void CreateCache(ITextEditorService textEditorService, ITextEditorModel model, TextEditorViewModel viewModel)
    {
    	if (viewModel.VirtualizationResult.EntryList.Length == 0)
			return;
		
		var tabKeyOutput = "&nbsp;&nbsp;&nbsp;&nbsp;";
	    var spaceKeyOutput = "&nbsp;";
			    
		if (textEditorService.OptionsStateWrap.Value.Options.ShowWhitespace)
	    {
	        tabKeyOutput = "--->";
	        spaceKeyOutput = "·";
	    }
		
		var spanBuilder = new StringBuilder();
		var currentDecorationByte = (byte)0;
		
    	var aaa_OUTER_LOOP_TimeElapsed = TimeSpan.Zero;
    	var aaa_OUTER_LOOP_START_DATE_TIME = DateTime.UtcNow;
	
		var aaa_INNER_LOOP_TimeElapsed = TimeSpan.Zero;
		var aaa_INNER_LOOP_START_DATE_TIME = DateTime.UtcNow;
	
		for (int entryIndex = 0; entryIndex < viewModel.VirtualizationResult.EntryList.Length; entryIndex++)
		{
			var virtualizationEntry = viewModel.VirtualizationResult.EntryList[entryIndex];
			
			if (virtualizationEntry.PositionIndexExclusiveEnd - virtualizationEntry.PositionIndexInclusiveStart <= 0)
				continue;
				
			if (virtualizationEntry.VirtualizationSpanList is not null)
				continue;
			
			// Avoid allocating the List by having it nullable reference?
			// i.e.: only allocate it if there is text that needs to be rendered on that line.
			virtualizationEntry.VirtualizationSpanList = new();
				
			byte? currentDecorationByte = null;
			
			{
		    	aaa_OUTER_LOOP_TimeElapsed += DateTime.UtcNow - aaa_OUTER_LOOP_START_DATE_TIME;
		    }
		    
		    {
		    	aaa_INNER_LOOP_START_DATE_TIME = DateTime.UtcNow;
		    }
		    
		    // WARNING: Making this foreach loop into a for loop causes it to run 300 to 500 times slower.
		    //          Presumably this is due to cache misses?
		    foreach (var richCharacter in model.RichCharacterList
		    		 	.Skip(virtualizationEntry.PositionIndexInclusiveStart)
		    			 .Take(virtualizationEntry.PositionIndexExclusiveEnd - virtualizationEntry.PositionIndexInclusiveStart))
		    {
		    	// var richCharacter = model.RichCharacterList[positionIndex];
				
				if ((currentDecorationByte ??= richCharacter.DecorationByte) == richCharacter.DecorationByte)
			    {
			        AppendTextEscaped(spanBuilder, richCharacter, tabKeyOutput, spaceKeyOutput);
			    }
			    else
			    {
			    	virtualizationEntry.VirtualizationSpanList.Add(new VirtualizationSpan(
			    		cssClass: model.DecorationMapper.Map(currentDecorationByte),
			    		text: spanBuilder.ToString()));
			        spanBuilder.Clear();
			        
			        AppendTextEscaped(spanBuilder, richCharacter, tabKeyOutput, spaceKeyOutput);
					currentDecorationByte = richCharacter.DecorationByte;
			    }

			    {
			    	aaa_INNER_LOOP_TimeElapsed += DateTime.UtcNow - aaa_INNER_LOOP_START_DATE_TIME;
			    	aaa_INNER_LOOP_START_DATE_TIME = DateTime.UtcNow;
			    }
		    }
		    
			{
				/* Final grouping of contiguous characters */
				virtualizationEntry.VirtualizationSpanList.Add(new VirtualizationSpan(
		    		cssClass: model.DecorationMapper.Map(currentDecorationByte),
		    		text: spanBuilder.ToString()));
				spanBuilder.Clear();
				currentDecorationByte = null;
			}
			
			{
				aaa_OUTER_LOOP_START_DATE_TIME = DateTime.UtcNow;
		    }
		    
			viewModel.VirtualizationResult.EntryList[entryIndex] = virtualizationEntry;
			
			{
		    	aaa_OUTER_LOOP_TimeElapsed += DateTime.UtcNow - aaa_OUTER_LOOP_START_DATE_TIME;
		  		aaa_OUTER_LOOP_START_DATE_TIME = DateTime.UtcNow;  	
		    }
		}
		
		Console.Write($", o{aaa_OUTER_LOOP_TimeElapsed.TotalMilliseconds}");
		
		Console.Write($", i{aaa_INNER_LOOP_TimeElapsed.TotalMilliseconds}");
		
		Console.WriteLine($")ms, ");
    }
    
    private void AppendTextEscaped(
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
    }
}