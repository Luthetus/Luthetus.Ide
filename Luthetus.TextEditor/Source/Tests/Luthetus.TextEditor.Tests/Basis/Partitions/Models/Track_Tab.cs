using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class Track_Tab : Track_Tests_Base
{
    #region Add
    [Fact]
    public override void Add_At_Start()
    {
        // Setup
        var richCharacterList = "\tab".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(0, partitionMetadata.TabList.Single());
    }

    [Fact]
    public override void Add_At_Middle()
    {
        // Setup
        var richCharacterList = "a\tb".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(1, partitionMetadata.TabList.Single());
    }

    [Fact]
    public override void Add_At_End()
    {
        // Setup
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(2, partitionMetadata.TabList.Single());
    }

    [Fact]
    public override void Add_Causes_Expansion()
    {
        // Setup
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        partitionContainer = partitionContainer.Add(new RichCharacter { Value = '\t' });
        // Assert
        {
            // Partition One
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Empty(partitionMetadata.TabList);
            }
            // Partition Two
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Empty(partitionMetadata.TabList);
            }
            // Partition Three
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Equal(2, partitionMetadata.TabList.Count);
            }
        }
    }

    [Fact]
    public override void Add_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\t', 4).Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        // Assert
        {
            // Partition One
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Single(partitionMetadata.TabList);
            }
            // Partition Two
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Single(partitionMetadata.TabList);
            }
            // Partition Three
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Equal(2, partitionMetadata.TabList.Count);
            }
        }
    }
    #endregion

    #region Insert
    [Fact]
    public override void Insert_At_Start()
    {
        // Setup
        var richCharacterList = "\tab".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(0, partitionMetadata.TabList.Single());
    }

    [Fact]
    public override void Insert_At_Middle()
    {
        // Setup
        var richCharacterList = "a\tb".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(1, partitionMetadata.TabList.Single());
    }

    [Fact]
    public override void Insert_At_End()
    {
        // Setup
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(2, partitionMetadata.TabList.Single());
    }

    [Fact]
    public override void Insert_Causes_Expansion()
    {
        // Setup
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        partitionContainer = partitionContainer.Insert(0, new RichCharacter { Value = '\t' });
        // Assert
        {
            // Partition One
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Single(partitionMetadata.TabList);
            }
            // Partition Two
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Empty(partitionMetadata.TabList);
            }
            // Partition Three
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Single(partitionMetadata.TabList);
            }
        }
    }

    [Fact]
    public override void Insert_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\t', 4).Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        partitionContainer = partitionContainer.Insert(0, new RichCharacter { Value = '\t' });
        // Assert
        {
            // Partition One
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Empty(partitionMetadata.TabList);
            }
            // Partition Two
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Empty(partitionMetadata.TabList);
            }
            // Partition Three
            {
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Equal(2, partitionMetadata.TabList.Count);
            }
        }
    }
    #endregion
}
