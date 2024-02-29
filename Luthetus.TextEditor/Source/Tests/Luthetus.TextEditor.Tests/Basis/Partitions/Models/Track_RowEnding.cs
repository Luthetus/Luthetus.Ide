using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class Track_RowEnding : Track_Tests_Base
{
    [Fact]
    public override void Add()
    {
        // Goal A: Use a for loop to add a row ending to the: start, middle and, end of the 'otherCharacters' string.
        // Goal B: Use an inner for loop to iterate over all possible row ending characters one can type.
        // Goal C: Assert that the row endings were properly tracked.
        //
        // ab          // ab          // ab
        // ^           //  ^          //   ^
        // Add at 0    // Add at 1    // Add at 2

        var otherCharacters = "ab";

        for (int i = 0; i < otherCharacters.Length; i++)
        {
            foreach (var rowEndingKind in RowEndingKind.Unset.GetRowEndingsUserAllowedToUse())
            {
                var rowEndingCharacters = rowEndingKind.AsCharacters();
                var richCharacterList = otherCharacters.Insert(i, rowEndingCharacters).Select(x => new RichCharacter { Value = x });

                var partitionContainer = new PartitionContainer(5_000)
                    .ToPartitionContainerModifier()
                    .AddRange(richCharacterList);

                // PartitionMetadata
                {
                    var partitionMetadata = partitionContainer.PartitionMetadata.Single();
                    // RowEndingList
                    {
                        var addedRowEnding = partitionMetadata.RowEndingList[0];
                        Assert.Equal(i, addedRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(i + rowEndingCharacters.Length, addedRowEnding.EndPositionIndexExclusive);

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
                    // RowEndingList
                    {
                        var addedRowEnding = partitionContainer.RowEndingList[0];
                        Assert.Equal(i, addedRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(i + rowEndingCharacters.Length, addedRowEnding.EndPositionIndexExclusive);

                        var endOfFileRowEnding = partitionContainer.RowEndingList[1];
                        var endOfFileRowEndingRelativePositionIndex = rowEndingCharacters.Length + otherCharacters.Length;
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.EndPositionIndexExclusive);
                    }
                    // RowEndingKindCountList
                    foreach (var rowEndingKindCount in partitionContainer.RowEndingKindCountList)
                    {
                        if (rowEndingKindCount.rowEndingKind == rowEndingKind)
                            Assert.Equal(1, rowEndingKindCount.count);
                        else
                            Assert.Equal(0, rowEndingKindCount.count);
                    }
                    // OnlyRowEndingKind
                    Assert.Equal(rowEndingKind, partitionContainer.OnlyRowEndingKind);
                }
            }
        }
    }

    [Fact]
    public override void Add_Causes_Expansion()
    {
        // Setup
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();

        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .AddRange(richCharacterList)
            .Add(new RichCharacter { Value = '\n' });

        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadata[0];
                Assert.Empty(partitionMetadata.RowEndingList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadata[1];
                Assert.Empty(partitionMetadata.RowEndingList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadata[2];
                Assert.Equal(2, partitionMetadata.RowEndingList.Count);
            }
        }
    }

    [Fact]
    public override void Add_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\n', 4).Select(x => new RichCharacter { Value = x }).ToArray();

        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .AddRange(richCharacterList);

        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadata[0];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadata[1];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadata[2];
                Assert.Equal(2, partitionMetadata.RowEndingList.Count);
            }
        }
    }

    [Fact]
    public override void Insert()
    {
        // Goal A: Use a for loop to insert a row ending to the: start, middle and, end of the 'otherCharacters' string.
        // Goal B: Use an inner for loop to iterate over all possible row ending characters one can type.
        // Goal C: Assert that the row endings were properly tracked.
        //
        // ab             // ab             // ab
        // ^              //  ^             //   ^
        // Insert at 0    // Insert at 1    // Insert at 2

        var otherCharacters = "ab";

        for (int i = 0; i < otherCharacters.Length; i++)
        {
            foreach (var rowEndingKind in RowEndingKind.Unset.GetRowEndingsUserAllowedToUse())
            {
                var rowEndingCharacters = rowEndingKind.AsCharacters();

                var partitionContainer = new PartitionContainer(5_000)
                    .ToPartitionContainerModifier()
                    .AddRange(otherCharacters.Select(x => new RichCharacter { Value = x }))
                    .InsertRange(i, rowEndingCharacters.Select(x => new RichCharacter { Value = x }));

                // PartitionMetadata
                {
                    var partitionMetadata = partitionContainer.PartitionMetadata.Single();
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
                    // RowEndingList
                    {
                        var insertedRowEnding = partitionContainer.RowEndingList[0];
                        Assert.Equal(i, insertedRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(i + rowEndingCharacters.Length, insertedRowEnding.EndPositionIndexExclusive);

                        var endOfFileRowEnding = partitionContainer.RowEndingList[1];
                        var endOfFileRowEndingRelativePositionIndex = rowEndingCharacters.Length + otherCharacters.Length;
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.EndPositionIndexExclusive);
                    }
                    // RowEndingKindCountList
                    foreach (var rowEndingKindCount in partitionContainer.RowEndingKindCountList)
                    {
                        if (rowEndingKindCount.rowEndingKind == rowEndingKind)
                            Assert.Equal(1, rowEndingKindCount.count);
                        else
                            Assert.Equal(0, rowEndingKindCount.count);
                    }
                    // OnlyRowEndingKind
                    Assert.Equal(rowEndingKind, partitionContainer.OnlyRowEndingKind);
                }
            }
        }
    }

    [Fact]
    public override void Insert_Causes_Expansion()
    {
        // Setup
        var richCharacterList = "ab\n".Select(x => new RichCharacter { Value = x }).ToArray();

        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .InsertRange(0, richCharacterList)
            .Insert(0, new RichCharacter { Value = '\n' });

        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadata[0];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadata[1];
                Assert.Empty(partitionMetadata.RowEndingList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadata[2];
                Assert.Single(partitionMetadata.RowEndingList);
            }
        }
    }

    [Fact]
    public override void Insert_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\n', 3).Select(x => new RichCharacter { Value = x }).ToArray();

        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .InsertRange(0, richCharacterList)
            .Insert(0, new RichCharacter { Value = '\n' });

        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadata[0];
                Assert.Equal(2, partitionMetadata.RowEndingList.Count);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadata[1];
                Assert.Single(partitionMetadata.RowEndingList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadata[2];
                Assert.Single(partitionMetadata.RowEndingList);
            }
        }
    }

    [Fact]
    public override void Remove()
    {
        // Goal A: Use a for loop to remove a row ending at the: start, middle and, end of the 'otherCharacters' string.
        // Goal B: Use an inner for loop to iterate over all possible row ending characters one can type.
        // Goal C: Assert that the row endings were properly tracked.
        //
        // ab             // ab             // ab
        // ^              //  ^             //   ^
        // Remove at 0    // Remove at 1    // Remove at 2

        var otherCharacters = "ab";

        for (int i = 0; i < otherCharacters.Length; i++)
        {
            foreach (var rowEndingKind in RowEndingKind.Unset.GetRowEndingsUserAllowedToUse())
            {
                var rowEndingCharacters = rowEndingKind.AsCharacters();
                var richCharacterList = otherCharacters.Insert(i, rowEndingCharacters).Select(x => new RichCharacter { Value = x });

                var partitionContainer = new PartitionContainer(5_000)
                    .ToPartitionContainerModifier()
                    .AddRange(richCharacterList);

                // Assert that the row ending was added
                Assert.Contains(partitionContainer.PartitionMetadata.Single().RowEndingList, x => x.RowEndingKind == rowEndingKind);

                // Remove the row ending
                for (int j = 0; j < rowEndingCharacters.Length; j++)
                    partitionContainer = partitionContainer.RemoveAt(i);

                // Assert results
                //
                // PartitionMetadata
                {
                    var partitionMetadata = partitionContainer.PartitionMetadata.Single();
                    // RowEndingList
                    {
                        // Assert the row ending was removed.
                        Assert.DoesNotContain(partitionMetadata.RowEndingList, x => x.RowEndingKind == rowEndingKind);
                        // Assert the "EndOFFile" row ending was not erroneously-removed.
                        var endOfFileRowEnding = partitionMetadata.RowEndingList.Single();
                        var endOfFileRowEndingRelativePositionIndex = otherCharacters.Length;
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.EndPositionIndexExclusive);
                    }
                    // RowEndingKindCountList
                    foreach (var rowEndingKindCount in partitionMetadata.RowEndingKindCountList)
                    {
                        Assert.Equal(0, rowEndingKindCount.count);
                    }
                    // OnlyRowEndingKind
                    Assert.Null(partitionMetadata.OnlyRowEndingKind);
                }
                // GlobalMetadata
                {
                    // RowEndingList
                    {
                        // Assert the row ending was removed.
                        Assert.DoesNotContain(partitionContainer.RowEndingList, x => x.RowEndingKind == rowEndingKind);
                        // Assert the "EndOFFile" row ending was not erroneously-removed.
                        var endOfFileRowEnding = partitionContainer.RowEndingList.Single();
                        var endOfFileRowEndingRelativePositionIndex = otherCharacters.Length;
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.StartPositionIndexInclusive);
                        Assert.Equal(endOfFileRowEndingRelativePositionIndex, endOfFileRowEnding.EndPositionIndexExclusive);
                    }
                    // RowEndingKindCountList
                    foreach (var rowEndingKindCount in partitionContainer.RowEndingKindCountList)
                    {
                        Assert.Equal(0, rowEndingKindCount.count);
                    }
                    // OnlyRowEndingKind
                    Assert.Null(partitionContainer.OnlyRowEndingKind);
                }
            }
        }
    }

    [Fact]
    public override void Remove_Causes_Empty_Partition()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .InsertRange(0, "ab\n\n".Select(x => new RichCharacter { Value = x }))
            .RemoveAt(3);
        
        // Assert
        throw new NotImplementedException();
    }

    [Fact]
    public override void Remove_Four_InARow()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .InsertRange(0, new string('\n', 4).Select(x => new RichCharacter { Value = x }))
            .RemoveRange(0, 4);

        // Assert
        throw new NotImplementedException();
    }
}
