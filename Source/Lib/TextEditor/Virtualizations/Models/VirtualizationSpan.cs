using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

/// <summary>
/// Currently, the UI is looping over all the 'RichCharacter'(s)
/// that are in the VirtualizationGrid.
///
/// It does this to combine 'RichCharacter'(s)
/// that have the same 'DecorationByte'
/// into the same 'StringBuilder'
/// so the text can be written all in the same '<span class="shared-decoration">@stringBuilder</span>'.
///
/// Even if the VirtualizationGrid does not change though, this combining of 'RichCharacter'(s) is
/// repeated even if it was already calculated the last time the text editor rendered.
///
/// So, the goal here is to do the combining and then cache the result,
/// as well to do this combining logic off the UI thread.
/// </summary>
public struct VirtualizationSpan
{
	public VirtualizationSpan(string cssClass, string text)
	{
		CssClass = cssClass;
		MarkupStringText = new MarkupString(text);
	}
	
	public string CssClass;
	public MarkupString MarkupStringText;
}
