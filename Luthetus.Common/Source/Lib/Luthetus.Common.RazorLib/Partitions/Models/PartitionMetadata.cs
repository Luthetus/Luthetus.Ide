using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.RazorLib.Partitions.Models;

public record PartitionMetadata
{
    public PartitionMetadata(int count)
    {
        Count = count;
    }

    public int Count { get; set; }
}
