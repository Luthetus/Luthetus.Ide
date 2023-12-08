using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorModel
{
    public const int TAB_WIDTH = 4;
    public const int GUTTER_PADDING_LEFT_IN_PIXELS = 5;
    public const int GUTTER_PADDING_RIGHT_IN_PIXELS = 15;
    public const int MAXIMUM_EDIT_BLOCKS = 10;
    public const int MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN = 5;
    
    /// <summary>
    /// TODO: Divide the content into partitions for optimization.
    /// </summary>
    public ImmutableList<RichCharacter> ContentBag = ImmutableList<RichCharacter>.Empty;
	public ImmutableList<EditBlock> EditBlocksBag { get; init; } = ImmutableList<EditBlock>.Empty;
    /// <summary>
    /// To get the ending position of RowIndex _rowEndingPositions[RowIndex]<br /><br />
    /// _rowEndingPositions returns the start of the NEXT row
    /// </summary>
	public ImmutableList<(int positionIndex, RowEndingKind rowEndingKind)> RowEndingPositionsBag { get; init; } = ImmutableList<(int positionIndex, RowEndingKind rowEndingKind)>.Empty;
	public ImmutableList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountsBag { get; init; } = ImmutableList<(RowEndingKind rowEndingKind, int count)>.Empty;
	public ImmutableList<TextEditorPresentationModel> PresentationModelsBag { get; init; } = ImmutableList<TextEditorPresentationModel>.Empty;
    /// <summary>
    /// Provides exact position index of a tab character
    /// </summary>
	public ImmutableList<int> TabKeyPositionsBag = ImmutableList<int>.Empty;
	/// <summary>
	/// If there is a mixture of<br/>-Carriage Return<br/>-Linefeed<br/>-CRLF<br/>
	/// Then this will be null.<br/><br/>
	/// If there are no line endings then this will be null.
	/// </summary>
	public RowEndingKind? OnlyRowEndingKind { get; init; }
    public RowEndingKind UsingRowEndingKind { get; init; }
    /// <summary>
    /// TODO: On (2023-10-02) Key&lt;TextEditorModel&gt; was removed, because it felt redundant...
    /// ...given only 1 <see cref="TextEditorModel"/> can exist for a given <see cref="ResourceUri"/>.
    /// This change however creates an issue regarding 'fake' resource uri's that are used for in-memory
    /// files. For example, <see cref="Options.Displays.TextEditorSettingsPreview"/> now has the resource
    /// URI of "__LUTHETUS_SETTINGS_PREVIEW__". This is an issue because could a user have on
    /// their filesystem the file "__LUTHETUS_SETTINGS_PREVIEW__"? (at that exact resource uri)
    /// </summary>
    public ResourceUri ResourceUri { get; init; }
    public DateTime ResourceLastWriteTime { get; init; }
	/// <summary>
	/// This is displayed within the<see cref="Displays.Internals.TextEditorFooter"/>.
	/// </summary>
	public string FileExtension { get; init; }
    public IDecorationMapper DecorationMapper { get; init; }
    public ICompilerService CompilerService { get; init; }
    public TextEditorSaveFileHelper TextEditorSaveFileHelper { get; init; } = new();
    public int EditBlockIndex { get; init; }
	public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple { get; init; }
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();
    public Keymap TextEditorKeymap { get; init; }
	public TextEditorOptions? TextEditorOptions { get; init; }

	public int RowCount => RowEndingPositionsBag.Count;
	public int DocumentLength => ContentBag.Count;
}