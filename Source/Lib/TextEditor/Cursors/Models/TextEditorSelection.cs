namespace Luthetus.TextEditor.RazorLib.Cursors.Models;

/// <summary>
/// To select the first character in a TextEditor one would
/// set <see cref="AnchorPositionIndex" /> = 0 and
/// set <see cref="EndingPositionIndex" /> = 1
/// <br/><br/>
/// The <see cref="AnchorPositionIndex" /> does not select any text by itself.
/// One must visualize that the user's cursor is rendered with <see cref="TextCursorKindFacts.BeamCssClassString" /> mode.
/// <br/><br/>
/// The <see cref="EndingPositionIndex" /> can then be set.
/// If <see cref="EndingPositionIndex" /> is less than <see cref="AnchorPositionIndex" />
/// then <see cref="EndingPositionIndex" /> will be INCLUSIVE in respect to
/// selecting the character at that PositionIndex and <see cref="AnchorPositionIndex" /> will be EXCLUSIVE.
/// <br/><br/>
/// If <see cref="EndingPositionIndex" /> is greater than <see cref="AnchorPositionIndex" /> then
/// <see cref="EndingPositionIndex" /> will be EXCLUSIVE in respect to
/// selecting the character at that PositionIndex and <see cref="AnchorPositionIndex" /> will be INCLUSIVE.
/// <br/><br/>
/// If <see cref="EndingPositionIndex" /> is equal to <see cref="AnchorPositionIndex" /> then
/// no selection is active.
/// <br/><br/>
/// If <see cref="AnchorPositionIndex" /> is -1 then
/// no selection is active.
/// </summary>
public struct TextEditorSelection
{
	public TextEditorSelection(
		int anchorPositionIndex,
	    int endingPositionIndex)
	{
		AnchorPositionIndex = anchorPositionIndex;
		EndingPositionIndex = endingPositionIndex;
	}

    public static readonly TextEditorSelection Empty = new TextEditorSelection(-1, 0);
    
    public int AnchorPositionIndex;
    public int EndingPositionIndex;
}
