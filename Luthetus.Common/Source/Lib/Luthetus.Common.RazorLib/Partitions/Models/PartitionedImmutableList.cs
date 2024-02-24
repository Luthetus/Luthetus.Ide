using System.Collections;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Partitions.Models;

public record PartitionedImmutableList<TItem> : IList<TItem> where TItem : notnull
{
    public PartitionedImmutableList(int partitionSize)
    {
        if (partitionSize < 2)
            throw new ApplicationException("TODO: Should this 'throw new exception' remain here? It is presumed, that a partitionSize < 2 being allowed could result in many 'checks' being necessary throughout the code. I don't have proof of this, I just have a 'worried feeling'");

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
    public ImmutableList<ImmutableList<TItem>> PartitionList { get; private init; } = ImmutableList<ImmutableList<TItem>>.Empty;

    /// <summary>
    /// Track the 'Count' of each partition in this.
    /// Therefore, one can lookup whether a partition is full or not.
    /// Without iterating through all the partitions just to check a specific one.
    /// <br/>
    /// The name 'Map' is used here because to get the Count of the 0th index partition,
    /// one would read the value at index 0 of this property.
    /// In otherwords, each partition index maps to its corresponding Count.
    /// </summary>
    public ImmutableList<int> PartitionMemoryMap { get; private init; } = ImmutableList<int>.Empty;

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

    public void Insert(int index, TItem item)
    {
        throw new NotImplementedException();
    }

    public bool Remove(TItem item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void ICollection<TItem>.Add(TItem item) => Add(item);
    void ICollection<TItem>.Clear() => Clear();
}
