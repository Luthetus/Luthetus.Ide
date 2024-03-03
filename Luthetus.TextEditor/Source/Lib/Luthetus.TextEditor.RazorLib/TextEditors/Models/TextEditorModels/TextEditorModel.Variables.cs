using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
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

    // (2024-02-29) Plan to add text editor partitioning #Step 100:
    // --------------------------------------------------
    // Change 'contentList' from 'List<RichCharacter>?' to 'List<List<RichCharacter>>?
    /// <inheritdoc cref="ITextEditorModel.ContentList"/>
    public IReadOnlyList<RichCharacter> ContentList { get; } = ImmutableList<RichCharacter>.Empty;
    public ImmutableList<ImmutableList<RichCharacter>> PartitionList { get; } = new ImmutableList<RichCharacter>[] { ImmutableList<RichCharacter>.Empty }.ToImmutableList();
    public ImmutableList<EditBlock> EditBlocksList { get; init; } = ImmutableList<EditBlock>.Empty;

    /// <inheritdoc cref="ITextEditorModel.RowEndingPositionsList"/>
	public ImmutableList<RowEnding> RowEndingPositionsList { get; init; } = ImmutableList<RowEnding>.Empty;
	public ImmutableList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountsList { get; init; } = ImmutableList<(RowEndingKind rowEndingKind, int count)>.Empty;
	public ImmutableList<TextEditorPresentationModel> PresentationModelsList { get; init; } = ImmutableList<TextEditorPresentationModel>.Empty;
    
    /// <inheritdoc cref="ITextEditorModel.TabKeyPositionsList"/>
	public ImmutableList<int> TabKeyPositionsList = ImmutableList<int>.Empty;
    
    /// <inheritdoc cref="ITextEditorModel.OnlyRowEndingKind"/>
    public RowEndingKind? OnlyRowEndingKind { get; init; }
    public RowEndingKind UsingRowEndingKind { get; init; }
    
    /// <inheritdoc cref="ITextEditorModel.ResourceUri"/>
    public ResourceUri ResourceUri { get; init; }
    public DateTime ResourceLastWriteTime { get; init; }
    public int PartitionSize { get; init; }
    
    /// <inheritdoc cref="ITextEditorModel.FileExtension"/>
    public string FileExtension { get; init; }
    public IDecorationMapper DecorationMapper { get; init; }
    public ILuthCompilerService CompilerService { get; init; }
    public TextEditorSaveFileHelper TextEditorSaveFileHelper { get; init; } = new();
    public int EditBlockIndex { get; init; }
    public bool IsDirty { get; init; }
	public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple { get; init; }
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();
}