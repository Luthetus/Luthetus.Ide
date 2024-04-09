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

    public void PartitionList_Add(RichCharacter richCharacter)
    {
        PartitionList_Insert(DocumentLength, richCharacter);
    }
}