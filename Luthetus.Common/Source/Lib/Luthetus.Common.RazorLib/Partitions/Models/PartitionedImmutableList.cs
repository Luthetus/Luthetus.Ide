using System.Collections;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Partitions.Models;

public record PartitionedImmutableList<TItem> : IList<TItem> where TItem : notnull
{
    /// <summary>
    /// When a partition runs out space its content is divided amongst
    /// some amount of partitions.
    /// If expansion factor is 3, then when a partition expands,
    /// it will insert 2 addition partitions after itself.
    /// Then the original partition splits its content into thirds.
    /// And distributes it across itself, and the other 2 newly inserted partitions.
    /// </summary>
    public const int EXPANSION_FACTOR = 3;

    public PartitionedImmutableList(int partitionSize)
    {
        if (partitionSize < EXPANSION_FACTOR)
            throw new ApplicationException($"Partition size must be equal to or greater than the {nameof(EXPANSION_FACTOR)}:{EXPANSION_FACTOR}.");

        PartitionSize = partitionSize;
    }

    public TItem this[int index]
    {
        get
        {
            var rollingCount = 0;

            for (int i = 0; i < PartitionMemoryMap.Count; i++)
            {
                var currentPartitionCount = PartitionMemoryMap[i];

                if (rollingCount + currentPartitionCount > index)
                {
                    var partition = PartitionList[i];
                    return partition[index - rollingCount];
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

    public int PartitionSize { get; }
    public ImmutableList<ImmutableList<TItem>> PartitionList { get; init; } = ImmutableList<ImmutableList<TItem>>.Empty;

    /// <summary>
    /// Track the 'Count' of each partition in this.
    /// Therefore, one can lookup whether a partition is full or not.
    /// Without iterating through all the partitions just to check a specific one.
    /// <br/>
    /// The name 'Map' is used here because to get the Count of the 0th index partition,
    /// one would read the value at index 0 of this property.
    /// In otherwords, each partition index maps to its corresponding Count.
    /// </summary>
    public ImmutableList<int> PartitionMemoryMap { get; init; } = ImmutableList<int>.Empty;

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

    public PartitionedImmutableList<TItem> Add(TItem value)
    {
        var indexPartitionFreeSpace = -1;

        for (int i = 0; i < PartitionMemoryMap.Count; i++)
        {
            int count = PartitionMemoryMap[i];

            if (count != PartitionSize)
                indexPartitionFreeSpace = i;
        }

        if (indexPartitionFreeSpace == -1)
        {
            var partition = new TItem[] { value }.ToImmutableList();

            return this with
            {
                PartitionList = PartitionList.Add(partition),
                PartitionMemoryMap = PartitionMemoryMap.Add(partition.Count)
            };
        }
        else
        {
            var partition = PartitionList[indexPartitionFreeSpace];
            partition = partition.Add(value);

            return this with
            {
                PartitionList = PartitionList.SetItem(indexPartitionFreeSpace, partition),
                PartitionMemoryMap = PartitionMemoryMap.SetItem(indexPartitionFreeSpace, partition.Count)
            };
        }
    }

    /// <summary>
    /// TODO: Make <see cref="AddRange"/> optimized if needed. Currently it just foreach invokes the <see cref="Add"/> version.
    /// </summary>
    public PartitionedImmutableList<TItem> AddRange(IEnumerable<TItem> itemList)
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
    public PartitionedImmutableList<TItem> Clear()
    {
        return new PartitionedImmutableList<TItem>(PartitionSize);
    }

    public bool Contains(TItem item)
    {
        return IndexOf(item) != -1;
    }

    public void CopyTo(TItem[] array, int arrayIndex)
    {
        PartitionedImmutableList<TItem> list = this;
        for (int i = 0; i < list.Count; i++)
        {
            array[i] = list[i];
        }
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        foreach (var partition in PartitionList)
        {
            foreach (var item in partition)
            {
                yield return item;
            }
        }
    }

    public int IndexOf(TItem item)
    {
        PartitionedImmutableList<TItem> list = this;
        for (int i = 0; i < list.Count; i++)
        {
            TItem? entry = list[i];
            if (item.Equals(entry))
                return i;
        }

        return -1;
    }

    public PartitionedImmutableList<TItem> Insert(int index, TItem item)
    {
        var outPartitionedImmutableList = this;

        if (outPartitionedImmutableList.PartitionMemoryMap.Count == 0)
            return outPartitionedImmutableList.Add(item);

        var rollingCount = 0;
        var indexPartition = 0;
        var partition = (ImmutableList<TItem>?)null;
        var offset = 0;

        for (int i = 0; i < outPartitionedImmutableList.PartitionMemoryMap.Count; i++)
        {
            var currentPartitionCount = outPartitionedImmutableList.PartitionMemoryMap[i];

            if (rollingCount + currentPartitionCount >= index)
            {
                indexPartition = i;

                if (currentPartitionCount == outPartitionedImmutableList.PartitionSize)
                {
                    outPartitionedImmutableList = outPartitionedImmutableList.ExpandPartition(indexPartition);
                    i -= 1;
                    continue;
                }

                partition = outPartitionedImmutableList.PartitionList[i];
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

        partition = partition.Insert(offset, item);

        var outPartitionList = outPartitionedImmutableList.PartitionList.SetItem(
            indexPartition,
            partition);

        var outPartitionMemoryMap = outPartitionedImmutableList.PartitionMemoryMap.SetItem(
            indexPartition,
            outPartitionedImmutableList.PartitionMemoryMap[indexPartition] + 1);

        return outPartitionedImmutableList with
        {
            PartitionList = outPartitionList,
            PartitionMemoryMap = outPartitionMemoryMap,
        };
    }
    
    public PartitionedImmutableList<TItem> InsertRange(int index, IEnumerable<TItem> itemList)
    {
        var partitionedImmutableList = this;

        foreach (var item in itemList)
        {
            partitionedImmutableList = partitionedImmutableList.Insert(index++, item);
        }

        return partitionedImmutableList;
    }

    public PartitionedImmutableList<TItem> Remove(TItem item)
    {
        PartitionedImmutableList<TItem> list = this;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(item))
                return RemoveAt(i);
        }

        return this;
    }

    public PartitionedImmutableList<TItem> RemoveAt(int index)
    {
        var rollingCount = 0;
        var indexPartition = 0;
        var partition = (ImmutableList<TItem>?)null;
        var offset = 0;

        for (int i = 0; i < PartitionMemoryMap.Count; i++)
        {
            var currentPartitionCount = PartitionMemoryMap[i];

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
        var outPartitionMemoryMap = PartitionMemoryMap.SetItem(indexPartition, PartitionMemoryMap[indexPartition] - 1);

        return this with
        {
            PartitionList = outPartitionList,
            PartitionMemoryMap = outPartitionMemoryMap,
        };
    }
    
    public PartitionedImmutableList<TItem> RemoveRange(int index, int count)
    {
        var partitionedImmutableList = this;

        for (int i = 0; i < count; i++)
        {
            partitionedImmutableList = partitionedImmutableList.RemoveAt(index);
        }

        return partitionedImmutableList;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void ICollection<TItem>.Add(TItem item) => Add(item);
    void ICollection<TItem>.Clear() => Clear();

    bool ICollection<TItem>.Remove(TItem item)
    {
        var index = IndexOf(item);

        if (index == -1)
            return false;

        RemoveAt(index);
        return true;
    }

    void IList<TItem>.Insert(int index, TItem item) => Insert(index, item);
    void IList<TItem>.RemoveAt(int index) => RemoveAt(index);

    private PartitionedImmutableList<TItem> ExpandPartition(int index)
    {
        var outPartitionedImmutableList = this;

        if (index < 0 || index >= outPartitionedImmutableList.PartitionList.Count)
            throw new IndexOutOfRangeException();

        var outPartitionList = outPartitionedImmutableList.PartitionList;

        // inPartition Contains original text.
        // The goal is to split the text across 3 partitions.
        // Allot the original content in 1/3.
        //
        // Only 2 partitions need to be inserted because the third partition will just be the
        // original partition, but with its contents change.
        var inPartition = outPartitionList[index];

        // The idealSplit likely will not be enough due to integer math losing decimals.
        // Therefore, give the middle partition any remainder.
        var idealSplit = inPartition.Count / EXPANSION_FACTOR;

        // Determine the impact of integer math loss of decimal places
        var charactersLost = inPartition.Count - (idealSplit * EXPANSION_FACTOR);

        var replaceOriginalPartition = inPartition;

        var partitionNewList = new List<ImmutableList<TItem>>();
        for (int i = 0; i < EXPANSION_FACTOR; i++)
        {
            if (i == 0)
            {
                replaceOriginalPartition = inPartition
                    .Take(idealSplit)
                    .ToImmutableList();
            }
            else if (i == 1)
            {
                var partitionNew = inPartition
                    .Skip(idealSplit)
                    .Take(idealSplit + charactersLost)
                    .ToImmutableList();

                partitionNewList.Add(partitionNew);
            }
            else
            {
                var partitionNew = inPartition
                    .Skip(idealSplit * i + charactersLost)
                    .Take(idealSplit)
                    .ToImmutableList();

                partitionNewList.Add(partitionNew);
            }
        }

        outPartitionList = outPartitionList.SetItem(index, replaceOriginalPartition);
        outPartitionList = outPartitionList.InsertRange(index + 1, partitionNewList);

        outPartitionedImmutableList = outPartitionedImmutableList with
        {
            PartitionList = outPartitionList
        };
        
        var outPartitionMemoryMap = outPartitionedImmutableList.PartitionMemoryMap
            .InsertRange(index + 1, new int[partitionNewList.Count]);

        for (int i = index; i < (index + EXPANSION_FACTOR); i++)
        {
            var partition = outPartitionedImmutableList.PartitionList[i];
            outPartitionMemoryMap = outPartitionMemoryMap.SetItem(i, partition.Count);
        }

        return outPartitionedImmutableList with
        {
            PartitionMemoryMap = outPartitionMemoryMap
        };
    }
}
