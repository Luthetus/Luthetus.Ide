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
        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Empty(partitionMetadata.TabList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Empty(partitionMetadata.TabList);
            }
            { // Partition Three
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
        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Single(partitionMetadata.TabList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Single(partitionMetadata.TabList);
            }
            { // Partition Three
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
        var partitionContainer = new PartitionContainer(3).AddRange("ab".Select(x => new RichCharacter { Value = x }));
        partitionContainer = partitionContainer.Insert(0, new RichCharacter { Value = '\t' });
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(0, partitionMetadata.TabList.Single());
    }

    [Fact]
    public override void Insert_At_Middle()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).AddRange("ab".Select(x => new RichCharacter { Value = x }));
        partitionContainer = partitionContainer.Insert(1, new RichCharacter { Value = '\t' });
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(1, partitionMetadata.TabList.Single());
    }

    [Fact]
    public override void Insert_At_End()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).AddRange("ab".Select(x => new RichCharacter { Value = x }));
        partitionContainer = partitionContainer.Insert(2, new RichCharacter { Value = '\t' });
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
        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Single(partitionMetadata.TabList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Empty(partitionMetadata.TabList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Single(partitionMetadata.TabList);
            }
        }
    }

    [Fact]
    public override void Insert_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\t', 3).Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).InsertRange(0, richCharacterList);
        partitionContainer = partitionContainer.Insert(0, new RichCharacter { Value = '\t' });
        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Equal(2, partitionMetadata.TabList.Count);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Single(partitionMetadata.TabList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Single(partitionMetadata.TabList);
            }
        }
    }
    #endregion

    #region Remove
    [Fact]
    public override void Remove_At_Start()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).InsertRange(0, "\tab".Select(x => new RichCharacter { Value = x }));
        // Pre assertion to ensure a before and after measurement
        Assert.Equal(0, partitionContainer.PartitionMetadataMap.Single().TabList.Single());
        partitionContainer = partitionContainer.RemoveAt(0);
        // Assert
        Assert.Empty(partitionContainer.PartitionMetadataMap.Single().TabList);
    }

    [Fact]
    public override void Remove_At_Middle()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).InsertRange(0, "a\tb".Select(x => new RichCharacter { Value = x }));
        // Pre assertion to ensure a before and after measurement
        Assert.Equal(1, partitionContainer.PartitionMetadataMap.Single().TabList.Single());
        partitionContainer = partitionContainer.RemoveAt(1);
        // Assert
        Assert.Empty(partitionContainer.PartitionMetadataMap.Single().TabList);
    }

    [Fact]
    public override void Remove_At_End()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).InsertRange(0, "ab\t".Select(x => new RichCharacter { Value = x }));
        // Pre assertion to ensure a before and after measurement
        Assert.Equal(2, partitionContainer.PartitionMetadataMap.Single().TabList.Single());
        partitionContainer = partitionContainer.RemoveAt(2);
        // Assert
        Assert.Empty(partitionContainer.PartitionMetadataMap.Single().TabList);
    }

    [Fact]
    public override void Remove_Causes_Empty_Partition()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).InsertRange(0, "ab\t\t".Select(x => new RichCharacter { Value = x }));
        // Pre assertion to ensure a before and after measurement
        Assert.Equal(2, partitionContainer.PartitionMetadataMap[0].TabList.Single()); // Tab in first partition
        Assert.Equal(0, partitionContainer.PartitionMetadataMap[1].TabList.Single()); // Tab in second partition
        partitionContainer = partitionContainer.RemoveAt(3); // Remove the tab from second partition
        // Assert
        Assert.Empty(partitionContainer.PartitionMetadataMap[1].TabList); // Second partition does NOT have any tabs.
    }

    [Fact]
    public override void Remove_Four_InARow()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).InsertRange(0, new string('\t', 4).Select(x => new RichCharacter { Value = x }));
        partitionContainer = partitionContainer.RemoveRange(0, 4);
        // Assert
        throw new NotImplementedException();
    }
    #endregion
}
