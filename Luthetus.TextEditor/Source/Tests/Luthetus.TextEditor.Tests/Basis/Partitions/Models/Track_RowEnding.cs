using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class Track_RowEnding : Track_Tests_Base
{
    #region Add
    [Fact]
    public override void Add_At_Start()
    {
        { // CarriageReturn
            // Setup
            var richCharacterList = $"\rab".Select(x => new RichCharacter { Value = x }).ToArray();
            var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
            { // Assert
                { // PartitionMetadata
                    var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
                    // RowEndingList
                    Assert.Equal(0, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
                    Assert.Equal(1, partitionMetadata.RowEndingList.Single().EndPositionIndexExclusive);
                    // RowEndingKindCountList
                    Assert.Equal((RowEndingKind.CarriageReturn, 1), partitionMetadata.RowEndingKindCountList[0]);
                    Assert.Equal((RowEndingKind.Linefeed, 0), partitionMetadata.RowEndingKindCountList[1]);
                    Assert.Equal((RowEndingKind.CarriageReturnLinefeed, 0), partitionMetadata.RowEndingKindCountList[2]);
                    // OnlyRowEndingKind
                    Assert.Equal(RowEndingKind.CarriageReturn, partitionMetadata.OnlyRowEndingKind);
                }
                { // GlobalMetadata
                    var globalMetadata = partitionContainer.GlobalMetadata;
                    // RowEndingList
                    Assert.Equal(0, globalMetadata.RowEndingList.Value.Single().StartPositionIndexInclusive);
                    Assert.Equal(1, globalMetadata.RowEndingList.Value.Single().EndPositionIndexExclusive);
                    // RowEndingKindCountList
                    Assert.Equal((RowEndingKind.CarriageReturn, 1), globalMetadata.RowEndingKindCountList.Value[0]);
                    Assert.Equal((RowEndingKind.Linefeed, 0), globalMetadata.RowEndingKindCountList.Value[1]);
                    Assert.Equal((RowEndingKind.CarriageReturnLinefeed, 0), globalMetadata.RowEndingKindCountList.Value[2]);
                    // OnlyRowEndingKind
                    Assert.Equal(RowEndingKind.CarriageReturn, partitionContainer.GlobalMetadata.OnlyRowEndingKind.Value);
                }
            }
        }
        { // Linefeed
            // Setup
            var richCharacterList = $"\nab".Select(x => new RichCharacter { Value = x }).ToArray();
            var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
            { // Assert
                { // PartitionMetadata
                    var partitionMetadata = partitionContainer.PartitionMetadataMap.Single();
                    // RowEndingList
                    Assert.Equal(0, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
                    Assert.Equal(1, partitionMetadata.RowEndingList.Single().EndPositionIndexExclusive);
                    // RowEndingKindCountList
                    Assert.Equal((RowEndingKind.CarriageReturn, 0), partitionMetadata.RowEndingKindCountList[0]);
                    Assert.Equal((RowEndingKind.Linefeed, 1), partitionMetadata.RowEndingKindCountList[1]);
                    Assert.Equal((RowEndingKind.CarriageReturnLinefeed, 0), partitionMetadata.RowEndingKindCountList[2]);
                    // OnlyRowEndingKind
                    Assert.Equal(RowEndingKind.Linefeed, partitionMetadata.OnlyRowEndingKind);
                }
                { // GlobalMetadata
                    var globalMetadata = partitionContainer.GlobalMetadata;
                    // RowEndingList
                    Assert.Equal(0, globalMetadata.RowEndingList.Value.Single().StartPositionIndexInclusive);
                    Assert.Equal(1, globalMetadata.RowEndingList.Value.Single().EndPositionIndexExclusive);
                    // RowEndingKindCountList
                    Assert.Equal((RowEndingKind.CarriageReturn, 0), globalMetadata.RowEndingKindCountList.Value[0]);
                    Assert.Equal((RowEndingKind.Linefeed, 1), globalMetadata.RowEndingKindCountList.Value[1]);
                    Assert.Equal((RowEndingKind.CarriageReturnLinefeed, 0), globalMetadata.RowEndingKindCountList.Value[2]);
                    // OnlyRowEndingKind
                    Assert.Equal(RowEndingKind.Linefeed, partitionContainer.GlobalMetadata.OnlyRowEndingKind.Value);
                }
            }
        }
        { // CarriageReturnLinefeed
            // Setup
            var richCharacterList = $"\r\nab".Select(x => new RichCharacter { Value = x }).ToArray();
            var partitionContainer = new PartitionContainer(3).AddRange(richCharacterList);
            { // Assert
                { // PartitionMetadata
                    { // First PartitionMetadata
                        var partitionMetadata = partitionContainer.PartitionMetadataMap[0];
                        // RowEndingList
                        Assert.Equal(0, partitionMetadata.RowEndingList.Single().StartPositionIndexInclusive);
                        Assert.Equal(2, partitionMetadata.RowEndingList.Single().EndPositionIndexExclusive);
                        // RowEndingKindCountList
                        Assert.Equal((RowEndingKind.CarriageReturn, 0), partitionMetadata.RowEndingKindCountList[0]);
                        Assert.Equal((RowEndingKind.Linefeed, 0), partitionMetadata.RowEndingKindCountList[1]);
                        Assert.Equal((RowEndingKind.CarriageReturnLinefeed, 1), partitionMetadata.RowEndingKindCountList[2]);
                        // OnlyRowEndingKind
                        Assert.Equal(RowEndingKind.CarriageReturnLinefeed, partitionMetadata.OnlyRowEndingKind);
                    }
                    { // Second PartitionMetadata
                        var partitionMetadata = partitionContainer.PartitionMetadataMap[1];
                        // RowEndingList
                        Assert.Empty(partitionMetadata.RowEndingList);
                        // RowEndingKindCountList
                        Assert.Equal((RowEndingKind.CarriageReturn, 0), partitionMetadata.RowEndingKindCountList[0]);
                        Assert.Equal((RowEndingKind.Linefeed, 0), partitionMetadata.RowEndingKindCountList[1]);
                        Assert.Equal((RowEndingKind.CarriageReturnLinefeed, 0), partitionMetadata.RowEndingKindCountList[2]);
                        // OnlyRowEndingKind
                        Assert.Null(partitionMetadata.OnlyRowEndingKind);
                    }
                }
                { // GlobalMetadata
                    var globalMetadata = partitionContainer.GlobalMetadata;
                    // RowEndingList
                    Assert.Equal(0, globalMetadata.RowEndingList.Value.Single().StartPositionIndexInclusive);
                    Assert.Equal(2, globalMetadata.RowEndingList.Value.Single().EndPositionIndexExclusive);
                    // RowEndingKindCountList
                    Assert.Equal((RowEndingKind.CarriageReturn, 0), globalMetadata.RowEndingKindCountList.Value[0]);
                    Assert.Equal((RowEndingKind.Linefeed, 0), globalMetadata.RowEndingKindCountList.Value[1]);
                    Assert.Equal((RowEndingKind.CarriageReturnLinefeed, 1), globalMetadata.RowEndingKindCountList.Value[2]);
                    // OnlyRowEndingKind
                    Assert.Equal(RowEndingKind.CarriageReturnLinefeed, partitionContainer.GlobalMetadata.OnlyRowEndingKind.Value);
                }
            }
        }
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
