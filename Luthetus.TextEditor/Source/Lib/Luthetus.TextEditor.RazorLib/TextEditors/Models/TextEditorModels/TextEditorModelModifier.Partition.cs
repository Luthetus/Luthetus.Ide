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
            TextEditorPartition? partition = _partitionList[i];

            if (runningCount + partition.CharList.Count >= globalPositionIndex)
            {
                // This is the partition we want to modify.
                // But, we must first check if it has available space.
                if (partition.CharList.Count >= PartitionSize)
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
                runningCount += partition.CharList.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new ApplicationException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new ApplicationException("if (relativePositionIndex == -1)");

        var targetPartition = _partitionList[indexOfPartitionWithAvailableSpace];

        targetPartition.DecorationByteList.Insert(relativePositionIndex, richCharacter.DecorationByte);

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            _partitionList[indexOfPartitionWithAvailableSpace] with 
            {
                CharList = targetPartition.CharList.Insert(relativePositionIndex, richCharacter.Value)
            });
    }

    public void PartitionList_RemoveAt(int globalPositionIndex)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            TextEditorPartition? partition = _partitionList[i];

            if (runningCount + partition.CharList.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.CharList.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new ApplicationException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new ApplicationException("if (relativePositionIndex == -1)");

        var targetPartition = _partitionList[indexOfPartitionWithAvailableSpace];

        targetPartition.DecorationByteList.RemoveAt(relativePositionIndex);

        targetPartition = targetPartition with
        {
            CharList = targetPartition.CharList.RemoveAt(relativePositionIndex)
        };

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            targetPartition);
    }

    private void PartitionList_InsertNewPartition(int partitionIndex)
    {
        _partitionList = _partitionList.Insert(partitionIndex, TextEditorPartition.Empty);
    }
    
    private void PartitionList_SplitIntoTwoPartitions(int partitionIndex)
    {
        var originalPartition = _partitionList[partitionIndex];

        var firstUnevenSplit = PartitionSize / 2 + (PartitionSize % 2);
        var secondUnevenSplit = PartitionSize / 2;

        // Replace old
        {
            var partition = new TextEditorPartition(
                ImmutableList<char>.Empty.AddRange(originalPartition.CharList.Take(firstUnevenSplit)),
                new());

            partition.DecorationByteList.AddRange(originalPartition.CharList.Take(firstUnevenSplit).Select(x => (byte)0));

            _partitionList = _partitionList.SetItem(
                partitionIndex,
                partition);
        }

        // Insert new
        {
            var partition = new TextEditorPartition(
                ImmutableList<char>.Empty.AddRange(originalPartition.CharList.Skip(firstUnevenSplit).Take(secondUnevenSplit)),
                new());

            partition.DecorationByteList.AddRange(
                originalPartition.CharList.Skip(firstUnevenSplit).Take(secondUnevenSplit).Select(x => (byte)0));

            _partitionList = _partitionList.Insert(
                partitionIndex + 1,
                partition);
        }
    }

    public void PartitionList_InsertRange(int globalPositionIndex, IEnumerable<RichCharacter> richCharacterList)
    {
        var richCharacterEnumerator = richCharacterList.GetEnumerator();

        while (richCharacterEnumerator.MoveNext())
        {
            int indexOfPartitionWithAvailableSpace = -1;
            int relativePositionIndex = -1;
            var runningCount = 0;
            TextEditorPartition? partition;

            for (int i = 0; i < _partitionList.Count; i++)
            {
                partition = _partitionList[i];

                if (runningCount + partition.CharList.Count >= globalPositionIndex)
                {
                    if (partition.CharList.Count >= PartitionSize)
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
                    runningCount += partition.CharList.Count;
                }
            }

            if (indexOfPartitionWithAvailableSpace == -1)
                throw new ApplicationException("if (indexOfPartitionWithAvailableSpace == -1)");

            if (relativePositionIndex == -1)
                throw new ApplicationException("if (relativePositionIndex == -1)");

            partition = _partitionList[indexOfPartitionWithAvailableSpace];
            var partitionAvailableSpace = PartitionSize - partition.CharList.Count;

            var charBatchInsertList = new List<char> { richCharacterEnumerator.Current.Value };
            var decorationByteBatchInsertList = new List<byte> { richCharacterEnumerator.Current.DecorationByte };

            while ((charBatchInsertList.Count < partitionAvailableSpace) && richCharacterEnumerator.MoveNext())
            {
                charBatchInsertList.Add(richCharacterEnumerator.Current.Value);
                decorationByteBatchInsertList.Add(richCharacterEnumerator.Current.DecorationByte);
            }

            var targetPartition = _partitionList[indexOfPartitionWithAvailableSpace];


            targetPartition.DecorationByteList.InsertRange(relativePositionIndex, decorationByteBatchInsertList);

            _partitionList = _partitionList.SetItem(
                indexOfPartitionWithAvailableSpace,
                targetPartition with 
                {
                    CharList = targetPartition.CharList.InsertRange(relativePositionIndex, charBatchInsertList),
                });

            globalPositionIndex += charBatchInsertList.Count;
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