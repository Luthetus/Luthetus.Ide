namespace Luthetus.Common.RazorLib.Partitions.Models;

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

        return new PartitionedImmutableList<TSource>(partitionSize).AddRange(source);
    }
}