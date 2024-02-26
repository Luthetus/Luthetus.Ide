using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class Track_Row : Track_Tests_Base
{
    #region Add
    [Fact]
    public override void Add_At_Start()
    {
        // Setup
        var richCharacterList = "\nab".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(0, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Add_At_Middle()
    {
        // Setup
        var richCharacterList = "a\nb".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(1, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Add_At_End()
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
        {
            // Partition One
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Empty(partitionMetadata.RowEndingList);
            }
            // Partition Two
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Empty(partitionMetadata.RowEndingList);
            }
            // Partition Three
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Equal(2, partitionMetadata.RowEndingList.Count);
            }
        }
    }

    [Fact]
    public override void Add_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\n', 4).Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        {
            // Partition One
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            // Partition Two
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            // Partition Three
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Equal(2, partitionMetadata.RowEndingList.Count);
            }
        }
    }
    #endregion

    #region Insert
    [Fact]
    public override void Insert_At_Start()
    {
        // Setup
        var richCharacterList = "\nab".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(0, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Insert_At_Middle()
    {
        // Setup
        var richCharacterList = "a\nb".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(1, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Insert_At_End()
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
        {
            // Partition One
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Empty(partitionMetadata.RowEndingList);
            }
            // Partition Two
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Empty(partitionMetadata.RowEndingList);
            }
            // Partition Three
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Equal(2, partitionMetadata.RowEndingList.Count);
            }
        }
    }

    [Fact]
    public override void Insert_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\n', 3).Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        partitionContainer = partitionContainer.Insert(0, new RichCharacter { Value = '\n' });
        // Assert
        {
            // Partition One
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Equal(2, partitionMetadata.RowEndingList.Count);
            }
            // Partition Two
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            // Partition Three
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Single(partitionMetadata.RowEndingList);
            }
        }
    }
    #endregion
}
