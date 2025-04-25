namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public struct InlineUi
{
	public InlineUi(int positionIndex, InlineUiKind inlineUiKind)
	{
		PositionIndex = positionIndex;
		InlineUiKind = inlineUiKind;
	}

    public int PositionIndex { get; private set; }
    public InlineUiKind InlineUiKind { get; }
    
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
