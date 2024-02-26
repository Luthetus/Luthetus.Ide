/// TODO: I need to handle the partition meta data differently for the text editor...
/// ...Perhaps it would be preferable to have a generic <see cref="PartitionedImmutableList{TItem}"/>
/// type, but I'm finding a it hard to do so without odd looking and confusing code.
/// So, I'm going to copy and paste the attempt at the generic type here, then just
/// change the source code to work for the text editor. (2024-02-25)

//namespace Luthetus.Common.RazorLib.Partitions.Models;

//public record PartitionMetadata
//{
//    public PartitionMetadata(int count)
//    {
//        Count = count;
//    }

//    public int Count { get; set; }
//}
