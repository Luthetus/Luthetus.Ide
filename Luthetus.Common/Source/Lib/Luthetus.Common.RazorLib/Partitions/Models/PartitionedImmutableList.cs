using System.Collections;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Partitions.Models;

public class PartitionedImmutableList<TItem> : IImmutableList<TItem>
{
    private readonly int _partitionSize;

    public PartitionedImmutableList(int partitionSize)
    {
        _partitionSize = partitionSize;
    }

    private ImmutableList<ImmutableList<TItem>> _items = ImmutableList<ImmutableList<TItem>>.Empty;

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
            throw new NotImplementedException();
        }
    }

    public IImmutableList<TItem> Add(TItem value)
    {
        throw new NotImplementedException();
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
