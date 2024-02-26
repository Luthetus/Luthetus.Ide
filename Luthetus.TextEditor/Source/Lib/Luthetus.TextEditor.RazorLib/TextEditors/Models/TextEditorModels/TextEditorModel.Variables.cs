using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModel
{
    public const int TAB_WIDTH = 4;
    public const int GUTTER_PADDING_LEFT_IN_PIXELS = 5;
    public const int GUTTER_PADDING_RIGHT_IN_PIXELS = 15;
    public const int MAXIMUM_EDIT_BLOCKS = 10;
    public const int MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN = 5;
    public const int PARTITION_SIZE = 5_000;

    /// <summary>
    /// An 'Empty' pattern cannot be done with <see cref="PartitionContainer"/>
    /// because it currently does not allow for changing partition size.
    /// And since the partition size can be any int, then just one 'Empty' cannot exist.
    /// <br/></br>
    /// That said, the text editor can make its own Empty partition given that it
    /// always will have the <see cref="PARTITION_SIZE"/>
    /// </summary>
    public static readonly PartitionContainer PARTITION_EMPTY = new(PARTITION_SIZE);

    /// <inheritdoc cref="ITextEditorModel.ContentList"/>
    public PartitionContainer ContentList = PARTITION_EMPTY;
	public ImmutableList<EditBlock> EditBlocksList { get; init; } = ImmutableList<EditBlock>.Empty;
    /// <inheritdoc cref="ITextEditorModel.RowEndingPositionsList"/>
	public ImmutableList<RowEnding> RowEndingPositionsList => ContentList.GlobalMetadata.RowEndingList.Value;
    public ImmutableList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountsList { get; init; } = ImmutableList<(RowEndingKind rowEndingKind, int count)>.Empty;
	public ImmutableList<TextEditorPresentationModel> PresentationModelsList { get; init; } = ImmutableList<TextEditorPresentationModel>.Empty;
    /// <inheritdoc cref="ITextEditorModel.TabKeyPositionsList"/>
	public ImmutableList<int> TabKeyPositionsList => ContentList.GlobalMetadata.TabList.Value;
    /// <inheritdoc cref="ITextEditorModel.OnlyRowEndingKind"/>
    public RowEndingKind? OnlyRowEndingKind { get; init; }
    public RowEndingKind UsingRowEndingKind { get; init; }
    /// <inheritdoc cref="ITextEditorModel.ResourceUri"/>
    public ResourceUri ResourceUri { get; init; }
    public DateTime ResourceLastWriteTime { get; init; }
    /// <inheritdoc cref="ITextEditorModel.FileExtension"/>
    public string FileExtension { get; init; }
    public IDecorationMapper DecorationMapper { get; init; }
    public ILuthCompilerService CompilerService { get; init; }
    public TextEditorSaveFileHelper TextEditorSaveFileHelper { get; init; } = new();
    public int EditBlockIndex { get; init; }
	public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple { get; init; }
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();
}