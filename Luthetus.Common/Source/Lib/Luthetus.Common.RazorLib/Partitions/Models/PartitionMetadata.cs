namespace Luthetus.Common.RazorLib.Partitions.Models;

public record PartitionMetadata
{
    public PartitionMetadata(int count)
    {
        Count = count;
    }

    public int Count { get; set; }
}
