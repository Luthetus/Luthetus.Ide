using Luthetus.TextEditor.RazorLib.Characters.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

internal class PartitionContainerModifier
{
    public int PartitionSize { get; }
    public ImmutableList<ImmutableList<RichCharacter>> PartitionList { get; set; }
    public ImmutableList<PartitionMetadata> PartitionMetadataMap { get; set; }
}
