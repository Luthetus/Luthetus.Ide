using Luthetus.TextEditor.RazorLib.Characters.Models;
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

        
    }
}