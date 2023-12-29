using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
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

    /// <inheritdoc cref="ITextEditorModel.ContentBag"/>
    public ImmutableList<RichCharacter> ContentBag = ImmutableList<RichCharacter>.Empty;
	public ImmutableList<EditBlock> EditBlocksBag { get; init; } = ImmutableList<EditBlock>.Empty;
    /// <inheritdoc cref="ITextEditorModel.RowEndingPositionsBag"/>
	public ImmutableList<(int positionIndex, RowEndingKind rowEndingKind)> RowEndingPositionsBag { get; init; } = ImmutableList<(int positionIndex, RowEndingKind rowEndingKind)>.Empty;
	public ImmutableList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountsBag { get; init; } = ImmutableList<(RowEndingKind rowEndingKind, int count)>.Empty;
	public ImmutableList<TextEditorPresentationModel> PresentationModelsBag { get; init; } = ImmutableList<TextEditorPresentationModel>.Empty;
    /// <inheritdoc cref="ITextEditorModel.TabKeyPositionsBag"/>
	public ImmutableList<int> TabKeyPositionsBag = ImmutableList<int>.Empty;
    /// <inheritdoc cref="ITextEditorModel.OnlyRowEndingKind"/>
    public RowEndingKind? OnlyRowEndingKind { get; init; }
    public RowEndingKind UsingRowEndingKind { get; init; }
    /// <inheritdoc cref="ITextEditorModel.ResourceUri"/>
    public ResourceUri ResourceUri { get; init; }
    public DateTime ResourceLastWriteTime { get; init; }
    /// <inheritdoc cref="ITextEditorModel.FileExtension"/>
    public string FileExtension { get; init; }
    public IDecorationMapper DecorationMapper { get; init; }
    public ICompilerService CompilerService { get; init; }
    public TextEditorSaveFileHelper TextEditorSaveFileHelper { get; init; } = new();
    public int EditBlockIndex { get; init; }
	public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple { get; init; }
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();
}