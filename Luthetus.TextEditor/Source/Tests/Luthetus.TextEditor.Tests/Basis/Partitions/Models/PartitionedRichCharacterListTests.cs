using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class PartitionedRichCharacterListTests
{
    [Fact]
    public void Add_Tab_One_Partitions_Start()
    {
        var partitionedRichCharacterList = new PartitionedRichCharacterList(3);
        var richCharacterList = "\tab".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionedRichCharacterList = partitionedRichCharacterList.AddRange(richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionedRichCharacterList.Select(x => x.Value).ToArray()));

        Assert.Single(partitionedRichCharacterList.PartitionRichCharacterMetadataMap);
        var partitionRichCharacterMetadata = partitionedRichCharacterList.PartitionRichCharacterMetadataMap.Single();

        Assert.Equal(3, partitionRichCharacterMetadata.Count);

        Assert.Single(partitionRichCharacterMetadata.TabKeyPositionsList);
        var tabKeyPosition = partitionRichCharacterMetadata.TabKeyPositionsList.Single();
        Assert.Equal(0, tabKeyPosition);
    }

    [Fact]
    public void Add_Tab_One_Partitions_Middle()
    {
        var partitionedRichCharacterList = new PartitionedRichCharacterList(3);
        var richCharacterList = "a\tb".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionedRichCharacterList = partitionedRichCharacterList.AddRange(richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionedRichCharacterList.Select(x => x.Value).ToArray()));

        Assert.Single(partitionedRichCharacterList.PartitionRichCharacterMetadataMap);
        var partitionRichCharacterMetadata = partitionedRichCharacterList.PartitionRichCharacterMetadataMap.Single();

        Assert.Equal(3, partitionRichCharacterMetadata.Count);

        Assert.Single(partitionRichCharacterMetadata.TabKeyPositionsList);
        var tabKeyPosition = partitionRichCharacterMetadata.TabKeyPositionsList.Single();
        Assert.Equal(1, tabKeyPosition);
    }

    [Fact]
    public void Add_Tab_One_Partitions_End()
    {
        var partitionedRichCharacterList = new PartitionedRichCharacterList(3);
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionedRichCharacterList = partitionedRichCharacterList.AddRange(richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionedRichCharacterList.Select(x => x.Value).ToArray()));

        Assert.Single(partitionedRichCharacterList.PartitionRichCharacterMetadataMap);
        var partitionRichCharacterMetadata = partitionedRichCharacterList.PartitionRichCharacterMetadataMap.Single();

        Assert.Equal(3, partitionRichCharacterMetadata.Count);

        Assert.Single(partitionRichCharacterMetadata.TabKeyPositionsList);
        var tabKeyPosition = partitionRichCharacterMetadata.TabKeyPositionsList.Single();
        Assert.Equal(2, tabKeyPosition);
    }

    [Fact]
    public void Insert_Tab_One_Partitions_Start()
    {
        var partitionedRichCharacterList = new PartitionedRichCharacterList(3);
        var richCharacterList = "\tab".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionedRichCharacterList = partitionedRichCharacterList.InsertRange(0, richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionedRichCharacterList.Select(x => x.Value).ToArray()));

        Assert.Single(partitionedRichCharacterList.PartitionRichCharacterMetadataMap);
        var partitionRichCharacterMetadata = partitionedRichCharacterList.PartitionRichCharacterMetadataMap.Single();

        Assert.Equal(3, partitionRichCharacterMetadata.Count);

        Assert.Single(partitionRichCharacterMetadata.TabKeyPositionsList);
        var tabKeyPosition = partitionRichCharacterMetadata.TabKeyPositionsList.Single();
        Assert.Equal(0, tabKeyPosition);
    }

    [Fact]
    public void Insert_Tab_One_Partitions_Middle()
    {
        var partitionedRichCharacterList = new PartitionedRichCharacterList(3);
        var richCharacterList = "a\tb".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionedRichCharacterList = partitionedRichCharacterList.InsertRange(0, richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionedRichCharacterList.Select(x => x.Value).ToArray()));

        Assert.Single(partitionedRichCharacterList.PartitionRichCharacterMetadataMap);
        var partitionRichCharacterMetadata = partitionedRichCharacterList.PartitionRichCharacterMetadataMap.Single();

        Assert.Equal(3, partitionRichCharacterMetadata.Count);

        Assert.Single(partitionRichCharacterMetadata.TabKeyPositionsList);
        var tabKeyPosition = partitionRichCharacterMetadata.TabKeyPositionsList.Single();
        Assert.Equal(1, tabKeyPosition);
    }

    [Fact]
    public void Insert_Tab_One_Partitions_End()
    {
        var partitionedRichCharacterList = new PartitionedRichCharacterList(3);
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionedRichCharacterList = partitionedRichCharacterList.InsertRange(0, richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionedRichCharacterList.Select(x => x.Value).ToArray()));

        Assert.Single(partitionedRichCharacterList.PartitionRichCharacterMetadataMap);
        var partitionRichCharacterMetadata = partitionedRichCharacterList.PartitionRichCharacterMetadataMap.Single();

        Assert.Equal(3, partitionRichCharacterMetadata.Count);

        Assert.Single(partitionRichCharacterMetadata.TabKeyPositionsList);
        var tabKeyPosition = partitionRichCharacterMetadata.TabKeyPositionsList.Single();
        Assert.Equal(2, tabKeyPosition);
    }
}
