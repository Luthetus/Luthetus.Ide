using Luthetus.TextEditor.RazorLib.Characters.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModelModifier
{
    // (2024-02-29) Plan to add text editor partitioning #Step 1,000:
    // --------------------------------------------------
    // The 'PartitionList_...(...)' methods are currently un-implemented.
    //
    // So, I need to decide how to implement these methods.
    //
    // I'm going to make a unit test.
    public void PartitionList_Add(RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            ImmutableList<RichCharacter>? partition = PartitionList[i];

            // (2024-02-29) Plan to add text editor partitioning #Step 1,100:
            // --------------------------------------------------
            // I am encountering an issue here. What is the partition size?
            //
            // I can go define that constant now. I'll make it 5,000.
            // In 'TextEditorModel.Variables.cs' I added: 'public const int PARTITION_SIZE = 5_000;'
            //
            // When comparing partition.Count and TextEditorModel.PARTITION_SIZE should
            // '==' operator be used or ">="?
            //
            // In an ideal world partition.Count will always be constrained by the
            // TextEditorModel.PARTITION_SIZE constant.
            //
            // But, an anxious voice tells me to put ">=" to be safe.
            // I'm not sure the correct answer to this question.
            //
            // I presume the ">=" instead of "==" is a negligible performance penalty, if any.
            // Whereas mentally, for me its a massive anxiety relief, but I don't know.
            if (partition.Count >= TextEditorModel.PARTITION_SIZE)
            {
                // (2024-02-29) Plan to add text editor partitioning #Step 1,100:
                // --------------------------------------------------
                // I'm still writing the 'PartitionList_Add(...)' method,
                // but I think it'd be best to not write the 'PartitionList_Add(...)' out.
                //
                // Instead, I could invoke 'PartitionList_Insert(...)', where the
                // index to insert at would be the global-count of all rich characters.
                //
                // I'm going to do this.
            }
        }
    }
    
    public void PartitionList_Insert(int globalPositionIndex, RichCharacter richCharacter)
    {
        
    }
    
    public void PartitionList_InsertRange(int globalPositionIndex, IEnumerable<RichCharacter> richCharacterList)
    {
        
    }
    
    public void PartitionList_RemoveRange(int index, int count)
    {
        
    }
}