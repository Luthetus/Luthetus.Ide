using Luthetus.TextEditor.RazorLib.Characters.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorPartition
{
    public TextEditorPartition(ImmutableList<RichCharacter> richCharacterList)
    {
        RichCharacterList = richCharacterList;
    }

    public ImmutableList<RichCharacter> RichCharacterList { get; }

    /// <summary>
    /// This is the count of rich characters in THIS particular partition, the <see cref="TextEditorModel.DocumentLength"/>
    /// contains the whole count of rich characters across all partitions.
    /// </summary>
    public int Count => RichCharacterList.Count;

    public TextEditorPartition Insert(
        int relativePositionIndex,
        RichCharacter richCharacter)
    {
        return new TextEditorPartition(RichCharacterList.Insert(relativePositionIndex, richCharacter));
    }

    public TextEditorPartition InsertRange(
        int relativePositionIndex,
        IEnumerable<RichCharacter> richCharacterList)
    {
        return new TextEditorPartition(RichCharacterList.InsertRange(relativePositionIndex, richCharacterList));
    }

    public TextEditorPartition RemoveAt(int relativePositionIndex)
    {
        return new TextEditorPartition(RichCharacterList.RemoveAt(relativePositionIndex));
    }

    public TextEditorPartition RemoveRange(int relativePositionIndex, int count)
    {
        return new TextEditorPartition(RichCharacterList.RemoveRange(relativePositionIndex, count));
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
        return new TextEditorPartition(RichCharacterList.SetItem(relativePositionIndex, richCharacter));
    }
}
