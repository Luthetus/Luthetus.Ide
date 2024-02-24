using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Partitions.Models;

//public record PartitionedImmutableList<TItem> : IReadOnlyList<TItem> where TItem : notnull
//{
//    public PartitionedImmutableList(int partitionSize)
//    {
//        if (partitionSize < 2)
//            throw new ApplicationException("TODO: Should this 'throw new exception' remain here? It is presumed, that a partitionSize < 2 being allowed could result in many 'checks' being necessary throughout the code. I don't have proof of this, I just have a 'worried feeling'");

//        PartitionSize = partitionSize;
//    }

//    public int PartitionSize { get; }
//    public ImmutableList<ImmutableList<TItem>> PartitionList { get; private init; } = ImmutableList<ImmutableList<TItem>>.Empty;

//    /// <summary>
//    /// Track the 'Count' of each partition in this.
//    /// Therefore, one can lookup whether a partition is full or not.
//    /// Without iterating through all the partitions just to check a specific one.
//    /// <br/>
//    /// The name 'Map' is used here because to get the Count of the 0th index partition,
//    /// one would read the value at index 0 of this property.
//    /// In otherwords, each partition index maps to its corresponding Count.
//    /// </summary>
//    public ImmutableList<int> PartitionMemoryMap { get; private init; } = ImmutableList<int>.Empty;



//    public PartitionedImmutableList<TItem> Insert(int index, TItem element)
//    {
//        throw new NotImplementedException();
//    }

//    public PartitionedImmutableList<TItem> InsertRange(int index, IEnumerable<TItem> items)
//    {
//        throw new NotImplementedException();
//    }

//    public int LastIndexOf(TItem item, int index, int count, IEqualityComparer<TItem>? equalityComparer)
//    {
//        throw new NotImplementedException();
//    }

//    public PartitionedImmutableList<TItem> Remove(TItem value, IEqualityComparer<TItem>? equalityComparer)
//    {
//        throw new NotImplementedException();
//    }

//    public PartitionedImmutableList<TItem> RemoveAll(Predicate<TItem> match)
//    {
//        throw new NotImplementedException();
//    }

//    public PartitionedImmutableList<TItem> RemoveAt(int index)
//    {
//        throw new NotImplementedException();
//    }

//    public PartitionedImmutableList<TItem> RemoveRange(IEnumerable<TItem> items, IEqualityComparer<TItem>? equalityComparer)
//    {
//        throw new NotImplementedException();
//    }

//    public PartitionedImmutableList<TItem> RemoveRange(int index, int count)
//    {
//        throw new NotImplementedException();
//    }

//    public PartitionedImmutableList<TItem> Replace(TItem oldValue, TItem newValue, IEqualityComparer<TItem>? equalityComparer)
//    {
//        throw new NotImplementedException();
//    }

//    public PartitionedImmutableList<TItem> SetItem(int index, TItem value)
//    {
//        throw new NotImplementedException();
//    }
//}

public static class PartitionedImmutableListExtensionMethods
{
    public static PartitionedImmutableList<TSource> ToPartitionedImmutableList<TSource>(
            this IEnumerable<TSource> source,
            int partitionSize)
        where TSource : notnull
    {
        var existingList = source as PartitionedImmutableList<TSource>;
        if (existingList != null)
        {
            return existingList;
        }

        return new PartitionedImmutableList<TSource>(partitionSize).AddRange(existingList);
    }
}