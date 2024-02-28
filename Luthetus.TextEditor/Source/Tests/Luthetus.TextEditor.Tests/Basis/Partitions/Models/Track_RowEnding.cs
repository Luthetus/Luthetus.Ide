using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class Track_RowEnding : Track_Tests_Base
{
    #region Add
    [Fact]
    public override void Add()
    {
        // Goal A: Use a for loop to add a row ending to the: start, middle and, end of the 'otherCharacters' string.
        // Goal B: Use an inner for loop to iterate over all possible row ending characters one can type.
        // Goal C: Assert that the row endings were properly tracked.
        //
        //  ab            //  ab            //  ab
        //  ^             //   ^            //    ^
        //  Add at 0      //  Add at 1      //  Add at 2

        var otherCharacters = "ab";

        for (int i = 0; i < otherCharacters.Length; i++)
        {
            foreach (var rowEndingKind in RowEndingKind.Unset.GetRowEndingsUserAllowedToUse())
            {
                var rowEndingCharacters = rowEndingKind.AsCharacters();
                var richCharacterList = otherCharacters.Insert(i, rowEndingCharacters).Select(x => new RichCharacter { Value = x });
                var partitionContainer = new PartitionContainer(5_000).AddRange(richCharacterList);

                // PartitionMetadata
                {
                    var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
                    // RowEndingList
                    {
                        var insertedRowEnding = partitionMetadata.RowEndingList[0];
                        Assert.Equal(i, insertedRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(i + rowEndingCharacters.Length, insertedRowEnding.EndPositionIndexExclusive);

                        var endOfFileRowEnding = partitionMetadata.RowEndingList[1];
                        var endOfFileRowEndingRelativePositionIndex = rowEndingCharacters.Length + otherCharacters.Length;
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.EndPositionIndexExclusive);
                    }
                    // RowEndingKindCountList
                    foreach (var rowEndingKindCount in partitionMetadata.RowEndingKindCountList)
                    {
                        if (rowEndingKindCount.rowEndingKind == rowEndingKind)
                            Assert.Equal(1, rowEndingKindCount.count);
                        else
                            Assert.Equal(0, rowEndingKindCount.count);
                    }
                    // OnlyRowEndingKind
                    Assert.Equal(rowEndingKind, partitionMetadata.OnlyRowEndingKind);
                }
                // GlobalMetadata
                {
                    var globalMetadata = partitionContainer.GlobalMetadata;
                    // RowEndingList
                    {
                        var insertedRowEnding = globalMetadata.RowEndingList.Value[0];
                        Assert.Equal(i, insertedRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(i + rowEndingCharacters.Length, insertedRowEnding.EndPositionIndexExclusive);

                        var endOfFileRowEnding = globalMetadata.RowEndingList.Value[1];
                        var endOfFileRowEndingRelativePositionIndex = rowEndingCharacters.Length + otherCharacters.Length;
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.EndPositionIndexExclusive);
                    }
                    // RowEndingKindCountList
                    foreach (var rowEndingKindCount in globalMetadata.RowEndingKindCountList.Value)
                    {
                        if (rowEndingKindCount.rowEndingKind == rowEndingKind)
                            Assert.Equal(1, rowEndingKindCount.count);
                        else
                            Assert.Equal(0, rowEndingKindCount.count);
                    }
                    // OnlyRowEndingKind
                    Assert.Equal(rowEndingKind, partitionContainer.GlobalMetadata.OnlyRowEndingKind.Value);
                }
            }
        }
    }

    [Fact]
    public override void Add_Causes_Expansion()
    {
        // Setup
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();
        var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
        partitionContainer = partitionContainer.Add(new RichCharacter { Value = '\n' });
        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Empty(partitionMetadata.RowEndingList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Empty(partitionMetadata.RowEndingList);
            }
            { // Partition Three
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
        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            { // Partition Three
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
        var partitionContainer = new PartitionContainer(3).AddRange("ab".Select(x => new RichCharacter { Value = x }));
        partitionContainer = partitionContainer.Insert(0, new RichCharacter { Value = '\n' });
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(0, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Insert_At_Middle()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).AddRange("ab".Select(x => new RichCharacter { Value = x }));
        partitionContainer = partitionContainer.Insert(1, new RichCharacter { Value = '\n' });
        // Assert
        var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
        Assert.Equal(1, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
    }

    [Fact]
    public override void Insert_At_End()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).AddRange("ab".Select(x => new RichCharacter { Value = x }));
        partitionContainer = partitionContainer.Insert(2, new RichCharacter { Value = '\n' });
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
        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Empty(partitionMetadata.RowEndingList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Single(partitionMetadata.RowEndingList);
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
        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                Assert.Equal(2, partitionMetadata.RowEndingList.Count);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadataMap[2];
                Assert.Single(partitionMetadata.RowEndingList);
            }
        }
    }
    #endregion
    
    #region Remove
    [Fact]
    public override void Remove_At_Start()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).InsertRange(0, "\nab".Select(x => new RichCharacter { Value = x }));
        // Pre assertion to ensure a before and after measurement
        Assert.Equal(0, partitionContainer.PartitionMetadataMap.Single().RowEndingList.Single().StartPositionIndexInclusive);
        Assert.Equal(1, partitionContainer.PartitionMetadataMap.Single().RowEndingList.Single().EndPositionIndexExclusive);
        partitionContainer = partitionContainer.RemoveAt(0);
        // Assert
        Assert.Empty(partitionContainer.PartitionMetadataMap.Single().TabList);
    }

    [Fact]
    public override void Remove_At_Middle()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).InsertRange(0, "a\nb".Select(x => new RichCharacter { Value = x }));
        // Pre assertion to ensure a before and after measurement
        Assert.Equal(1, partitionContainer.PartitionMetadataMap.Single().RowEndingList.Single().StartPositionIndexInclusive);
        Assert.Equal(2, partitionContainer.PartitionMetadataMap.Single().RowEndingList.Single().EndPositionIndexExclusive);
        partitionContainer = partitionContainer.RemoveAt(1);
        // Assert
        Assert.Empty(partitionContainer.PartitionMetadataMap.Single().TabList);
    }

    [Fact]
    public override void Remove_At_End()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).InsertRange(0, "ab\n".Select(x => new RichCharacter { Value = x }));
        // Pre assertion to ensure a before and after measurement
        Assert.Equal(2, partitionContainer.PartitionMetadataMap.Single().RowEndingList.Single().StartPositionIndexInclusive);
        Assert.Equal(3, partitionContainer.PartitionMetadataMap.Single().RowEndingList.Single().EndPositionIndexExclusive);
        partitionContainer = partitionContainer.RemoveAt(2);
        // Assert
        Assert.Empty(partitionContainer.PartitionMetadataMap.Single().TabList);
    }

    [Fact]
    public override void Remove_Causes_Empty_Partition()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).InsertRange(0, "ab\n\n".Select(x => new RichCharacter { Value = x }));
        partitionContainer = partitionContainer.RemoveAt(3);
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public override void Remove_Four_InARow()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3).InsertRange(0, new string('\n', 4).Select(x => new RichCharacter { Value = x }));
        partitionContainer = partitionContainer.RemoveRange(0, 4);
        // Assert
        throw new NotImplementedException();
    }
    #endregion
}
