using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

public class TextEditorContent : ITextEditorContent 
{
    public ImmutableList<TextEditorPartition> PartitionList { get; set; }

    public int CharacterCount { get; set; }
    public IReadOnlyList<int> TabList { get; set; } = ImmutableList<int>.Empty;
    public IReadOnlyList<RowEnding> RowEndingList { get; set; } = ImmutableList<RowEnding>.Empty;
    public IReadOnlyList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountList { get; set; } = ImmutableList<(RowEndingKind rowEndingKind, int count)>.Empty;
    public RowEndingKind? OnlyRowEndingKind { get; set; }
    public int RowCount => RowEndingList.Count;
    public int DocumentLength => CharacterCount;

    public IReadOnlyList<RichCharacter> AllRichCharacters => PartitionList.SelectMany(x => x.RichCharacterList).ToList();

    public RichCharacter GetRichCharacterAt(int globalPositionIndex)
    {
        var runningCount = 0;
        for (int i = 0; i < PartitionList.Count; i++)
        {
            var partitionCount = PartitionList[i].CharacterCount;
            if (runningCount + partitionCount > globalPositionIndex)
            {
                var partition = PartitionList[i];
                return partition.RichCharacterList[globalPositionIndex - runningCount];
            }
            else
            {
                runningCount += partitionCount;
            }
        }
        throw new IndexOutOfRangeException();
    }

    public void Insert(int globalPositionIndex, RichCharacter richCharacter)
    {

    }
    
    public void InsertRange(int globalPositionIndex, IEnumerable<RichCharacter> richCharacterList)
    {

    }

    public void Clear()
    {

    }
}