using Luthetus.TextEditor.RazorLib.Characters.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModelModifier
{
    public void PartitionList_Insert(int globalPositionIndex, RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            ImmutableList<RichCharacter>? partition = _partitionList[i];

            if (runningCount + partition.Count >= globalPositionIndex)
            {
                // This is the partition we want to modify.
                // But, we must first check if it has available space.
                if (partition.Count >= PartitionSize)
                {
                    // (2024-02-29) Plan to add text editor partitioning #Step 1,700:
                    // --------------------------------------------------
                    // If the next partition has space available, then insert onto it,
                    // as opposed to constructing a new partition.

                    PartitionList_SplitIntoTwoPartitions(i);
                    i--;
                    continue;
                    // (2024-02-29) Plan to add text editor partitioning #Step 1,400:
                    // --------------------------------------------------
                    // The method 'PartitionList_SplitIntoTwoPartitions(...)' is presumed
                    // to result in the partition at index 'i' to have available space.
                    //
                    // If the PARTITION_SIZE were '1', then
                    // The first partition would be repopulated with the same content it original had.
                    // This is because 'PartitionList_SplitIntoTwoPartitions(...)' currently
                    // performs an even split into 2, then adds the remainder to the first partition.
                    // So a PARTITION_SIZE is an even split of '0', and a remainder of '1'.
                    //
                    // For this reason, I am going to add code to the constructor, where
                    // if the (PARTITION_SIZE < 2) I will
                    // 'throw new ApplicationException($"{nameof(PARTITION_SIZE)} must be >= 2")'
                }

                relativePositionIndex = globalPositionIndex - runningCount;
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

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            _partitionList[indexOfPartitionWithAvailableSpace].Insert(relativePositionIndex, richCharacter));
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
                relativePositionIndex = globalPositionIndex - runningCount;
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

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            _partitionList[indexOfPartitionWithAvailableSpace].RemoveAt(relativePositionIndex));
    }

    private void PartitionList_InsertNewPartition(int partitionIndex)
    {
        _partitionList = _partitionList.Insert(partitionIndex, ImmutableList<RichCharacter>.Empty);
    }
    
    private void PartitionList_SplitIntoTwoPartitions(int partitionIndex)
    {
        // (2024-02-29) Plan to add text editor partitioning #Step 1,400:
        // --------------------------------------------------
        // This method will change a PartitionList of:
        // PartitionList.SetPartitionSize(5);
        // # {
        // #     [ 'H', 'e', 'l', 'l', 'o' ]
        // # }
        //
        // To a partitionList of:
        // PartitionList.SetPartitionSize(5);
        // # {
        // #     [ 'H', 'e', 'l' ],
        // #     [ 'l', 'o' ]
        // # }

        var originalPartition = _partitionList[partitionIndex];

        var firstUnevenSplit = PartitionSize / 2 + (PartitionSize % 2);
        var secondUnevenSplit = PartitionSize / 2;

        _partitionList = _partitionList.SetItem(
            partitionIndex,
            ImmutableList<RichCharacter>.Empty.AddRange(originalPartition.Take(firstUnevenSplit)));

        _partitionList = _partitionList.Insert(
            partitionIndex + 1,
            ImmutableList<RichCharacter>.Empty.AddRange(originalPartition.Skip(firstUnevenSplit).Take(secondUnevenSplit)));
    }

    public void PartitionList_InsertRange(int globalPositionIndex, IEnumerable<RichCharacter> richCharacterList)
    {
        // (2024-02-29) Plan to add text editor partitioning #Step 1,700:
        // --------------------------------------------------
        // It is taking 11 seconds to construct a text editor with 11,000 characters in it.
        //
        // I tracked down the cause to this method. For, I commented out the part were
        // the content is inserted, and it went from taking 11 seconds to 14 miliseconds.
        //
        // Goal: Determine how much space is left in a given partition.
        //
        //       Then, do a bulk insertion of the 'richCharacterList'.
        //           Where one takes up to the remaining size of the partition amount of entries.
        //
        //       If there are still more RichCharacter(s) to write,
        //           then determine if the next partition has space that can be written to.
        //       If not then insert a new partition.
        //
        //       Rinse and repeat these steps until all the RichCharacter(s) have been written out.
        //
        // (2024-02-29) Plan to add text editor partitioning #Step 1,700:
        // --------------------------------------------------
        // I'm going to start by copying and pasting the code from PartitionList_Insert(...)
        // to this method, and then wrap that code in a while loop.

        var richCharacterEnumerator = richCharacterList.GetEnumerator();

        while (richCharacterEnumerator.MoveNext())
        {
            int indexOfPartitionWithAvailableSpace = -1;
            int relativePositionIndex = -1;
            var runningCount = 0;
            ImmutableList<RichCharacter>? partition;

            for (int i = 0; i < _partitionList.Count; i++)
            {
                partition = _partitionList[i];

                if (runningCount + partition.Count >= globalPositionIndex)
                {
                    if (partition.Count >= PartitionSize)
                    {
                        // (2024-02-29) Plan to add text editor partitioning #Step 1,700:
                        // --------------------------------------------------
                        // If the next partition has space available, then insert onto it,
                        // as opposed to constructing a new partition.
                        PartitionList_SplitIntoTwoPartitions(i);
                        i--;
                        continue;
                    }

                    relativePositionIndex = globalPositionIndex - runningCount;
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

            partition = _partitionList[indexOfPartitionWithAvailableSpace];
            var partitionAvailableSpace = PartitionSize - partition.Count;

            var batchInsertList = new List<RichCharacter> { richCharacterEnumerator.Current };

            while ((batchInsertList.Count < partitionAvailableSpace) && richCharacterEnumerator.MoveNext())
            {
                batchInsertList.Add(richCharacterEnumerator.Current);
            }
            
            _partitionList = _partitionList.SetItem(
                indexOfPartitionWithAvailableSpace,
                _partitionList[indexOfPartitionWithAvailableSpace].InsertRange(relativePositionIndex, batchInsertList));

            globalPositionIndex += batchInsertList.Count;
        }
    }

    public void PartitionList_RemoveRange(int globalPositionIndex, int count)
    {
        for (int i = 0; i < count; i++)
        {
            PartitionList_RemoveAt(globalPositionIndex);
        }
    }

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
}