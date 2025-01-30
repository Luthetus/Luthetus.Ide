using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorModelModifier : ITextEditorModel
{
    public void __Insert(int globalPositionIndex, RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition? partition = PartitionList[i];

            if (runningCount + partition.Count >= globalPositionIndex)
            {
                // This is the partition we want to modify.
                // But, we must first check if it has available space.
                if (partition.Count >= PartitionSize)
                {
                    __SplitIntoTwoPartitions(i);
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
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.Insert(relativePositionIndex, richCharacter);

        PartitionListSetItem(
            indexOfPartitionWithAvailableSpace,
            outPartition);
    }

    /// <summary>
    /// If either character or decoration byte are 'null', then the respective
    /// collection will be left unchanged.
    /// 
    /// i.e.: to change ONLY a character value invoke this method with decorationByte set to null,
    ///       and only the <see cref="CharList"/> will be changed.
    /// </summary>
    public void __SetItem(
        int globalPositionIndex,
        RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition? partition = PartitionList[i];

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
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.SetItem(relativePositionIndex, richCharacter);

        PartitionListSetItem(
            indexOfPartitionWithAvailableSpace,
            outPartition);
    }

    public void __SetDecorationByte(
        int globalPositionIndex,
        byte decorationByte)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition? partition = PartitionList[i];

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
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
        var targetRichCharacter = inPartition.RichCharacterList[relativePositionIndex];
        
        inPartition.RichCharacterList[relativePositionIndex] = new(
        	targetRichCharacter.Value,
        	decorationByte);
    }

    public void __RemoveAt(int globalPositionIndex)
    {
        if (globalPositionIndex >= CharCount)
            return;

        int indexOfPartitionWithContent = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition? partition = PartitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithContent = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithContent == -1)
            throw new LuthetusTextEditorException("if (indexOfPartitionWithContent == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithContent];
        var outPartition = inPartition.RemoveAt(relativePositionIndex);

        PartitionListSetItem(
            indexOfPartitionWithContent,
            outPartition);
    }

    private void __InsertNewPartition(int partitionIndex)
    {
        PartitionListInsert(partitionIndex, new TextEditorPartition(new List<RichCharacter>()));
    }

    private void __SplitIntoTwoPartitions(int partitionIndex)
    {
        var originalPartition = PartitionList[partitionIndex];

        var firstUnevenSplit = PartitionSize / 2 + PartitionSize % 2;
        var secondUnevenSplit = PartitionSize / 2;

        // Validate multi-byte characters go on same partition (i.e.: '\r\n')
        {
            // firstUnevenSplit is a count so -1 to make it an index
            if (originalPartition.RichCharacterList[firstUnevenSplit - 1].Value == '\r')
            {
                if (originalPartition.RichCharacterList[(firstUnevenSplit - 1) + 1].Value == '\n')
                {
                    firstUnevenSplit += 1;
                    secondUnevenSplit -= 1;
                }
            }

            // TODO: If the partition to split ends in '\r' and the cause for the split
            //       is to create space in order to insert a '\n',
            //       |
            //       Then this works out as a "happy accident" of sorts.
            //       This is not ideal, it should be more concrete than "oops it worked".
            //       |
            //       The reason it works is because a split won't check if the next partition
            //       has space (? source needed) and will always move the '\r' to the new partition,
            //       then return to the insert and put the '\n' immediately after.

            // One of the reasons for not having a multi-byte character span multiple partitions,
            // is that if a partition has capacity 4,096 but a count of 2,048,
            // one cannot insert between the bytes of a multi-byte character
            // so the first partition with only half its capacity used, would be unable to be used
            // any further than 2,048 because it would mean writing between its multi-byte character
            // than spans into the next partition.
        }

        // Replace old
        {
            var partition = new TextEditorPartition(originalPartition.RichCharacterList
                .Skip(0)
                .Take(firstUnevenSplit)
                .ToList());

            PartitionListSetItem(
                partitionIndex,
                partition);
        }

        // Insert new
        {
            var partition = new TextEditorPartition(originalPartition.RichCharacterList
                .Skip(firstUnevenSplit)
                .Take(secondUnevenSplit)
                .ToList());

            PartitionListInsert(
                partitionIndex + 1,
                partition);
        }
    }

    public void __InsertRange(int globalPositionIndex, IEnumerable<RichCharacter> richCharacterList)
    {
        var richCharacterEnumerator = richCharacterList.GetEnumerator();

        while (richCharacterEnumerator.MoveNext())
        {
            int indexOfPartitionWithAvailableSpace = -1;
            int relativePositionIndex = -1;
            var runningCount = 0;
            TextEditorPartition? partition;

            for (int i = 0; i < PartitionList.Count; i++)
            {
                partition = PartitionList[i];

                if (runningCount + partition.Count >= globalPositionIndex)
                {
                    if (partition.Count >= PartitionSize)
                    {
                        __SplitIntoTwoPartitions(i);
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
                throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

            if (relativePositionIndex == -1)
                throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

            partition = PartitionList[indexOfPartitionWithAvailableSpace];
            var partitionAvailableSpace = PartitionSize - partition.Count;

            var richCharacterBatchInsertList = new List<RichCharacter> { richCharacterEnumerator.Current };

            while (richCharacterBatchInsertList.Count < partitionAvailableSpace && richCharacterEnumerator.MoveNext())
            {
                richCharacterBatchInsertList.Add(richCharacterEnumerator.Current);
            }

            var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
            var outPartition = inPartition.InsertRange(relativePositionIndex, richCharacterBatchInsertList);

            PartitionListSetItem(
                indexOfPartitionWithAvailableSpace,
                outPartition);

            globalPositionIndex += richCharacterBatchInsertList.Count;
        }
    }

    public void __RemoveRange(int targetGlobalPositionIndex, int targetDeleteCount)
    {
        var foundTargetGlobalPositionIndex = false;
        // The 'searchGlobalPositionIndex' is no longer updated after 'foundTargetGlobalPositionIndex' is true
        // It is just being used to find where the data that should be deleted starts.
        var searchGlobalPositionIndex = 0;
        var runningDeleteCount = 0;

        for (int partitionIndex = 0; partitionIndex < PartitionList.Count; partitionIndex++)
        {
            var partition = PartitionList[partitionIndex];
            var relativePositionIndex = 0;

            if (!foundTargetGlobalPositionIndex)
            {
                // It is '>' specifically, because '0 + partition.Count' is a count, therefore the
                // largest index that could exist in the partition is 1 less than the partition.Count.
                if (searchGlobalPositionIndex + partition.Count > targetGlobalPositionIndex)
                {
                    foundTargetGlobalPositionIndex = true;
                    relativePositionIndex = targetGlobalPositionIndex - searchGlobalPositionIndex;
                }
                else
                {
                    searchGlobalPositionIndex += partition.Count;
                    continue;
                }
            }

            // This section of code is dependent on the condition branch above it having performed
            // a 'continue' if it was entered, but still didn't find the 'targetGlobalPositionIndex'

            var availableDeletes = partition.Count - relativePositionIndex;
            var remainingDeletes = targetDeleteCount - runningDeleteCount;

            var deletes = availableDeletes < remainingDeletes
                ? availableDeletes
                : remainingDeletes;

            // WARNING: The code does not currently alter the _partitionList in any way other than this 'SetItem'...
            //          ...invocation, with regards to this method.
            //          If one adds other alterations to the _partitionList in this method,
            //          check if this logic would break.
            PartitionListSetItem(
                partitionIndex,
                partition.RemoveRange(relativePositionIndex, deletes));

            runningDeleteCount += deletes;

            if (runningDeleteCount >= targetDeleteCount)
                break;
        }
    }

    public void __Add(RichCharacter richCharacter)
    {
        __Insert(CharCount, richCharacter);
    }
    
    public void PartitionListSetItem(int index, TextEditorPartition partition)
    {
    	if (!_partitionListIsShallowCopy)
    	{
    		_partitionListIsShallowCopy = true;
    		PartitionList = new(PartitionList);
    	}
    	
    	PartitionList[index] = partition;
    }
    
    public void PartitionListInsert(int index, TextEditorPartition partition)
    {
    	if (!_partitionListIsShallowCopy)
    	{
    		_partitionListIsShallowCopy = true;
    		PartitionList = new(PartitionList);
    	}
    	
    	PartitionList.Insert(index, partition);
    }
}

