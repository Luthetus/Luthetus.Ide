using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;

namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public class Track_Tab : Track_Tests_Base
{
    [Fact]
    public override void Add()
    {
        // Goal A: Use a for loop to add a tab to the: start, middle and, end of the 'otherCharacters' string.
        // Goal C: Assert that the tabs were properly tracked.
        //
        // ab          // ab          // ab
        // ^           //  ^          //   ^
        // Add at 0    // Add at 1    // Add at 2

        var otherCharacters = "ab";

        for (int i = 0; i < otherCharacters.Length; i++)
        {
            var richCharacterList = otherCharacters.Insert(i, "\t").Select(x => new RichCharacter { Value = x });
            
            var partitionContainer = new PartitionContainer(5_000)
                .ToPartitionContainerModifier()
                .AddRange(richCharacterList);

            // PartitionMetadata
            {
                var partitionMetadata = partitionContainer.PartitionMetadata.Single();
                Assert.Equal(i, partitionMetadata.TabList.Single());
            }
            // GlobalMetadata
            {
                Assert.Equal(i, partitionContainer.TabList.Single());
            }
        }
    }

    [Fact]
    public override void Add_Causes_Expansion()
    {
        // Setup
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();

        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .AddRange(richCharacterList)
            .Add(new RichCharacter { Value = '\t' })
            .ToPartitionContainer();

        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadata[0];
                Assert.Empty(partitionMetadata.TabList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadata[1];
                Assert.Empty(partitionMetadata.TabList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadata[2];
                Assert.Equal(2, partitionMetadata.TabList.Count);
            }
        }
    }

    [Fact]
    public override void Add_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\t', 4).Select(x => new RichCharacter { Value = x }).ToArray();

        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .AddRange(richCharacterList)
            .ToPartitionContainer();

        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadata[0];
                Assert.Single(partitionMetadata.TabList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadata[1];
                Assert.Single(partitionMetadata.TabList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadata[2];
                Assert.Equal(2, partitionMetadata.TabList.Count);
            }
        }
    }

    [Fact]
    public override void Insert()
    {
        // Goal A: Use a for loop to insert a tab to the: start, middle and, end of the 'otherCharacters' string.
        // Goal C: Assert that the tabs were properly tracked.
        //
        // ab             // ab             // ab
        // ^              //  ^             //   ^
        // Insert at 0    // Insert at 1    // Insert at 2

        var otherCharacters = "ab";

        for (int i = 0; i < otherCharacters.Length; i++)
        {
            var partitionContainer = new PartitionContainer(5_000)
                .ToPartitionContainerModifier()
                .AddRange(otherCharacters.Select(x => new RichCharacter { Value = x }))
                .Insert(i, new RichCharacter { Value = '\t' })
                .ToPartitionContainer();

            // PartitionMetadata
            {
                var partitionMetadata = partitionContainer.PartitionMetadata.Single();
                Assert.Equal(i, partitionMetadata.TabList.Single());
            }
            // GlobalMetadata
            {
                Assert.Equal(i, partitionContainer.TabList.Single());
            }
        }
    }

    [Fact]
    public override void Insert_Causes_Expansion()
    {
        // Setup
        var richCharacterList = "ab\t".Select(x => new RichCharacter { Value = x }).ToArray();
        
        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .InsertRange(0, richCharacterList)
            .Insert(0, new RichCharacter { Value = '\t' })
            .ToPartitionContainer();

        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadata[0];
                Assert.Single(partitionMetadata.TabList);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadata[1];
                Assert.Empty(partitionMetadata.TabList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadata[2];
                Assert.Single(partitionMetadata.TabList);
            }
        }
    }

    [Fact]
    public override void Insert_Four_InARow()
    {
        // Setup
        var richCharacterList = new string('\t', 3).Select(x => new RichCharacter { Value = x }).ToArray();

        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .InsertRange(0, richCharacterList)
            .Insert(0, new RichCharacter { Value = '\t' })
            .ToPartitionContainer();

        { // Assert
            { // Partition One
                var partitionMetadata = partitionContainer.PartitionMetadata[0];
                Assert.Equal(2, partitionMetadata.TabList.Count);
            }
            { // Partition Two
                var partitionMetadata = partitionContainer.PartitionMetadata[1];
                Assert.Single(partitionMetadata.TabList);
            }
            { // Partition Three
                var partitionMetadata = partitionContainer.PartitionMetadata[2];
                Assert.Single(partitionMetadata.TabList);
            }
        }
    }

    [Fact]
    public override void Remove()
    {
        // Goal A: Use a for loop to remove a tab at the: start, middle and, end of the 'otherCharacters' string.
        // Goal C: Assert that the tabs were properly tracked.
        //
        // ab             // ab             // ab
        // ^              //  ^             //   ^
        // Remove at 0    // Remove at 1    // Remove at 2

        var otherCharacters = "ab";

        for (int i = 0; i < otherCharacters.Length; i++)
        {
            var richCharacterList = otherCharacters.Insert(i, "\t").Select(x => new RichCharacter { Value = x });

            var partitionContainer = new PartitionContainer(5_000)
                .ToPartitionContainerModifier()
                .AddRange(richCharacterList);

            // Assert that the tab was added
            Assert.Contains(partitionContainer.PartitionMetadata.Single().TabList, x => x == i);

            // Remove the tab
            partitionContainer = partitionContainer.RemoveAt(i);
            
            // Assert results
            //
            // PartitionMetadata
            {
                var partitionMetadata = partitionContainer.PartitionMetadata.Single();
                // Assert the tab was removed.
                Assert.Empty(partitionMetadata.TabList);
            }
            // GlobalMetadata
            {
                // Assert the tab was removed.
                Assert.Empty(partitionContainer.TabList);
            }
        }
    }

    [Fact]
    public override void Remove_Causes_Empty_Partition()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .InsertRange(0, "ab\t\t".Select(x => new RichCharacter { Value = x }));

        // Pre assertion to ensure a before and after measurement
        Assert.Equal(2, partitionContainer.PartitionMetadata[0].TabList.Single()); // Tab in first partition
        Assert.Equal(0, partitionContainer.PartitionMetadata[1].TabList.Single()); // Tab in second partition
        partitionContainer = partitionContainer.RemoveAt(3); // Remove the tab from second partition
        // Assert
        Assert.Empty(partitionContainer.PartitionMetadata[1].TabList); // Second partition does NOT have any tabs.
    }

    [Fact]
    public override void Remove_Four_InARow()
    {
        // Setup
        var partitionContainer = new PartitionContainer(3)
            .ToPartitionContainerModifier()
            .InsertRange(0, new string('\t', 4).Select(x => new RichCharacter { Value = x }))
            .RemoveRange(0, 4);

        // Assert
        throw new NotImplementedException();
    }
}
