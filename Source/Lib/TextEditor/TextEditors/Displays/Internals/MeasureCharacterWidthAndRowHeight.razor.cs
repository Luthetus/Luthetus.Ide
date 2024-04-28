using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

/// <summary>
/// Goal: better measurement system. (2024-04-21)
/// #Step 100 (2024-04-21)
/// ---------------------------------------------
/// 
/// Proportional fonts (NOT-monospace) is the most clear explanation
/// of the problem currently faced.
/// 
/// With proportional fonts, the cursor is rendered as if the text
/// were monospace.
/// 
/// Then, the editor sees the 'UseMonospaceOptimizations' boolean set to false,
/// so the editor uses JavaScript to determine the true cursor position then re-render.
/// 
/// To re-render the text editor twice everytime that a optimized render would have occurred,
/// is the main issue.
/// 
/// The second issue is with timing. One should know the text is ready to be measured,
/// prior to measuring it.
/// 
/// Currently, there is a bug where the text editor measures the char height and line height,
/// but gets a char height of 0 and a line height of 0, because the text wasn't rendered yet to begin with.
/// 
/// Idea for fix: Use 'ShouldRender()' to delay any attempts for the text editor to re-render.
/// And instead, let the 'MeasureCharacterWidthAndRowHeight' component know that things need to be rendered.
/// 
/// This gives the 'MeasureCharacterWidthAndRowHeight' component a chance to deal with any
/// proportional (non-monospace) font logic. And then have the result ready to be
/// rendered the first time, rather than taking 2 renders.
/// </summary>
public partial class MeasureCharacterWidthAndRowHeight : ComponentBase
{
    [Parameter, EditorRequired]
    public string HtmlElementId { get; set; } = null!;

    private string _testStringForMeasurement = "abcdefghijklmnopqrstuvwxyz0123456789";
    private int _testStringRepeatCount = 6;

    public int CountOfTestCharacters => _testStringRepeatCount * _testStringForMeasurement.Length;
}