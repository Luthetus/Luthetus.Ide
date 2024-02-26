using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class Track_RowTests : Track_Tests_Base
{
    #region Add
    [Fact]
    public override void Add_Start()
    {
        // Setup
        var richCharacterList = "\nab".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(0, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Add_Middle()
    {
        // Setup
        var richCharacterList = "a\nb".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(1, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Add_End()
    {
        // Setup
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(2, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Add_Causes_Expansion()
    {
        // Setup
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        partitionContainer = partitionContainer.Add(new RichCharacter { Value = '\n' });
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public override void Add_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\n', 4).Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        throw new NotImplementedException();
    }
    #endregion

    #region Insert
    [Fact]
    public override void Insert_Start()
    {
        // Setup
        var richCharacterList = "\nab".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(0, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Insert_Middle()
    {
        // Setup
        var richCharacterList = "a\nb".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(1, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Insert_End()
    {
        // Setup
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(2, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Insert_Causes_Expansion()
    {
        // Setup
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        partitionContainer = partitionContainer.Insert(0, new RichCharacter { Value = '\n' });
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public override void Insert_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\n', 4).Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        partitionContainer = partitionContainer.Insert(0, new RichCharacter { Value = '\n' });
        // Assert
        throw new NotImplementedException();
    }
    #endregion
}
