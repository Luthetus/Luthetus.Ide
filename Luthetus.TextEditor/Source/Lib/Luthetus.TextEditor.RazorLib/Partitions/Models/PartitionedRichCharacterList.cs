using Luthetus.Common.RazorLib.Partitions.Models;
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
public record PartitionedRichCharacterList : IList<RichCharacter>
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
    public PartitionedRichCharacterList(PartitionedRichCharacterList other)
    {
        // Reset the global metadata each time the record 'with' keyword is used.
        GlobalMetadata = new GlobalRichCharacterMetadataLazy(this);
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public PartitionedRichCharacterList(int partitionSize)
    {
        if (partitionSize < EXPANSION_FACTOR)
            throw new ApplicationException($"Partition size must be equal to or greater than the {nameof(EXPANSION_FACTOR)}:{EXPANSION_FACTOR}.");

        PartitionSize = partitionSize;

        // TODO: How does one not duplicate this code? It exists in the record copy constructor too...
        // ...a parameterless constructor was tried and invoked with 'this()' but it gives
        // an error message specific to usage of record copy constructors.
        GlobalMetadata = new GlobalRichCharacterMetadataLazy(this);
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
    public ImmutableList<PartitionRichCharacterMetadata> PartitionMetadataMap { get; init; } = ImmutableList<PartitionRichCharacterMetadata>.Empty;

    public GlobalRichCharacterMetadataLazy GlobalMetadata { get; }

    public int Count
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

    public RichCharacter this[int globalPositionIndex]
    {
        get
        {
            var rollingCount = 0;

            for (int i = 0; i < PartitionMetadataMap.Count; i++)
            {
                var currentPartitionCount = PartitionMetadataMap[i].Count;

                if (rollingCount + currentPartitionCount > globalPositionIndex)
                {
                    var partition = PartitionList[i];
                    return partition[globalPositionIndex - rollingCount];
                }
                else
                {
                    rollingCount += currentPartitionCount;
                }
            }

            throw new IndexOutOfRangeException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public PartitionedRichCharacterList Add(RichCharacter item)
    {
        var partitionWithFreeSpaceIndex = -1;

        for (int i = 0; i < PartitionMetadataMap.Count; i++)
        {
            int count = PartitionMetadataMap[i].Count;

            if (count != PartitionSize)
                partitionWithFreeSpaceIndex = i;
        }

        var localPartitionList = PartitionList;
        var localPartitionMetadataMap = PartitionMetadataMap;
        int relativePositionIndex;

        if (partitionWithFreeSpaceIndex == -1)
        {
            var partition = new RichCharacter[] { item }.ToImmutableList();

            localPartitionList = localPartitionList.Add(partition);

            localPartitionMetadataMap = localPartitionMetadataMap.Add(new(partition.Count));

            partitionWithFreeSpaceIndex = localPartitionList.Count - 1;
            relativePositionIndex = 0;
        }
        else
        {
            var partition = localPartitionList[partitionWithFreeSpaceIndex];
            partition = partition.Add(item);

            localPartitionList = localPartitionList
                .SetItem(partitionWithFreeSpaceIndex, partition);

            var metadata = localPartitionMetadataMap[partitionWithFreeSpaceIndex];

            localPartitionMetadataMap = localPartitionMetadataMap
                .SetItem(partitionWithFreeSpaceIndex, metadata with { Count = partition.Count });

            relativePositionIndex = partition.Count - 1;
        }

        Add_TabList(
            item,
            partitionWithFreeSpaceIndex,
            localPartitionMetadataMap,
            relativePositionIndex);

        return this with
        {
            PartitionList = localPartitionList,
            PartitionMetadataMap = localPartitionMetadataMap
         };
    }

    private static void Add_TabList(
        RichCharacter item,
        int indexPartitionFreeSpace,
        ImmutableList<PartitionRichCharacterMetadata> localPartitionMetadataMap,
        int relativePositionIndex)
    {
        var inTabList = localPartitionMetadataMap[indexPartitionFreeSpace].TabList;
        var mutableTabList = new List<int>();

        for (int i = 0; i < inTabList.Count; i++)
        {
            mutableTabList.Add(inTabList[i]);
        }

        if (item.Value == '\t')
        {
            mutableTabList.Add(relativePositionIndex);

            localPartitionMetadataMap[indexPartitionFreeSpace].TabList =
                mutableTabList.ToImmutableList();
        }
    }

    /// <summary>
    /// TODO: Make <see cref="AddRange"/> optimized if needed. Currently it just foreach invokes the <see cref="Add"/> version.
    /// </summary>
    public PartitionedRichCharacterList AddRange(IEnumerable<RichCharacter> itemList)
    {
        var partitionedImmutableList = this;

        foreach (var item in itemList)
        {
            partitionedImmutableList = partitionedImmutableList.Add(item);
        }

        return partitionedImmutableList;
    }

    /// <summary>
    /// TODO: Should this method maintain the partitions, but just clear them?...
    /// Or should it return an entirely new instance so that all the memory is freed?
    /// As of this comment I'm going to return an entirely new instance with the same <see cref="PartitionSize"/>.
    /// (2024-02-24).
    /// </summary>
    public PartitionedRichCharacterList Clear()
    {
        return new PartitionedRichCharacterList(PartitionSize);
    }

    public bool Contains(RichCharacter item)
    {
        return IndexOf(item) != -1;
    }

    public void CopyTo(RichCharacter[] array, int arrayIndex)
    {
        PartitionedRichCharacterList list = this;
        for (int i = 0; i < list.Count; i++)
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
        PartitionedRichCharacterList list = this;
        for (int i = 0; i < list.Count; i++)
        {
            RichCharacter? entry = list[i];
            if (item.Equals(entry))
                return i;
        }

        return -1;
    }

    public PartitionedRichCharacterList Insert(int globalPositionIndex, RichCharacter item)
    {
        var outPartitionedImmutableList = this;

        if (outPartitionedImmutableList.PartitionMetadataMap.Count == 0)
            return outPartitionedImmutableList.Add(item);

        var rollingCount = 0;
        var partitionIndex = 0;
        var partition = (ImmutableList<RichCharacter>?)null;
        var relativePositionIndex = 0;

        for (int i = 0; i < outPartitionedImmutableList.PartitionMetadataMap.Count; i++)
        {
            var currentPartitionCount = outPartitionedImmutableList.PartitionMetadataMap[i].Count;

            if (rollingCount + currentPartitionCount >= globalPositionIndex)
            {
                partitionIndex = i;

                if (currentPartitionCount == outPartitionedImmutableList.PartitionSize)
                {
                    outPartitionedImmutableList = outPartitionedImmutableList.ExpandPartition(
                        partitionIndex);

                    i -= 1;
                    continue;
                }

                partition = outPartitionedImmutableList.PartitionList[i];
                relativePositionIndex = globalPositionIndex - rollingCount;
                break;
            }
            else
            {
                rollingCount += currentPartitionCount;
            }
        }

        if (partition is null)
            throw new IndexOutOfRangeException();

        partition = partition.Insert(relativePositionIndex, item);

        var outPartitionList = outPartitionedImmutableList.PartitionList
            .SetItem(partitionIndex, partition);

        var metadata = outPartitionedImmutableList.PartitionMetadataMap[partitionIndex];

        var outPartitionMemoryMap = outPartitionedImmutableList.PartitionMetadataMap.SetItem(
            partitionIndex, metadata with { Count = metadata.Count + 1 });

        Insert_TabList(
            globalPositionIndex,
            item,
            partitionIndex,
            outPartitionList,
            outPartitionMemoryMap);

        return outPartitionedImmutableList with
        {
            PartitionList = outPartitionList,
            PartitionMetadataMap = outPartitionMemoryMap,
        };
    }

    private static void Insert_TabList(
        int globalPositionIndex,
        RichCharacter item,
        int partitionIndex,
        ImmutableList<ImmutableList<RichCharacter>> outPartitionList,
        ImmutableList<PartitionRichCharacterMetadata> outPartitionMemoryMap)
    {
        var inTabList = outPartitionMemoryMap[partitionIndex].TabList;
        var mutableTabList = new List<int>();

        var relativePositionIndex = GetRelativePositionIndex(
            partitionIndex, outPartitionList, outPartitionMemoryMap, globalPositionIndex);

        var relativeTabIndex = inTabList.FindIndex(x => x >= relativePositionIndex);

        // Copy over unmodified values
        var copyForLoopUpperLimit = relativeTabIndex == -1 ? inTabList.Count : relativeTabIndex;
        for (int i = 0; i < copyForLoopUpperLimit; i++)
        {
            mutableTabList.Add(inTabList[i]);
        }

        // Write the shifted values
        var shiftForLoopLowerLimit = copyForLoopUpperLimit;
        for (int i = shiftForLoopLowerLimit; i < inTabList.Count; i++)
        {
            mutableTabList.Add(inTabList[i] + 1);
        }
        
        if (item.Value == '\t')
        {
            if (relativeTabIndex == -1)
                mutableTabList.Add(relativePositionIndex);
            else
                mutableTabList.Insert(relativeTabIndex, relativePositionIndex);

            outPartitionMemoryMap[partitionIndex].TabList = mutableTabList.ToImmutableList();
        }
    }

    public PartitionedRichCharacterList InsertRange(int index, IEnumerable<RichCharacter> itemList)
    {
        var partitionedImmutableList = this;

        foreach (var item in itemList)
        {
            partitionedImmutableList = partitionedImmutableList.Insert(index++, item);

#if DEBUG // TODO: Delete these variables
            // Reading some state so I see it in debugger
            {
                var globalMetadataLazy = partitionedImmutableList.GlobalMetadata;
                var globalTabList = globalMetadataLazy.TabList.Value;
                var globalAllText = globalMetadataLazy.AllText.Value;
            }
#endif
        }

        return partitionedImmutableList;
    }

    public PartitionedRichCharacterList Remove(RichCharacter item)
    {
        PartitionedRichCharacterList list = this;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(item))
                return RemoveAt(i);
        }

        return this;
    }

    public PartitionedRichCharacterList RemoveAt(int index)
    {
        var rollingCount = 0;
        var indexPartition = 0;
        var partition = (ImmutableList<RichCharacter>?)null;
        var offset = 0;

        for (int i = 0; i < PartitionMetadataMap.Count; i++)
        {
            var currentPartitionCount = PartitionMetadataMap[i].Count;

            if (rollingCount + currentPartitionCount > index)
            {
                indexPartition = i;
                partition = PartitionList[i];
                offset = index - rollingCount;
                break;
            }
            else
            {
                rollingCount += currentPartitionCount;
            }
        }

        if (partition is null)
            throw new IndexOutOfRangeException();

        partition = partition.RemoveAt(offset);

        var outPartitionList = PartitionList.SetItem(indexPartition, partition);

        var metadata = PartitionMetadataMap[indexPartition];

        var outPartitionMemoryMap = PartitionMetadataMap.SetItem(
            indexPartition, metadata with { Count = metadata.Count - 1 });

        RemoveAt_TabList(index, indexPartition, outPartitionMemoryMap);

        return this with
        {
            PartitionList = outPartitionList,
            PartitionMetadataMap = outPartitionMemoryMap,
        };
    }

    private static void RemoveAt_TabList(int globalPositionIndex, int indexPartition, ImmutableList<PartitionRichCharacterMetadata> outPartitionMemoryMap)
    {
        var inTabList = outPartitionMemoryMap[indexPartition].TabList;
        var mutableTabList = new List<int>();

        var relativeTabIndex = inTabList.FindIndex(x => x >= globalPositionIndex);

        // Copy over unmodified values
        for (int i = 0; i < relativeTabIndex; i++)
        {
            mutableTabList.Add(inTabList[i]);
        }

        // Write the shifted values
        for (int i = relativeTabIndex; i < inTabList.Count; i++)
        {
            // Do not write out the 'removed tab' (if one were removed)
            if (inTabList[i] == globalPositionIndex)
                continue;

            mutableTabList.Add(inTabList[i] - 1);
        }

        outPartitionMemoryMap[indexPartition].TabList = mutableTabList.ToImmutableList();
    }

    public PartitionedRichCharacterList RemoveRange(int globalPositionIndex, int count)
    {
        var partitionedImmutableList = this;

        for (int i = 0; i < count; i++)
        {
            partitionedImmutableList = partitionedImmutableList.RemoveAt(globalPositionIndex);
        }

        return partitionedImmutableList;
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

    private PartitionedRichCharacterList ExpandPartition(int partitionIndex)
    {
        var outPartitionedImmutableList = this;

        if (partitionIndex < 0 || partitionIndex >= outPartitionedImmutableList.PartitionList.Count)
            throw new IndexOutOfRangeException();

        var outPartitionList = outPartitionedImmutableList.PartitionList;

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
                    .Skip(idealSplit * i + charactersLost) .Take(idealSplit) .ToImmutableList();

                partitionNewList.Add(partitionNew);
            }
        }

        outPartitionList = outPartitionList.SetItem(partitionIndex, replaceOriginalPartition);
        outPartitionList = outPartitionList.InsertRange(partitionIndex + 1, partitionNewList);

        outPartitionedImmutableList = outPartitionedImmutableList with
        {
            PartitionList = outPartitionList
        };

        var newPartitionMetadata = partitionNewList.Select(x => new PartitionRichCharacterMetadata(x.Count));

        var outPartitionMemoryMap = outPartitionedImmutableList.PartitionMetadataMap
            .InsertRange(partitionIndex + 1, newPartitionMetadata);

        for (int i = partitionIndex; i < (partitionIndex + EXPANSION_FACTOR); i++)
        {
            var partition = outPartitionedImmutableList.PartitionList[i];

            // Handle TabList
            List<int> mutableList = ExpandPartition_TabList(partition);

            outPartitionMemoryMap = outPartitionMemoryMap.SetItem(
                i,
                new(partition.Count)
                {
                    TabList = mutableList.ToImmutableList()
                });
        }

        return outPartitionedImmutableList with
        {
            PartitionMetadataMap = outPartitionMemoryMap
        };
    }

    private static List<int> ExpandPartition_TabList(ImmutableList<RichCharacter> partition)
    {
        List<int> mutableList = new();
        {
            // TODO: Don't count the tabs, instead divide the original TabList

            for (int tabCounterIndex = 0; tabCounterIndex < partition.Count; tabCounterIndex++)
            {
                if (partition[tabCounterIndex].Value == '\t')
                    mutableList.Add(tabCounterIndex);
            }
        }

        return mutableList;
    }

    private static (ImmutableList<RichCharacter> partition, int partitionIndex, int rollingCount) GetOffsetPartition(
        ImmutableList<ImmutableList<RichCharacter>> partitionList,
        ImmutableList<PartitionRichCharacterMetadata> partitionMetadataMap,
        int index)
    {
        var rollingCount = 0;

        for (int i = 0; i < partitionMetadataMap.Count; i++)
        {
            var currentPartitionCount = partitionMetadataMap[i].Count;

            if (rollingCount + currentPartitionCount > index)
                return (partitionList[i], i, rollingCount);
            else
                rollingCount += currentPartitionCount;
        }

        throw new IndexOutOfRangeException();
    }
    
    private static int GetRelativePositionIndex(
        int partitionIndex,
        ImmutableList<ImmutableList<RichCharacter>> partitionList,
        ImmutableList<PartitionRichCharacterMetadata> partitionMetadataMap,
        int globalPositionIndex)
    {
        var offsetInfoPartition = GetOffsetPartition(
            partitionList, partitionMetadataMap, partitionIndex);

        return globalPositionIndex - offsetInfoPartition.rollingCount;
    }
}
