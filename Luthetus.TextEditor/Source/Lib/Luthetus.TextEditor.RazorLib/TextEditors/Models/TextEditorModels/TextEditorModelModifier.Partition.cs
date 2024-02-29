using Luthetus.TextEditor.RazorLib.Characters.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModelModifier
{
    // (2024-02-29) Plan to add text editor partitioning #Step 1,000:
    // --------------------------------------------------
    // The 'PartitionList_...(...)' methods are currently un-implemented.
    //
    // So, I need to decide how to implement these methods.
    //
    // I'm going to make a unit test.
    public void PartitionList_Add(RichCharacter richCharacter)
    {
        PartitionList_Insert(DocumentLength, richCharacter);

        //int indexOfPartitionWithAvailableSpace = -1;
        //
        //for (int i = 0; i < PartitionList.Count; i++)
        //{
        //    ImmutableList<RichCharacter>? partition = PartitionList[i];
        //
        //    // (2024-02-29) Plan to add text editor partitioning #Step 1,100:
        //    // --------------------------------------------------
        //    // I am encountering an issue here. What is the partition size?
        //    //
        //    // I can go define that constant now. I'll make it 5,000.
        //    // In 'TextEditorModel.Variables.cs' I added: 'public const int PARTITION_SIZE = 5_000;'
        //    //
        //    // When comparing partition.Count and TextEditorModel.PARTITION_SIZE should
        //    // '==' operator be used or ">="?
        //    //
        //    // In an ideal world partition.Count will always be constrained by the
        //    // TextEditorModel.PARTITION_SIZE constant.
        //    //
        //    // But, an anxious voice tells me to put ">=" to be safe.
        //    // I'm not sure the correct answer to this question.
        //    //
        //    // I presume the ">=" instead of "==" is a negligible performance penalty, if any.
        //    // Whereas mentally, for me its a massive anxiety relief, but I don't know.
        //    if (partition.Count >= TextEditorModel.PARTITION_SIZE)
        //    {
        //        // (2024-02-29) Plan to add text editor partitioning #Step 1,100:
        //        // --------------------------------------------------
        //        // I'm still writing the 'PartitionList_Add(...)' method,
        //        // but I think it'd be best to not write the 'PartitionList_Add(...)' out.
        //        //
        //        // Instead, I could invoke 'PartitionList_Insert(...)', where the
        //        // index to insert at would be the global-count of all rich characters.
        //        //
        //        // I'm going to do this.
        //    }
        //}
    }
    
    public void PartitionList_Insert(int globalPositionIndex, RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            ImmutableList<RichCharacter>? partition = _partitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                // But, we must first check if it has available space.
                if (partition.Count >= TextEditorModel.PARTITION_SIZE)
                {
                    indexOfPartitionWithAvailableSpace = i + 1;
                    relativePositionIndex = 0;
                    PartitionList_InsertNewPartition(indexOfPartitionWithAvailableSpace);
                    break;
                    // (2024-02-29) Plan to add text editor partitioning #Step 1,200:
                    // --------------------------------------------------
                    // Here, I inserted a new partition, because the current partition did not have available space.
                    // But, what if there were an existing 'next' partition, which had available space?
                }

                relativePositionIndex = partition.Count - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new ApplicationException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new ApplicationException("if (relativePositionIndex == -1)");

        _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            _partitionList[indexOfPartitionWithAvailableSpace].Insert(relativePositionIndex, richCharacter));
    }

    public void PartitionList_InsertRange(int globalPositionIndex, IEnumerable<RichCharacter> richCharacterList)
    {
        var offsetIndex = 0;

        foreach (var richCharacter in richCharacterList)
        {
            PartitionList_Insert(globalPositionIndex + offsetIndex, richCharacter);
        }
    }
    
    public void PartitionList_RemoveAt(int globalPositionIndex)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            ImmutableList<RichCharacter>? partition = _partitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = partition.Count - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new ApplicationException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new ApplicationException("if (relativePositionIndex == -1)");

        _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            _partitionList[indexOfPartitionWithAvailableSpace].RemoveAt(relativePositionIndex));
    }
    
    public void PartitionList_RemoveRange(int globalPositionIndex, int count)
    {
        for (int i = 0; i < count; i++)
        {
            PartitionList_RemoveAt(globalPositionIndex);
        }
    }

    private void PartitionList_InsertNewPartition(int partitionIndex)
    {
        _partitionList = _partitionList.Insert(partitionIndex, ImmutableList<RichCharacter>.Empty);
    }
}