using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class Track_RowTests : Track_Tests_Base
{
    #region Add
    [Fact]
    public override void Add_Start()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "\nab".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.AddRange(richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.RowEndingList);
        var rowEndingPosition = partitionMetadata.RowEndingList.Single();
        Assert.Equal(0, rowEndingPosition.StartPositionIndexInclusive);
    }

    [Fact]
    public override void Add_Middle()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "a\nb".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.AddRange(richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.RowEndingList);
        var rowEndingPosition = partitionMetadata.RowEndingList.Single();
        Assert.Equal(1, rowEndingPosition.StartPositionIndexInclusive);
    }

    [Fact]
    public override void Add_End()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.AddRange(richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.RowEndingList);
        var rowEndingPosition = partitionMetadata.RowEndingList.Single();
        Assert.Equal(2, rowEndingPosition.StartPositionIndexInclusive);
    }

    [Fact]
    public override void Add_Causes_Expansion()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.AddRange(richCharacterList);

        partitionContainer = partitionContainer.Add(
            new RichCharacter { Value = '\n' });

        var globalMetadataLazy = partitionContainer.GlobalMetadata;
        var globalRowEndingList = globalMetadataLazy.RowEndingList.Value;
        var globalAllText = globalMetadataLazy.AllText.Value;

        throw new NotImplementedException();
    }

    [Fact]
    public override void Add_Four_InARow()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = new string('\n', 4).Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.AddRange(richCharacterList);

        var globalMetadataLazy = partitionContainer.GlobalMetadata;
        var globalRowEndingList = globalMetadataLazy.RowEndingList.Value;
        var globalAllText = globalMetadataLazy.AllText.Value;

        throw new NotImplementedException();
    }
    #endregion

    #region Insert
    [Fact]
    public override void Insert_Start()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "\nab".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.InsertRange(0, richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.RowEndingList);
        var rowEndingPosition = partitionMetadata.RowEndingList.Single();
        Assert.Equal(0, rowEndingPosition.StartPositionIndexInclusive);
    }

    [Fact]
    public override void Insert_Middle()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "a\nb".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.InsertRange(0, richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.RowEndingList);
        var rowEndingPosition = partitionMetadata.RowEndingList.Single();
        Assert.Equal(1, rowEndingPosition.StartPositionIndexInclusive);
    }

    [Fact]
    public override void Insert_End()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.InsertRange(0, richCharacterList);

        Assert.Equal(
            new string(richCharacterList.Select(x => x.Value).ToArray()),
            new string(partitionContainer.Select(x => x.Value).ToArray()));

        Assert.Single(partitionContainer.PartitionMetadataMap);
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();

        Assert.Equal(3, partitionMetadata.Count);

        Assert.Single(partitionMetadata.RowEndingList);
        var rowEndingPosition = partitionMetadata.RowEndingList.Single();
        Assert.Equal(2, rowEndingPosition.StartPositionIndexInclusive);
    }

    [Fact]
    public override void Insert_Causes_Expansion()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.InsertRange(0, richCharacterList);

        partitionContainer = partitionContainer.Insert(
            0, new RichCharacter { Value = '\n' });

        var globalMetadataLazy = partitionContainer.GlobalMetadata;
        var globalRowEndingList = globalMetadataLazy.RowEndingList.Value;
        var globalAllText = globalMetadataLazy.AllText.Value;

        throw new NotImplementedException();
    }

    [Fact]
    public override void Insert_Four_InARow()
    {
        var partitionContainer = new PartitionContainer(3);
        var richCharacterList = new string('\n', 4).Select(x => new RichCharacter { Value = x }).ToArray();
        partitionContainer = partitionContainer.InsertRange(0, richCharacterList);

        // Reading some state so I see it in debugger
        {
            var globalMetadataLazy = partitionContainer.GlobalMetadata;
            var globalRowEndingList = globalMetadataLazy.RowEndingList.Value;
            var globalAllText = globalMetadataLazy.AllText.Value;
        }

        partitionContainer = partitionContainer.Insert(
            0, new RichCharacter { Value = '\n' });

        // Reading some state so I see it in debugger
        {
            var globalMetadataLazy = partitionContainer.GlobalMetadata;
            var globalRowEndingList = globalMetadataLazy.RowEndingList.Value;
            var globalAllText = globalMetadataLazy.AllText.Value;
        }

        throw new NotImplementedException();
    }
    #endregion
}
