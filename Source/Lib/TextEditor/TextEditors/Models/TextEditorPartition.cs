using Luthetus.TextEditor.RazorLib.Characters.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public struct TextEditorPartition
{
    public TextEditorPartition(List<RichCharacter> richCharacterList)
    {
        RichCharacterList = richCharacterList;
    }

    public List<RichCharacter> RichCharacterList;

    /// <summary>
    /// This is the count of rich characters in THIS particular partition, the <see cref="TextEditorModel.DocumentLength"/>
    /// contains the whole count of rich characters across all partitions.
    /// </summary>
    public int Count => RichCharacterList.Count;

    public TextEditorPartition Insert(
        int relativePositionIndex,
        RichCharacter richCharacter)
    {
    	var copy = new List<RichCharacter>(RichCharacterList);
    	copy.Insert(relativePositionIndex, richCharacter);
    	return new(copy);
    }

    public TextEditorPartition InsertRange(
        int relativePositionIndex,
        IEnumerable<RichCharacter> richCharacterList)
    {
    	var copy = new List<RichCharacter>(RichCharacterList);
    	copy.InsertRange(relativePositionIndex, richCharacterList);
    	return new(copy);
    }

    public TextEditorPartition RemoveAt(int relativePositionIndex)
    {
    	var copy = new List<RichCharacter>(RichCharacterList);
    	copy.RemoveAt(relativePositionIndex);
    	return new(copy);
    }

    public TextEditorPartition RemoveRange(int relativePositionIndex, int count)
    {
    	var copy = new List<RichCharacter>(RichCharacterList);
    	copy.RemoveRange(relativePositionIndex, count);
    	return new(copy);
    }

    public TextEditorPartition AddRange(IEnumerable<RichCharacter> richCharacterList)
    {
        return InsertRange(Count, richCharacterList);
    }

    /// <summary>
    /// If either character or decoration byte are 'null', then the respective
    /// collection will be left unchanged.
    /// 
    /// i.e.: to change ONLY a character value invoke this method with decorationByte set to null,
    ///       and only the <see cref="CharList"/> will be changed.
    /// </summary>
    public TextEditorPartition SetItem(
        int relativePositionIndex,
        RichCharacter richCharacter)
    {
	    var copy = new List<RichCharacter>(RichCharacterList);
    	copy[relativePositionIndex] = richCharacter;
    	return new(copy);
    }
}
