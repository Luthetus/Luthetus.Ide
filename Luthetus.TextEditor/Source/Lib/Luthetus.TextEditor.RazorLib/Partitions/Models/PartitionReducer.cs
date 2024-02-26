using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

internal class PartitionReducer
{
    public static PartitionContainer ReduceAdd(
        RichCharacter richCharacter, PartitionContainer container)
    {
        return ReduceInsert(container.GlobalCharacterCount, richCharacter, container);
        //var partitionIndex = -1;

        //for (int i = 0; i < container.PartitionMetadataMap.Count; i++)
        //{
        //    int count = container.PartitionMetadataMap[i].RelativeCharacterCount;
        //    if (count != container.PartitionSize)
        //        partitionIndex = i;
        //}

        //var partitionList = container.PartitionList;
        //var partitionMetadataMap = container.PartitionMetadataMap;
        //int relativePositionIndex;

        //if (partitionIndex == -1)
        //{
        //    var partition = new RichCharacter[] { richCharacter }.ToImmutableList();
        //    partitionList = partitionList.Add(partition);

        //    partitionMetadataMap = partitionMetadataMap.Add(new(partition.Count));

        //    partitionIndex = partitionList.Count - 1;
        //    relativePositionIndex = 0;
        //}
        //else
        //{
        //    var partition = partitionList[partitionIndex].Add(richCharacter);
        //    partitionList = partitionList.SetItem(partitionIndex, partition);

        //    var metadata = partitionMetadataMap[partitionIndex];
        //    partitionMetadataMap = partitionMetadataMap.SetItem(
        //        partitionIndex, metadata with { RelativeCharacterCount = partition.Count });

        //    relativePositionIndex = partition.Count - 1;
        //}

        //Track.Add(
        //    relativePositionIndex, richCharacter, partitionIndex, partitionList, partitionMetadataMap);

        //return container with
        //{
        //    PartitionList = partitionList,
        //    PartitionMetadataMap = partitionMetadataMap
        //};
    }

    public static PartitionContainer ReduceAddRange(
        IEnumerable<RichCharacter> itemList, PartitionContainer container)
    {
        foreach (var item in itemList)
        {
            container = container.Add(item);
        }

        return container;
    }

    public static PartitionContainer ReduceInsert(
        int globalPositionIndex, RichCharacter richCharacter, PartitionContainer container)
    {
        var runningCount = 0;
        var partitionIndex = 0;
        var partition = (ImmutableList<RichCharacter>?)null;
        var relativePositionIndex = 0;

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

        partition = partition.Insert(relativePositionIndex, richCharacter);
        var outPartitionList = container.PartitionList.SetItem(partitionIndex, partition);

        var metadata = container.PartitionMetadataMap[partitionIndex];
        var outPartitionMemoryMap = container.PartitionMetadataMap.SetItem(
            partitionIndex, metadata with { RelativeCharacterCount = metadata.RelativeCharacterCount + 1 });

        Track.Insert(
            globalPositionIndex, richCharacter, partitionIndex, outPartitionList, outPartitionMemoryMap);

        return container with
        {
            PartitionList = outPartitionList,
            PartitionMetadataMap = outPartitionMemoryMap,
        };
    }

    public static PartitionContainer ReduceInsertRange(
        int index, IEnumerable<RichCharacter> itemList, PartitionContainer container)
    {
        foreach (var item in itemList)
        {
            container = container.Insert(index++, item);
        }

        return container;
    }

    public static PartitionContainer ReduceRemove(
        RichCharacter richCharacter, PartitionContainer container)
    {
        for (int i = 0; i < container.GlobalCharacterCount; i++)
        {
            if (container[i].Equals(richCharacter))
                return container.RemoveAt(i);
        }

        return container;
    }

    public static PartitionContainer ReduceRemoveAt(
        int globalPositionIndex, PartitionContainer container)
    {
        var runningCount = 0;
        var partitionIndex = 0;
        var partition = (ImmutableList<RichCharacter>?)null;
        var offset = 0;

        for (int i = 0; i < container.PartitionMetadataMap.Count; i++)
        {
            var currentPartitionCount = container.PartitionMetadataMap[i].RelativeCharacterCount;
            if (runningCount + currentPartitionCount > globalPositionIndex)
            {
                partitionIndex = i;
                partition = container.PartitionList[i];
                offset = globalPositionIndex - runningCount;
                break;
            }
            else
            {
                runningCount += currentPartitionCount;
            }
        }

        if (partition is null)
            throw new IndexOutOfRangeException();

        partition = partition.RemoveAt(offset);
        var outPartitionList = container.PartitionList.SetItem(partitionIndex, partition);

        var metadata = container.PartitionMetadataMap[partitionIndex];
        var outPartitionMetadataMap = container.PartitionMetadataMap.SetItem(
            partitionIndex, metadata with { RelativeCharacterCount = metadata.RelativeCharacterCount - 1 });

        var relativePositionIndex = PartitionContainer.GetRelativePositionIndex(
            partitionIndex, outPartitionList, outPartitionMetadataMap, globalPositionIndex);

        Track.RemoveAt(
            relativePositionIndex, partitionIndex, outPartitionList, outPartitionMetadataMap);

        return container with
        {
            PartitionList = outPartitionList,
            PartitionMetadataMap = outPartitionMetadataMap,
        };
    }

    public static PartitionContainer ReduceRemoveRange(
        int globalPositionIndex, int count, PartitionContainer container)
    {
        for (int i = 0; i < count; i++)
        {
            container = container.RemoveAt(globalPositionIndex);
        }

        return container;
    }

    public static PartitionContainer ReduceExpandPartition(
        int expansionFactor, int partitionIndex, PartitionContainer container)
    {
        if (partitionIndex < 0 || partitionIndex >= container.PartitionList.Count)
            throw new IndexOutOfRangeException();

        var outPartitionList = container.PartitionList;

        // inPartition Contains original text. The goal is to split the text across 3 partitions.
        // Only 2 partitions need to be added because the original partition will be re-used
        var inPartition = outPartitionList[partitionIndex];
        
        var idealSplit = inPartition.Count / expansionFactor;

        // The idealSplit likely will not be enough due to integer math losing decimals.
        // Therefore, figure out how many characters were lost due to integer math.
        var charactersLost = inPartition.Count - (idealSplit * expansionFactor);

        var replaceOriginalPartition = inPartition.Take(idealSplit).ToImmutableList();

        var partitionNewList = new List<ImmutableList<RichCharacter>>();
        for (int i = 1; i < expansionFactor; i++)
        {
            // Start this for loop at 1 since the original is re-used.
            if (i == 1)
            {
                var partitionNew = inPartition
                    .Skip(idealSplit).Take(idealSplit + charactersLost).ToImmutableList();

                partitionNewList.Add(partitionNew);
            }
            else
            {
                var partitionNew = inPartition
                    .Skip(idealSplit * i + charactersLost).Take(idealSplit).ToImmutableList();

                partitionNewList.Add(partitionNew);
            }
        }

        outPartitionList = outPartitionList.SetItem(partitionIndex, replaceOriginalPartition);
        outPartitionList = outPartitionList.InsertRange(partitionIndex + 1, partitionNewList);

        container = container with
        {
            PartitionList = outPartitionList
        };

        var newPartitionMetadata = partitionNewList.Select(x => new PartitionMetadata(x.Count));

        var outPartitionMemoryMap = container.PartitionMetadataMap.InsertRange(
            partitionIndex + 1, newPartitionMetadata);

        for (int i = partitionIndex; i < (partitionIndex + expansionFactor); i++)
        {
            var partition = container.PartitionList[i];
            List<int> tabList = Track.ExpandPartition_Tab(partition);
            List<RowEnding> rowEndingList = Track.ExpandPartition_RowEnding(partition);

            outPartitionMemoryMap = outPartitionMemoryMap.SetItem(
                i,
                new(partition.Count)
                { 
                    TabList = tabList.ToImmutableList(),
                    RowEndingList = rowEndingList.ToImmutableList(),
                });
        }

        return container with
        {
            PartitionMetadataMap = outPartitionMemoryMap
        };
    }
}
