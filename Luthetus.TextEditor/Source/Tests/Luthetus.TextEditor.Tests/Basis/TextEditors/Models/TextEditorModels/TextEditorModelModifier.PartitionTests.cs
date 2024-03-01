using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorModels;

/// <summary>
/// <see cref="TextEditorModelModifier"/>
/// </summary>
public partial class TextEditorModelModifierTests
{
    /// <summary>
    /// <see cref="TextEditorModelModifier.PartitionList_Add(RichCharacter)"/>
    /// </summary>
    [Fact]
    public void PartitionList_Add()
    {
        // (2024-02-29) Plan to add text editor partitioning #Step 1,000:
        // --------------------------------------------------
        // Here I have a unit test for the un-implemented 'PartitionList_Add(...)'
        // method.
        //
        // When I invoke 'PartitionList_Add()', are there a few distinct cases I
        // can list out prior to writing any code?
        //
        // Well, if the PartitionList is empty, how would 'PartitionList_Add()' handle this?
        // Furthermore, should PartitionList ever be empty? Perhaps it is initialized
        // to contain a partition.
        //
        // I'm going to initialize 'PartitionList' with a 'partition',
        // and aim to ensure there is always at least one 'partition' in the 'PartitionList'.
        //
        // I used: 'new ImmutableList<RichCharacter>[] { ImmutableList<RichCharacter>.Empty }.ToImmutableList();'
        // to initialize the 'PartitionList' with a 'partition'.
        //
        // Well, if the only existing partition is full, how would 'PartitionList_Add()' handle this?
        //
        // We can most easily, just create another partition. I suppose that is the best
        // route to initially take.
        //
        // So, how do we create another partition? Well, it is part of the code that was used to
        // initialize the 'PartitionList' with a 'partition'.
        //
        // Specifically, 'ImmutableList<RichCharacter>.Empty', will get us an empty partition.
        // Then we can pass this empty partition as a parameter to the 'Add(...)' method
        // of 'PartitionList'.
        //
        // Since 'PartitionList' is an immutable type, we must re-assign 'PartitionList' to the
        // output of 'Add(...)'.
        //
        // PartitionList = PartitionList.Add(ImmutableList<RichCharacter>.Empty);
        //
        // Well, if there were space in the existing partition, how would 'PartitionList_Add()' handle this?
        //
        // If we store the existing partition's index in a variable named, 'indexOfPartitionWithAvailableSpace'.
        // Then we can access that partition with the index operator,
        // 'var partition = PartitionList[indexOfPartitionWithAvailableSpace]'.
        //
        // But, the partition is immutable, so just invoking '.Add(richCharacter)' on the 'partition'
        // would not suffice.
        // 
        // The 'partition' needs to store the output of invoking '.Add(richCharacter)' at its own position within 'PartitionList'.
        //
        // So, it appears there could be 2 cases that need handled in the 'PartitionList_Add(...)' method:
        //     -Not enough space, so add a new partition.
        //     -A partition was found which has available space, so add the 'richCharacter' to it.
        //
        // We can write out the cases such that, if there isn't enough space, we 'secretly'
        // add a new partition. Then we fall into the second case, 'a partition was found which has space available'.
        // The second case doesn't need to know we had just added that partition.
        //
        // (2024-02-29) Plan to add text editor partitioning #Step 1,300:
        // --------------------------------------------------
        // I want to write some unit tests.
        //
        // What are the situation that can occur when invoking the 'PartitionList_Add' method?
        //     -Not enough space, so add a new partition.
        //         -PartitionList_Add_SHOULD_CREATE_MORE_SPACE_IF_NEEDED();
        //     -A partition was found which has available space, so add the 'richCharacter' to it.
        //         --PartitionList_Add_SHOULD_INSERT_INTO_PARTITION_WITH_AVAILABLE_SPACE();
        //
        // I want to add another test, because it should be asserted that the 'PartitionList_Add' method
        // correctly identifies the partition at which the 'richCharacter' should be inserted.
        //
        // Another way of saying this is: the 'PartitionList_Add' needs to map a 'globalPositionIndex'
        // to a 'partition'. And furthermore, calculate the 'relativePositionIndex' where relativity is
        // to the 'partition'.
        //
        // Example (pseudo code):
        // --------
        //
        // PartitionList.SetPartitionSize(5);
        // # This results in the following PartitionList:
        // # {
        // #     [] // This partition is initially empty.
        // # }
        //
        // PartitionList_AddRange("Hello World!");
        // # The string "Hello World!" is 12 characters long.
        // # So, we can break down the 'PartitionList_AddRange(...)' invocation to
        //     # 12 invocations of 'PartitionList_Add(...)', one for each character in the string.
        // #
        // # First 'PartitionList_Add(...)' invocation results in the following PartitionList:
        // # {
        // #     [ 'H' ]
        // # }
        // # Once the pattern is well established I'll skip ahead to some unique invocations of 'PartitionList_Add(...)'.
        // #
        // # Second 'PartitionList_Add(...)' invocation results in the following PartitionList:
        // # {
        // #     [ 'H', 'e' ]
        // # }
        // #
        // # Third 'PartitionList_Add(...)' invocation results in the following PartitionList:
        // # {
        // #     [ 'H', 'e', 'l' ]
        // # }
        // # With 3 invocations having been performed, I believe the pattern is well established.
        // # we are just adding each character one after another into the first partition.
        // #
        // # Fifth 'PartitionList_Add(...)' invocation and something unique happens:
        // # UPON-invoking 'PartitionList_Add(...)' the PartitionList looks as follows:
        // # {
        // #     [ 'H', 'e', 'l', 'l', 'o' ]
        // # }
        // #
        // # The issue arises however, that the partition size is '5'.
        // # So, no more space is available in the first partition.
        // # The fifth invocation of needs to create a new partition and insert it after the first.
        // # This results in the following PartitionList:
        // # {
        // #     [ 'H', 'e', 'l', 'l', 'o' ],
        // #     [ ] // Initially this new partition is empty.
        // # }
        // #
        // # Now that space was added to store more characters, the fifth invocation can return to the pattern
        // # that was seen in the first, second, and third invocations.
        // # Fifth 'PartitionList_Add(...)' invocation results in the following PartitionList:
        // # {
        // #     [ 'H', 'e', 'l', 'l', 'o' ],
        // #     [ ' ' ]
        // # }
        // #
        // # The previous example glosses over some details though.
        // # We can use the Sixth invocation of 'PartitionList_Add(...)' to illustrate this.
        // # The Sixth invocation needs to add its character to the second partition.
        // # How would 'PartitionList_Add(...)' determine which partition receives the character?
        // # In the case of 'Add' methods, one expects the entry to be added at the end of the list.
        // # So this results in a degree of simplification.
        // #
        // # But, given the output from the fifth invocation how would one insert at index 5, a comma character?
        // # Well, the index '5' divided by the partition size of '5' is '1'.
        // # So, we can put the text in the partition with index of '1'.
        // # Furthermore, the index '5' modulo the partition size '5' is 0. So,
        // # the relative index within the second partition at which to insert the comma is '0'.
        // #
        // # We made a presumption though. The presumption is that all partitions will be filled except for the
        // # last partition in PartitionList.
        // #
        // # If out presumption is true, then the partitions wouldn't be quite as useful.
        // # By having each partition, NOT, filled. Then one can write to a partition,
        // # and only need to re-calculate metadata for that one partition which was changed.
        // #
        // # Constantly, creating new partitions, or any other inter-partition calculation,
        // # is costly. Avoiding inter-partition calculations is the goal.
        // #
        // # I'll write out the result of having added the comma character.
        // # Insertiion of the comma character results in the following PartitionList:
        // # {
        // #     [ 'H', 'e', 'l', 'l', 'o' ],
        // #     [ ',', ' ' ]
        // # }
        // # 
        // # If we insert at index '4' the letter 'z', what occurs?
        // # 
        // # Well, the index '4' resides on the first partition.
        // # And, the first partition is full.
        // # So, should we once again, create a new partition?
        // # Perhaps, we could move the letter 'o' to the first index of the second partition.
        // # If we were to do this it results in the following PartitionList:
        // # {
        // #     [ 'H', 'e', 'l', 'l' ],
        // #     [ 'o', ',', ' ' ]
        // # }
        // # 
        // # Now we have space on the first partition to add the 'z' at index 4.
        // # If we were to do this it results in the following PartitionList:
        // # {
        // #     [ 'H', 'e', 'l', 'l', 'z' ],
        // #     [ 'o', ',', ' ' ]
        // # }
        // # 
        // # While this algorithm worked, we can consider the worst case scenaro.
        // # What happens when a user repeatedly type into the first partition?
        // # We have an algorithm where every typed character results in
        // # an inter-partition calculation.
        // # 
        // # Where the inter-partion calculation is to move
        // # a character from one partition to another.
        // #
        // # We should go back in time, to the fifth invocation of 'PartitionList_Add(...)'.
        // # It was the invocation where we added the second partition. 
        // #
        // # Once again we are here, UPON-invoking 'PartitionList_Add(...)' the PartitionList looks as follows:
        // # {
        // #     [ 'H', 'e', 'l', 'l', 'o' ]
        // # }
        // # 
        // # But, how can we improve the efficiency of the partitions?
        // # That is to say, how can we decrease inter-partition calculations.
        // #
        // # My thought is to still add a new partition like we did.
        // # But, one step further, is to at the same time, split the
        // # contents of partition one amongst itself, and the new partition.
        // # If we were to do this the PartitionList looks as follows:
        // # {
        // #     [ 'H', 'e', 'l' ],
        // #     [ 'l', 'o' ]
        // # }
        // #
        // # An even split could not be made here, since the partition size is 5 and won't divide by 2.
        // # But, with the split we have, we see that now there are 2 characters which can be typed into
        // # the first partition, without resulting in an inter-partition calculation.
        // # And, for the second partition there are 3 characters available.
        // #
        // # Contrast this with the first way the new partition was handled.
        // # In that scenario, every single time a user typed into the first partition,
        // # and inter-partition calculation needed to be performed.
        // #
        // # Furthermore, a partition size of '5' is not a realistic partition size.
        // # This size is only chosen to aid in illustration.
        // # I imagine a realtistic partition size to be 5,000 (although I'm not quite sure what it would be myself).
        // 
        // Moving on past the example, let's revisit the scenarios that we want to make unit tests for:
        //     -Not enough space, so add a new partition.
        //         -PartitionList_Add_SHOULD_CREATE_MORE_SPACE_IF_NEEDED();
        //     -A partition was found which has available space, so add the 'richCharacter' to it.
        //         --PartitionList_Add_SHOULD_INSERT_INTO_PARTITION_WITH_AVAILABLE_SPACE();
        //
        // I often like to start with a naive, and simple solution (provided that the naive implementation
        // provides a malleable foundation on which the more complex solution can be built).
        //
        // So I'm going to do exactly that:
    }

    [Fact]
    public void PartitionList_Add_SHOULD_INSERT_INTO_PARTITION_WITH_AVAILABLE_SPACE()
    {
        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;

        var model = new TextEditorModel(
             resourceUri,
             resourceLastWriteTime,
             fileExtension,
             string.Empty,
             null,
             null,
             partitionSize: 5);

        var modifier = new TextEditorModelModifier(model);

        // Assert that the first partition is empty at the start.
        Assert.Empty(modifier.PartitionList.First());

        // Assert that more space than just one partition will be needed.
        var sourceText = "Hello World!";
        Assert.True(sourceText.Length > model.PartitionSize);

        var firstPartitionStringValue = new string(modifier.PartitionList.First().Select(x => x.Value).ToArray());

        for (int i = 0; i < sourceText.Length; i++)
        {
            var firstPartition = modifier.PartitionList.First();

            var richCharacter = new RichCharacter { Value = sourceText[i] };
            modifier.PartitionList_Add(richCharacter);

            if (i < model.PartitionSize)
            {
                // Assert that the first n loops write to the first partition, because it has available space
                // This is asserted by checking that the string value of the first partition has changed.
                var newStringValue = new string(modifier.PartitionList.First().Select(x => x.Value).ToArray());
                Assert.NotEqual(firstPartitionStringValue, newStringValue);
                firstPartitionStringValue = newStringValue; 
            }
            else
            {
                // Assert that the last (n + 1) loops do NOT write to the first partition, because it no longer has available space
                // This is asserted by checking that the string value of the first partition has NOT changed.
                var newStringValue = new string(modifier.PartitionList.First().Select(x => x.Value).ToArray());
                Assert.Equal(firstPartitionStringValue, newStringValue);
            }
        }

        // Assert that the output is correct.
        Assert.Equal(
            new string(modifier.ContentList.Select(x => x.Value).ToArray()),
            sourceText);
    }

    [Fact]
    public void PartitionList_Add_SHOULD_CREATE_MORE_SPACE_IF_NEEDED_V2()
    {
        // (2024-02-29) Plan to add text editor partitioning #Step 1,400:
        // --------------------------------------------------
        // Now that I've written the following tests:
        //     -Not enough space, so add a new partition.
        //         -PartitionList_Add_SHOULD_CREATE_MORE_SPACE_IF_NEEDED();
        //     -A partition was found which has available space, so add the 'richCharacter' to it.
        //         --PartitionList_Add_SHOULD_INSERT_INTO_PARTITION_WITH_AVAILABLE_SPACE();
        //
        // I want to change the logic for adding a new partition when more space is needed.
        // I want the partition which is full, to split its content amongst itself, and the newly created partition.
        // This way, inter-partition calculations can be minimized, upon successive insertions to an initially-full partition.
        // 
        // I previously have used an example of invoking 'PartitionList_AddRange(...)',
        // in which I presume AddRange to be a loop that invokes 'PartitionList_Add(...)',
        // for each character.
        //
        // PartitionList.SetPartitionSize(5);
        // PartitionList_AddRange("Hello World!");
        //
        // # If we skip to the fifth invocation of 'PartitionList_Add(...)'
        // # Then, the PartitionList looks as follows:
        // # {
        // #     [ 'H', 'e', 'l', 'l', 'o' ]
        // # }
        // # 
        // # I want to change the 'create more space' logic, to result in a PartitionList that looks as follows:
        // # {
        // #     [ 'H', 'e', 'l' ],
        // #     [ 'l', 'o' ]
        // # }
        // 
        // I modified the original 'PartitionList_Add_SHOULD_CREATE_MORE_SPACE_IF_NEEDED'
        // unit test to include the 'Obsolete' attribute like so:
        // [Obsolete($"See: {nameof(PartitionList_Add_SHOULD_CREATE_MORE_SPACE_IF_NEEDED_V2)}"), Fact]
        //
        // As well, I changed the original test's method name to have the prefix "OBSOLETE_"
        //
        // Once V2 is finished, I'll go back and delete the original.

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;

        var model = new TextEditorModel(
             resourceUri,
             resourceLastWriteTime,
             fileExtension,
             string.Empty,
             null,
             null,
             partitionSize: 5);

        var modifier = new TextEditorModelModifier(model);

        // Assert that only one partition exists at the start.
        Assert.Single(modifier.PartitionList);

        // Assert that more space will be needed.
        var sourceText = "Hello World!";
        Assert.True(sourceText.Length > model.PartitionSize);

        for (int i = 0; i < sourceText.Length; i++)
        {
            if (i == model.PartitionSize)
            {
                // Assert that up until this loop iteration only 1 partition has existed.
                Assert.Single(modifier.PartitionList);
            }

            var richCharacter = new RichCharacter { Value = sourceText[i] };
            modifier.PartitionList_Add(richCharacter);

            if (i == model.PartitionSize)
            {
                // Assert that this loop iteration caused another partition to be made
                Assert.Equal(2, modifier.PartitionList.Count);

                // Furthermore, assert that the first partition contains
                // (PARTITION_SIZE / 2 + (PARTITION_SIZE % 2)) entries, and the second partition contains
                // (PARTITION_SIZE / 2)
                Assert.Equal(
                    model.PartitionSize / 2 + (model.PartitionSize % 2),
                    modifier.PartitionList.First().Count);

                Assert.Equal(
                    // This had to be changed to include a '+1' because the insertion already occurred.
                    model.PartitionSize / 2 + 1,
                    modifier.PartitionList.Last().Count);
            }
        }

        // (2024-02-29) Plan to add text editor partitioning #Step 1,400:
        // --------------------------------------------------
        // In the end, we have a PartitionList as follows:
        //
        // PartitionList.SetPartitionSize(5);
        // # {
        // #     [ 'H', 'e', 'l' ]
        // #     [ 'l', 'o', ' ' ]
        // #     [ 'W', 'o', 'r' ]
        // #     [ 'l', 'd', '!' ]
        // # }
        Assert.Equal(
            "Hel",
            new string(modifier.PartitionList[0].Select(x => x.Value).ToArray()));

        Assert.Equal(
            "lo ",
            new string(modifier.PartitionList[1].Select(x => x.Value).ToArray()));

        Assert.Equal(
            "Wor",
            new string(modifier.PartitionList[2].Select(x => x.Value).ToArray()));

        Assert.Equal(
            "ld!",
            new string(modifier.PartitionList[3].Select(x => x.Value).ToArray()));

        // Assert that the output is correct.
        Assert.Equal(
            new string(modifier.ContentList.Select(x => x.Value).ToArray()),
            sourceText);
    }

    [Fact]
    public void Aaa()
    {
        // (2024-02-29) Plan to add text editor partitioning #Step 1,600:
        // --------------------------------------------------
        // I ran the Photino.Ide host to see how things were.
        //
        // It seems there is a 'backspace' bug when crossing the boundaries of one
        // partition into the other. But I'm not sure...
        //
        // I presume 'delete' would have the same problem, but in the opposite direction.
        //
        // A better wording might be to say, "If one has their cursor in the second partion,
        // at the 0th index of that partition. Then they hit 'backspace'. I believe nothing will
        // occur."
        //
        // So, the user has their cursor in the second partition, but the keyboard event results
        // in removal of a character in the previous partition.
        //
        // Then, it also is presumed that if the user has their cursor at the end of the first partition,
        // and they hit 'delete', that nothing will occur, just the same.
        // 
        // Because they had their cursor in the first partition, but the keyboard event results
        // in removal of a character in the next partition.
        //
        // All in all, I think this boils down to making the following tests:
        //     -BACKSPACE_REMOVES_CHARACTER_FROM_PREVIOUS_PARTITION()
        //     -DELETE_REMOVES_CHARACTER_FROM_NEXT_PARTITION()
        //     -SELECTING_FIRST_CHARACTER_OF_PARTITION_THEN_BACKSPACE_REMOVES_FIRST_CHARACTER()
        //     -SELECTING_LAST_CHARACTER_OF_PARTITION_THEN_DELETE_REMOVES_LAST_CHARACTER()
        //     -SELECTION_THAT_SPANS_MORE_THAN_ONE_PARTITION_IS_REMOVED_PROPERLY_WITH_BACKSPACE()
        //     -SELECTION_THAT_SPANS_MORE_THAN_ONE_PARTITION_IS_REMOVED_PROPERLY_WITH_DELETE()
        // 
    }
}