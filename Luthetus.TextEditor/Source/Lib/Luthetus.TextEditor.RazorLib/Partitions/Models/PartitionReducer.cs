using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

internal class PartitionReducer
{
    public static PartitionContainer ReduceAdd(RichCharacter richCharacter, PartitionContainer container) => ReduceInsert(container.GlobalCharacterCount, richCharacter, container);
    public static PartitionContainer ReduceAddRange(IEnumerable<RichCharacter> richCharacterList, PartitionContainer container) => ReduceInsertRange(container.GlobalCharacterCount, richCharacterList, container);
    public static PartitionContainer ReduceInsert(int globalPositionIndex, RichCharacter richCharacter, PartitionContainer container)
    {
        int runningCount = 0, partitionIndex = 0, relativePositionIndex = 0;
        var partition = (ImmutableList<RichCharacter>?)null;
        for (int i = 0; i < container.PartitionMetadataMap.Count; i++)
        {
            var currentPartitionCount = container.PartitionMetadataMap[i].RelativeCharacterCount;
            if (runningCount + currentPartitionCount >= globalPositionIndex)
            {
                partitionIndex = i;
                if (currentPartitionCount == container.PartitionSize)
                {
                    container = ReduceExpandPartition(PartitionContainer.EXPANSION_FACTOR, partitionIndex, container);
                    i -= 1;
                    continue;
                }
                partition = container.PartitionList[i];
                relativePositionIndex = globalPositionIndex - runningCount;
                break;
            }
            else
            {
                runningCount += currentPartitionCount;
            }
        }
        if (partition is null)
            throw new IndexOutOfRangeException();

        var outPartitionList = container.PartitionList.SetItem(partitionIndex, partition.Insert(relativePositionIndex, richCharacter));

        var metadata = container.PartitionMetadataMap[partitionIndex];
        var outPartitionMemoryMap = container.PartitionMetadataMap.SetItem(partitionIndex,
            metadata with { RelativeCharacterCount = metadata.RelativeCharacterCount + 1 });

        Track.Insert(relativePositionIndex, richCharacter, partitionIndex, outPartitionList, outPartitionMemoryMap);
        return container with { PartitionList = outPartitionList, PartitionMetadataMap = outPartitionMemoryMap, };
    }

    public static PartitionContainer ReduceInsertRange(int index, IEnumerable<RichCharacter> itemList, PartitionContainer container)
    {
        foreach (var item in itemList)
            container = ReduceInsert(index++, item, container);
        return container;
    }

    public static PartitionContainer ReduceRemove(RichCharacter richCharacter, PartitionContainer container)
    {
        for (int globalPositionIndex = 0; globalPositionIndex < container.GlobalCharacterCount; globalPositionIndex++)
            if (container[globalPositionIndex].Equals(richCharacter))
                return ReduceRemoveAt(globalPositionIndex, container);
        return container;
    }

    public static PartitionContainer ReduceRemoveAt(int globalPositionIndex, PartitionContainer container)
    {
        int runningCount = 0, partitionIndex = 0, relativePositionIndex = 0;
        var partition = (ImmutableList<RichCharacter>?)null;
        for (int i = 0; i < container.PartitionMetadataMap.Count; i++)
        {
            var currentPartitionCount = container.PartitionMetadataMap[i].RelativeCharacterCount;
            if (runningCount + currentPartitionCount > globalPositionIndex)
            {
                partitionIndex = i;
                partition = container.PartitionList[i];
                relativePositionIndex = globalPositionIndex - runningCount;
                break;
            }
            else
            {
                runningCount += currentPartitionCount;
            }
        }
        if (partition is null)
            throw new IndexOutOfRangeException();

        var characterToRemove = partition[relativePositionIndex];
        var outPartitionList = container.PartitionList.SetItem(partitionIndex, partition.RemoveAt(relativePositionIndex));

        var metadata = container.PartitionMetadataMap[partitionIndex];
        var outPartitionMetadataMap = container.PartitionMetadataMap.SetItem(partitionIndex,
            metadata with { RelativeCharacterCount = metadata.RelativeCharacterCount - 1 });

        Track.RemoveAt(relativePositionIndex, characterToRemove, partitionIndex, outPartitionList, outPartitionMetadataMap);
        return container with { PartitionList = outPartitionList, PartitionMetadataMap = outPartitionMetadataMap, };
    }

    public static PartitionContainer ReduceRemoveRange(int globalPositionIndex, int count, PartitionContainer container)
    {
        for (int i = 0; i < count; i++)
            container = ReduceRemoveAt(globalPositionIndex, container);
        return container;
    }

    public static PartitionContainer ReduceExpandPartition(int expansionFactor, int partitionIndex, PartitionContainer container)
    {
        if (partitionIndex < 0 || partitionIndex >= container.PartitionList.Count)
            throw new IndexOutOfRangeException();

        var outPartitionList = container.PartitionList; // originalPartition Contains original text. The goal is to split the text across 3 partitions.
        var originalPartition = outPartitionList[partitionIndex];
        var idealSplit = originalPartition.Count / expansionFactor;        
        var charactersLost = originalPartition.Count - (idealSplit * expansionFactor); // The idealSplit likely will not be enough due to integer math losing decimals. Therefore, figure out how many characters were lost due to integer math.
        var replaceOriginalPartition = originalPartition.Take(idealSplit).ToImmutableList();
        var partitionNewList = new List<ImmutableList<RichCharacter>>(); // Only 2 partitions need to be added because the original partition will be re-used

        for (int i = 1; i < expansionFactor; i++)
        {
            if (i == 1) // Start this for loop at 1 since the original is re-used.
            {
                var partitionNew = originalPartition.Skip(idealSplit).Take(idealSplit + charactersLost).ToImmutableList();
                partitionNewList.Add(partitionNew);
            }
            else
            {
                var partitionNew = originalPartition.Skip(idealSplit * i + charactersLost).Take(idealSplit).ToImmutableList();
                partitionNewList.Add(partitionNew);
            }
        }

        outPartitionList = outPartitionList.SetItem(partitionIndex, replaceOriginalPartition);
        outPartitionList = outPartitionList.InsertRange(partitionIndex + 1, partitionNewList);
        container = container with { PartitionList = outPartitionList };

        var newPartitionMetadata = partitionNewList.Select(x => new PartitionMetadata(x.Count));
        var outPartitionMemoryMap = container.PartitionMetadataMap.InsertRange(partitionIndex + 1, newPartitionMetadata);

        for (int i = partitionIndex; i < (partitionIndex + expansionFactor); i++)
        {
            var partition = container.PartitionList[i];
            List<int> tabList = Track.ExpandPartition_Tab(partition);
            List<RowEnding> rowEndingList = Track.ExpandPartition_RowEnding(partition);
            outPartitionMemoryMap = outPartitionMemoryMap.SetItem(i,
                new(partition.Count) {  TabList = tabList.ToImmutableList(), RowEndingList = rowEndingList.ToImmutableList(), });
        }
        return container with { PartitionMetadataMap = outPartitionMemoryMap };
    }
}
