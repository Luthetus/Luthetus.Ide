namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// InlineUiKind.None indicates the default value.
/// </summary>
public struct InlineUi
{
	public InlineUi(int positionIndex, InlineUiKind inlineUiKind)
	{
		PositionIndex = positionIndex;
		InlineUiKind = inlineUiKind;
	}

    public int PositionIndex;
    public InlineUiKind InlineUiKind;
    
    public InlineUi WithIncrementPositionIndex(int amount)
    {
    	PositionIndex += amount;
    	return this;
    }
    
    public InlineUi WithDecrementPositionIndex(int amount)
    {
    	PositionIndex -= amount;
    	return this;
    }
}
