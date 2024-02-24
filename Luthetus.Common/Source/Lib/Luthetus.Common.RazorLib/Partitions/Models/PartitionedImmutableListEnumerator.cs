using System.Collections;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Partitions.Models;

public class PartitionedImmutableListEnumerator<TItem> : IEnumerator<TItem> where TItem : notnull
{
    private readonly PartitionedImmutableList<TItem> _partitionedImmutableList;

    public PartitionedImmutableListEnumerator(PartitionedImmutableList<TItem> partitionedImmutableList)
    {
        _partitionedImmutableList = partitionedImmutableList;
        Reset();
    }

    private ImmutableList<ImmutableList<TItem>>.Enumerator _listPartitionEnumerator;
    private ImmutableList<TItem>.Enumerator? _partitionEnumerator;

    public TItem Current
    {
        get
        {
            if (_partitionEnumerator is null)
                throw new InvalidOperationException();

            if (_partitionEnumerator.Value.Current is null)
                throw new InvalidOperationException();

            return _partitionEnumerator.Value.Current;
        }
    }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (_partitionEnumerator is null)
            return TryMoveListPartition();
        else if (_partitionEnumerator.Value.MoveNext())
            return true;
        else
            return TryMoveListPartition();
    }

    public void Reset()
    {
        _listPartitionEnumerator = _partitionedImmutableList.PartitionList.GetEnumerator();
        _listPartitionEnumerator.Reset();
        TryMoveListPartition();
    }

    private bool TryMoveListPartition()
    {
        var success = _listPartitionEnumerator.MoveNext();

        if (success)
        {
            _partitionEnumerator = _listPartitionEnumerator.Current.GetEnumerator();
            _partitionEnumerator.Value.MoveNext();
        }
        else
        {
            _partitionEnumerator = null;
        }

        return success;
    }

    public void Dispose()
    {
        _listPartitionEnumerator.Dispose();
        _partitionEnumerator?.Dispose();
    }
}
