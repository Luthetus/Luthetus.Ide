using System.Collections;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Partitions.Models;

public record PartitionedImmutableList<TItem> : IReadOnlyList<TItem> where TItem : notnull
{
    public PartitionedImmutableList(int partitionSize)
    {
        if (partitionSize < 2)
            throw new ApplicationException("TODO: Should this 'throw new exception' remain here? It is presumed, that a partitionSize < 2 being allowed could result in many 'checks' being necessary throughout the code. I don't have proof of this, I just have a 'worried feeling'");

        PartitionSize = partitionSize;
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
    }

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

    public PartitionedImmutableList<TItem> AddRange(IEnumerable<TItem> items)
    {
        throw new NotImplementedException();
    }

    public PartitionedImmutableList<TItem> Clear()
    {
        throw new NotImplementedException();
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

    public int IndexOf(TItem item, int index, int count, IEqualityComparer<TItem>? equalityComparer)
    {
        throw new NotImplementedException();
    }

    public PartitionedImmutableList<TItem> Insert(int index, TItem element)
    {
        throw new NotImplementedException();
    }

    public PartitionedImmutableList<TItem> InsertRange(int index, IEnumerable<TItem> items)
    {
        throw new NotImplementedException();
    }

    public int LastIndexOf(TItem item, int index, int count, IEqualityComparer<TItem>? equalityComparer)
    {
        throw new NotImplementedException();
    }

    public PartitionedImmutableList<TItem> Remove(TItem value, IEqualityComparer<TItem>? equalityComparer)
    {
        throw new NotImplementedException();
    }

    public PartitionedImmutableList<TItem> RemoveAll(Predicate<TItem> match)
    {
        throw new NotImplementedException();
    }

    public PartitionedImmutableList<TItem> RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public PartitionedImmutableList<TItem> RemoveRange(IEnumerable<TItem> items, IEqualityComparer<TItem>? equalityComparer)
    {
        throw new NotImplementedException();
    }

    public PartitionedImmutableList<TItem> RemoveRange(int index, int count)
    {
        throw new NotImplementedException();
    }

    public PartitionedImmutableList<TItem> Replace(TItem oldValue, TItem newValue, IEqualityComparer<TItem>? equalityComparer)
    {
        throw new NotImplementedException();
    }

    public PartitionedImmutableList<TItem> SetItem(int index, TItem value)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
