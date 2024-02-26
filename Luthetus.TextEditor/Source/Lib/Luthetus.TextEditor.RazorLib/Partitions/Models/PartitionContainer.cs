using Luthetus.TextEditor.RazorLib.Characters.Models;
using System.Collections;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

/// <summary>
/// TODO: I need to handle the partition meta data differently for the text editor...
/// ...Perhaps it would be preferable to have a generic <see cref="PartitionedImmutableList{TItem}"/>
/// type, but I'm finding a it hard to do so without odd looking and confusing code.
/// So, I'm going to copy and paste the attempt at the generic type here, then just
/// change the source code to work for the text editor. (2024-02-25)
/// </summary>
public record PartitionContainer : IList<RichCharacter>
{
    /// <summary>
    /// When a partition runs out space its content is divided amongst some amount of partitions.
    /// If expansion factor is 3, then when a partition expands,
    /// it will insert 2 addition partitions after itself.
    /// Then the original partition splits its content into thirds.
    /// And distributes it across itself, and the other 2 newly inserted partitions.
    /// </summary>
    public const int EXPANSION_FACTOR = 3;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // This is a record copy constructor
    public PartitionContainer(PartitionContainer other)
    {
        PartitionSize = other.PartitionSize;

        // Reset the global metadata each time the record 'with' keyword is used.
        GlobalMetadata = new GlobalMetadataLazy(this);
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public PartitionContainer(int partitionSize)
    {
        if (partitionSize < EXPANSION_FACTOR)
            throw new ApplicationException($"Partition size must be equal to or greater than the {nameof(EXPANSION_FACTOR)}:{EXPANSION_FACTOR}.");

        PartitionSize = partitionSize;

        // TODO: How does one not duplicate this code? It exists in the record copy constructor too...
        // ...a parameterless constructor was tried and invoked with 'this()' but it gives
        // an error message specific to usage of record copy constructors.
        GlobalMetadata = new GlobalMetadataLazy(this);
    }

    public int PartitionSize { get; }
    public ImmutableList<ImmutableList<RichCharacter>> PartitionList { get; init; } = ImmutableList<ImmutableList<RichCharacter>>.Empty;

    /// <summary>
    /// Track the 'Count' of each partition in this.
    /// Therefore, one can lookup whether a partition is full or not.
    /// Without iterating through all the partitions just to check a specific one.
    /// <br/>
    /// The name 'Map' is used here because to get the Count of the 0th index partition,
    /// one would read the value at index 0 of this property.
    /// In otherwords, each partition index maps to its corresponding Count.
    /// </summary>
    public ImmutableList<PartitionMetadata> PartitionMetadataMap { get; init; } = ImmutableList<PartitionMetadata>.Empty;

    public GlobalMetadataLazy GlobalMetadata { get; }

    public int GlobalCharacterCount
    {
        get
        {
            if (PartitionList.Count == 0)
                return 0;

            var count = 0;

            foreach (var partition in PartitionList)
            {
                count += partition.Count;
            }

            return count;
        }
    }

    public bool IsReadOnly => true;

    int ICollection<RichCharacter>.Count => GlobalCharacterCount;

    public RichCharacter this[int globalPositionIndex]
    {
        get
        {
            var runningCount = 0;

            for (int i = 0; i < PartitionMetadataMap.Count; i++)
            {
                var partitionCount = PartitionMetadataMap[i].RelativeCharacterCount;

                if (runningCount + partitionCount > globalPositionIndex)
                {
                    var partition = PartitionList[i];
                    return partition[globalPositionIndex - runningCount];
                }
                else
                {
                    runningCount += partitionCount;
                }
            }

            throw new IndexOutOfRangeException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public PartitionContainer Add(RichCharacter item)
    {
        return Static_Add(item, this);
        static PartitionContainer Static_Add(RichCharacter richCharacter, PartitionContainer container)
        {
            var partitionIndex = -1;

            for (int i = 0; i < container.PartitionMetadataMap.Count; i++)
            {
                int count = container.PartitionMetadataMap[i].RelativeCharacterCount;

                if (count != container.PartitionSize)
                    partitionIndex = i;
            }

            var partitionList = container.PartitionList;
            var partitionMetadataMap = container.PartitionMetadataMap;
            int relativePositionIndex;

            if (partitionIndex == -1)
            {
                var partition = new RichCharacter[] { richCharacter }.ToImmutableList();

                partitionList = partitionList.Add(partition);

                partitionMetadataMap = partitionMetadataMap.Add(new(partition.Count));

                partitionIndex = partitionList.Count - 1;
                relativePositionIndex = 0;
            }
            else
            {
                var partition = partitionList[partitionIndex];
                partition = partition.Add(richCharacter);

                partitionList = partitionList
                    .SetItem(partitionIndex, partition);

                var metadata = partitionMetadataMap[partitionIndex];

                partitionMetadataMap = partitionMetadataMap
                    .SetItem(partitionIndex, metadata with { RelativeCharacterCount = partition.Count });

                relativePositionIndex = partition.Count - 1;
            }

            Track.Add(
                relativePositionIndex,
                richCharacter,
                partitionIndex,
                partitionList,
                partitionMetadataMap);

            return container with
            {
                PartitionList = partitionList,
                PartitionMetadataMap = partitionMetadataMap
            };
        }
    }

    public PartitionContainer AddRange(IEnumerable<RichCharacter> itemList)
    {
        return Static_AddRange(itemList, this);
        static PartitionContainer Static_AddRange(IEnumerable<RichCharacter> itemList, PartitionContainer container)
        {
            foreach (var item in itemList)
            {
                container = container.Add(item);
            }

            return container;
        }
    }

    public PartitionContainer Clear() => new PartitionContainer(PartitionSize);

    public bool Contains(RichCharacter item) => IndexOf(item) != -1;

    public void CopyTo(RichCharacter[] array, int arrayIndex)
    {
        PartitionContainer list = this;
        for (int i = 0; i < list.GlobalCharacterCount; i++)
        {
            array[i] = list[i];
        }
    }

    public IEnumerator<RichCharacter> GetEnumerator()
    {
        foreach (var partition in PartitionList)
        {
            foreach (var item in partition)
            {
                yield return item;
            }
        }
    }

    public int IndexOf(RichCharacter item)
    {
        PartitionContainer list = this;
        for (int i = 0; i < list.GlobalCharacterCount; i++)
        {
            RichCharacter? entry = list[i];
            if (item.Equals(entry))
                return i;
        }

        return -1;
    }

    public PartitionContainer Insert(int globalPositionIndex, RichCharacter richCharacter)
    {
        return Static_Insert(globalPositionIndex, richCharacter, this);
        static PartitionContainer Static_Insert(int globalPositionIndex, RichCharacter richCharacter, PartitionContainer container)
        {
            if (container.PartitionMetadataMap.Count == 0)
                return container.Add(richCharacter);

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
                        container = container.ExpandPartition(
                            partitionIndex);

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

            var outPartitionList = container.PartitionList
                .SetItem(partitionIndex, partition);

            var metadata = container.PartitionMetadataMap[partitionIndex];

            var outPartitionMemoryMap = container.PartitionMetadataMap.SetItem(
                partitionIndex, metadata with { RelativeCharacterCount = metadata.RelativeCharacterCount + 1 });

            Track.Insert(
                globalPositionIndex,
                richCharacter,
                partitionIndex,
                outPartitionList,
                outPartitionMemoryMap);

            return container with
            {
                PartitionList = outPartitionList,
                PartitionMetadataMap = outPartitionMemoryMap,
            };
        }
    }

    public PartitionContainer InsertRange(int index, IEnumerable<RichCharacter> itemList)
    {
        return Static_InsertRange(index, itemList, this);
        static PartitionContainer Static_InsertRange(int index, IEnumerable<RichCharacter> itemList, PartitionContainer container)
        {
            foreach (var item in itemList)
            {
                container = container.Insert(index++, item);
            }

            return container;
        }
    }

    public PartitionContainer Remove(RichCharacter richCharacter)
    {
        return Static_Remove(richCharacter, this);
        static PartitionContainer Static_Remove(RichCharacter richCharacter, PartitionContainer container)
        {
            for (int i = 0; i < container.GlobalCharacterCount; i++)
            {
                if (container[i].Equals(richCharacter))
                    return container.RemoveAt(i);
            }

            return container;
        }
    }

    public PartitionContainer RemoveAt(int globalPositionIndex)
    {
        return Static_RemoveAt(globalPositionIndex, this);
        static PartitionContainer Static_RemoveAt(int globalPositionIndex, PartitionContainer container)
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

            var relativePositionIndex = GetRelativePositionIndex(
                partitionIndex,
                outPartitionList,
                outPartitionMetadataMap,
                globalPositionIndex);

            Track.RemoveAt(
                relativePositionIndex,
                partitionIndex,
                outPartitionList,
                outPartitionMetadataMap);

            return container with
            {
                PartitionList = outPartitionList,
                PartitionMetadataMap = outPartitionMetadataMap,
            };
        }
    }

    public PartitionContainer RemoveRange(int globalPositionIndex, int count)
    {
        return Static_RemoveRange(globalPositionIndex, count, this);
        static PartitionContainer Static_RemoveRange(int globalPositionIndex, int count, PartitionContainer container)
        {
            for (int i = 0; i < count; i++)
            {
                container = container.RemoveAt(globalPositionIndex);
            }

            return container;
        }
    }

    private PartitionContainer ExpandPartition(int partitionIndex)
    {
        return Static_ExpandPartition(partitionIndex, this);
        static PartitionContainer Static_ExpandPartition(int partitionIndex, PartitionContainer container)
        {
            if (partitionIndex < 0 || partitionIndex >= container.PartitionList.Count)
                throw new IndexOutOfRangeException();

            var outPartitionList = container.PartitionList;

            // inPartition Contains original text.
            // The goal is to split the text across 3 partitions.
            // Allot the original content in 1/3.
            //
            // Only 2 partitions need to be inserted because the third partition will just be the
            // original partition, but with its contents change.
            var inPartition = outPartitionList[partitionIndex];

            // The idealSplit likely will not be enough due to integer math losing decimals.
            // Therefore, give the middle partition any remainder.
            var idealSplit = inPartition.Count / EXPANSION_FACTOR;

            // Determine the impact of integer math loss of decimal places
            var charactersLost = inPartition.Count - (idealSplit * EXPANSION_FACTOR);

            var replaceOriginalPartition = inPartition;

            var partitionNewList = new List<ImmutableList<RichCharacter>>();
            for (int i = 0; i < EXPANSION_FACTOR; i++)
            {
                if (i == 0)
                {
                    replaceOriginalPartition = inPartition.Take(idealSplit).ToImmutableList();
                }
                else if (i == 1)
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

            var outPartitionMemoryMap = container.PartitionMetadataMap
                .InsertRange(partitionIndex + 1, newPartitionMetadata);

            for (int i = partitionIndex; i < (partitionIndex + EXPANSION_FACTOR); i++)
            {
                var partition = container.PartitionList[i];

                List<int> mutableList = Track.ExpandPartition(partition);

                outPartitionMemoryMap = outPartitionMemoryMap.SetItem(
                    i,
                    new(partition.Count)
                    {
                        TabList = mutableList.ToImmutableList()
                    });
            }

            return container with
            {
                PartitionMetadataMap = outPartitionMemoryMap
            };
        }
    }

    private static (ImmutableList<RichCharacter> partition, int partitionIndex, int runningCount) GetOffsetPartition(
        ImmutableList<ImmutableList<RichCharacter>> partitionList,
        ImmutableList<PartitionMetadata> partitionMetadataMap,
        int index)
    {
        var runningCount = 0;

        for (int i = 0; i < partitionMetadataMap.Count; i++)
        {
            var currentPartitionCount = partitionMetadataMap[i].RelativeCharacterCount;

            if (runningCount + currentPartitionCount > index)
                return (partitionList[i], i, runningCount);
            else
                runningCount += currentPartitionCount;
        }

        throw new IndexOutOfRangeException();
    }
    
    internal static int GetRelativePositionIndex(
        int partitionIndex,
        ImmutableList<ImmutableList<RichCharacter>> partitionList,
        ImmutableList<PartitionMetadata> partitionMetadataMap,
        int globalPositionIndex)
    {
        var offsetInfoPartition = GetOffsetPartition(
            partitionList, partitionMetadataMap, partitionIndex);

        return globalPositionIndex - offsetInfoPartition.runningCount;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void ICollection<RichCharacter>.Add(RichCharacter item) => Add(item);
    void ICollection<RichCharacter>.Clear() => Clear();

    bool ICollection<RichCharacter>.Remove(RichCharacter item)
    {
        var index = IndexOf(item);

        if (index == -1)
            return false;

        RemoveAt(index);
        return true;
    }

    void IList<RichCharacter>.Insert(int index, RichCharacter item) => Insert(index, item);
    void IList<RichCharacter>.RemoveAt(int index) => RemoveAt(index);
}
