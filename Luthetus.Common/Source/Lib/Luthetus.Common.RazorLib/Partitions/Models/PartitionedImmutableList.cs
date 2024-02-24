using System.Collections;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Partitions.Models;

public class PartitionedImmutableList<TItem> : IImmutableList<TItem>
{
    public PartitionedImmutableList(int partitionSize)
    {
        if (partitionSize < 2)
            throw new ApplicationException("TODO: Should this 'throw new exception' remain here? It is presumed, that a partitionSize < 2 being allowed could result in many 'checks' being necessary throughout the code. I don't have proof of this, I just have a 'worried feeling'");

        PartitionSize = partitionSize;
    }

    private ImmutableList<ImmutableList<TItem>> _partitionList = ImmutableList<ImmutableList<TItem>>.Empty;

    public int PartitionSize { get; }
    public int PartitionCount => _partitionList.Count;

    /// <summary>
    /// Track the 'Count' of each partition in this.
    /// Therefore, one can lookup whether a partition is full or not.
    /// Without iterating through all the partitions just to check a specific one.
    /// <br/>
    /// The name 'Map' is used here because to get the Count of the 0th index partition,
    /// one would read the value at index 0 of this property.
    /// In otherwords, each partition index maps to its corresponding Count.
    /// </summary>
    public ImmutableList<int> PartitionMemoryMap { get; private set; } = ImmutableList<int>.Empty;

    public TItem this[int index]
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public int Count
    {
        get
        {
            // Case 0 partitions
            if (PartitionCount == 0)
                return 0;

            var count = 0;

            foreach (var partition in _partitionList)
            {
                count += partition.Count;
            }

            return count;
        }
    }

    public IImmutableList<TItem> Add(TItem value)
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

            // Careful: one must change BOTH '_partitionList' and 'PartitionMemoryMap'
            {
                _partitionList = _partitionList.Add(partition);
                PartitionMemoryMap = PartitionMemoryMap.Add(partition.Count);
            }

            // Awkwardly I need to return 'IImmutableList<TItem>'.
            // For now return the partition which was changed with its item added.
            // TODO: Don't inherit IImmutableList? Its odd to have to return one from most methods.
            return partition;
        }
        else
        {
            var partition = _partitionList[indexPartitionFreeSpace];
            partition = partition.Add(value);

            // Careful: one must change BOTH '_partitionList' and 'PartitionMemoryMap'
            {
                _partitionList = _partitionList.SetItem(indexPartitionFreeSpace, partition);
                PartitionMemoryMap = PartitionMemoryMap.SetItem(indexPartitionFreeSpace, partition.Count);
            }

            // Awkwardly I need to return 'IImmutableList<TItem>'.
            // For now return the partition which was changed with its item added.
            // TODO: Don't inherit IImmutableList? Its odd to have to return one from most methods.
            return _partitionList[indexPartitionFreeSpace];
        }
    }

    public IImmutableList<TItem> AddRange(IEnumerable<TItem> items)
    {
        throw new NotImplementedException();
    }

    public IImmutableList<TItem> Clear()
    {
        throw new NotImplementedException();
    }

    public IEnumerator<TItem> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public int IndexOf(TItem item, int index, int count, IEqualityComparer<TItem>? equalityComparer)
    {
        throw new NotImplementedException();
    }

    public IImmutableList<TItem> Insert(int index, TItem element)
    {
        throw new NotImplementedException();
    }

    public IImmutableList<TItem> InsertRange(int index, IEnumerable<TItem> items)
    {
        throw new NotImplementedException();
    }

    public int LastIndexOf(TItem item, int index, int count, IEqualityComparer<TItem>? equalityComparer)
    {
        throw new NotImplementedException();
    }

    public IImmutableList<TItem> Remove(TItem value, IEqualityComparer<TItem>? equalityComparer)
    {
        throw new NotImplementedException();
    }

    public IImmutableList<TItem> RemoveAll(Predicate<TItem> match)
    {
        throw new NotImplementedException();
    }

    public IImmutableList<TItem> RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public IImmutableList<TItem> RemoveRange(IEnumerable<TItem> items, IEqualityComparer<TItem>? equalityComparer)
    {
        throw new NotImplementedException();
    }

    public IImmutableList<TItem> RemoveRange(int index, int count)
    {
        throw new NotImplementedException();
    }

    public IImmutableList<TItem> Replace(TItem oldValue, TItem newValue, IEqualityComparer<TItem>? equalityComparer)
    {
        throw new NotImplementedException();
    }

    public IImmutableList<TItem> SetItem(int index, TItem value)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
