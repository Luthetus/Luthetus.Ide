using Luthetus.TextEditor.RazorLib.Characters.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

internal class PartitionContainerModifier
{
    public int PartitionSize { get; }
    public ImmutableList<ImmutableList<RichCharacter>> PartitionList { get; set; }
    public ImmutableList<PartitionMetadata> PartitionMetadataMap { get; set; }
}
