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
    
    public void IncrementPositionIndex(int amount)
    {
    	PositionIndex += amount;
    }
    
    public void DecrementPositionIndex(int amount)
    {
    	PositionIndex -= amount;
    }
}
