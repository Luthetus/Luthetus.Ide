using Luthetus.Common.RazorLib.Partitions.Models;

namespace Luthetus.Common.Tests.Basis.Partitions.Models;

public class PartitionedImmutableListTests
{
    [Fact]
    public void Constructor()
    {
        var partitionSize = 5_000;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);
        Assert.Equal(partitionSize, partitionedImmutableList.PartitionSize);
    }

    [Fact]
    public void Count()
    {
        //public int Count => throw new NotImplementedException();
        throw new NotImplementedException();
    }

    [Fact]
    public void IndexOperator()
    {
        //public TItem this[int index] => throw new NotImplementedException();
        throw new NotImplementedException();
    }

    [Fact]
    public void GetEnumerator()
    {
        //public IEnumerator<TextEditorDiagnostic> GetEnumerator()
        throw new NotImplementedException();
    }

    [Fact]
    public void Add()
    {
        var partitionSize = 5;
        var partitionedImmutableList = new PartitionedImmutableList<char>(partitionSize);
        Assert.Equal(partitionSize, partitionedImmutableList.PartitionSize);

        // Invoke 'Add(...)' one more times than what the partitionSize is.
        // where each invocation adds a character.
        // Therefore, the final invocation one can assert that a new partition was
        // made to hold that final character.
        for (int i = 0; i <= partitionSize; i++)
        {
            partitionedImmutableList.Add('a');
        }

        throw new NotImplementedException();
    }

    [Fact]
    public void AddRange()
    {
        //public IImmutableList<TItem> AddRange(IEnumerable<TItem> items)
        throw new NotImplementedException();
    }

    [Fact]
    public void Clear()
    {
        //public IImmutableList<TItem> Clear()
        throw new NotImplementedException();
    }

    [Fact]
    public void IndexOf()
    {
        //public int IndexOf(TItem item, int index, int count, IEqualityComparer<TItem>? equalityComparer)
        throw new NotImplementedException();
    }

    [Fact]
    public void Insert()
    {
        //public IImmutableList<TItem> Insert(int index, TItem element)
        throw new NotImplementedException();
    }

    [Fact]
    public void InsertRange()
    {
        //public IImmutableList<TItem> InsertRange(int index, IEnumerable<TItem> items)
        throw new NotImplementedException();
    }

    [Fact]
    public void LastIndexOf()
    {
        //public int LastIndexOf(TItem item, int index, int count, IEqualityComparer<TItem>? equalityComparer)
        throw new NotImplementedException();
    }

    [Fact]
    public void Remove()
    {
        //public IImmutableList<TItem> Remove(TItem value, IEqualityComparer<TItem>? equalityComparer)
        throw new NotImplementedException();
    }

    [Fact]
    public void RemoveAll()
    {
        //public IImmutableList<TItem> RemoveAll(Predicate<TItem> match)
        throw new NotImplementedException();
    }

    [Fact]
    public void RemoveAt()
    {
        //public IImmutableList<TItem> RemoveAt(int index)
        throw new NotImplementedException();
    }

    [Fact]
    public void RemoveRange_A()
    {
        //public IImmutableList<TItem> RemoveRange(IEnumerable<TItem> items, IEqualityComparer<TItem>? equalityComparer)
        throw new NotImplementedException();
    }

    [Fact]
    public void RemoveRange_B()
    {
        //public IImmutableList<TItem> RemoveRange(int index, int count)
        throw new NotImplementedException();
    }

    [Fact]
    public void Replace()
    {
        //public IImmutableList<TItem> Replace(TItem oldValue, TItem newValue, IEqualityComparer<TItem>? equalityComparer)
        throw new NotImplementedException();
    }

    [Fact]
    public void SetItem()
    {
        //public IImmutableList<TItem> SetItem(int index, TItem value)
        throw new NotImplementedException();
    }

    [Fact]
    public void ExplicitInterfaceGetEnumerator()
    {
        //IEnumerator IEnumerable.GetEnumerator()
        throw new NotImplementedException();
    }
}
