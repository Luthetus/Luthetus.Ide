using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Displays;

public partial class TreeViewFindAllTextSpanDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewFindAllTextSpan TreeViewFindAllTextSpan { get; set; } = null!;
	
	protected override void OnInitialized()
	{
		CalculatePreviewNearbyText();
		base.OnInitialized();
	}
	
	private void CalculatePreviewNearbyText()
	{
		var localTreeView = TreeViewFindAllTextSpan;
		
		if (localTreeView.PreviewEarlierNearbyText is not null &&
			localTreeView.PreviewLaterNearbyText is not null)
		{
			return;
		}
		
		var distanceOffset = 15;
		
		var startingIndexInclusive = localTreeView.Item.StartingIndexInclusive - distanceOffset;
		startingIndexInclusive = Math.Max(0, startingIndexInclusive);
		
    	var endingIndexExclusive = localTreeView.Item.EndingIndexExclusive + distanceOffset;
  	  endingIndexExclusive = Math.Min(localTreeView.Item.SourceText.Length, endingIndexExclusive);
    	
    	var earlierTextSpan = new TextEditorTextSpan(
		    startingIndexInclusive,
		    startingIndexInclusive + localTreeView.Item.StartingIndexInclusive - startingIndexInclusive,
		    0,
		    localTreeView.Item.ResourceUri,
		    localTreeView.Item.SourceText);
		localTreeView.PreviewEarlierNearbyText = earlierTextSpan.GetText();
		
		var laterTextSpan = new TextEditorTextSpan(
		    localTreeView.Item.EndingIndexExclusive,
		    localTreeView.Item.EndingIndexExclusive + endingIndexExclusive - localTreeView.Item.EndingIndexExclusive,
		    0,
		    localTreeView.Item.ResourceUri,
		    localTreeView.Item.SourceText);
		localTreeView.PreviewLaterNearbyText = laterTextSpan.GetText();
	}
}