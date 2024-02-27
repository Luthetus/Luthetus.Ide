using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.Partitions.Models;

public record PartitionMetadata // Presume all meta-data to be relative to the partition itself, NOT to the entirety of the text editor.<br/> For example, if a partition says they have a <see cref="TabList"/> entry at positionIndex '3'. That is the third index of the partition, regardless of however much text came before the start of this positionIndex.<br/> To convert meta-data which is relative to a partition, one must sum the count of text in every preceeding partition, then add the relative value.<br/><br/> By doing this, one can edit the text editor 5,000 characters at a time, even if there were to be 100,000 characters. In this example 5,000 would be the partition size. <br/><br/> TODO: I need to handle the partition meta data differently for the text editor. Perhaps it would be preferable to have a generic <see cref="PartitionMetadata"/> type, but I'm finding a it hard to do so without odd looking and confusing code. So, I'm going to copy and paste the attempt at the generic type here, then just change the source code to work for the text editor. (2024-02-25)
{
    public PartitionMetadata(int count)
    {
        RelativeCharacterCount = count;
    }

    public int RelativeCharacterCount { get; set; }
	public ImmutableList<int> TabList { get; set; } = ImmutableList<int>.Empty; // ITextEditorModel.TabKeyPositionsList
	public ImmutableList<RowEnding> RowEndingList { get; set; } = ImmutableList<RowEnding>.Empty; // ITextEditorModel.RowEndingPositionsList
    public ImmutableList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountList { get; set; } = ImmutableList<(RowEndingKind rowEndingKind, int count)>.Empty; // ITextEditorModel.RowEndingKindCountsList
    public RowEndingKind? OnlyRowEndingKind { get; set; } // ITextEditorModel.OnlyRowEndingKind
}
