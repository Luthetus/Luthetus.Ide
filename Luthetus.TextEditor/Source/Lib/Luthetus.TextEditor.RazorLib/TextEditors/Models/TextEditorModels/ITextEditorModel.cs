using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Partitions.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

/// <summary>
/// Stores the <see cref="RichCharacter"/> class instances that represent the text.<br/><br/>
/// Each TextEditorModel has a unique underlying resource uri.<br/><br/>
/// Therefore, if one has a text file named "myHomework.txt", then only one TextEditorModel
/// can exist with the resource uri of "myHomework.txt".
/// <br/><br/>
/// <see cref="TextEditorModel"/> uses <see cref="ResourceUri"/> as its unique identifier.
/// Throughout this library, one finds <see cref="Key{T}"/> to be a unique identifier.
/// However, since <see cref="ResourceUri"/> should be unique,
/// <see cref="TextEditorModel"/> is an exception to this pattern.
/// </summary>
public interface ITextEditorModel
{
    public PartitionedImmutableList<RichCharacter> ContentList { get; }
	public IList<EditBlock> EditBlocksList { get; }
    /// <summary>
    /// To get the ending position of RowIndex _rowEndingPositions[RowIndex]<br /><br />
    /// _rowEndingPositions returns the start of the NEXT row
    /// </summary>
    public IList<RowEnding> RowEndingPositionsList { get; }
	public IList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountsList { get; }
	public IList<TextEditorPresentationModel> PresentationModelsList { get; }
    /// <summary>
    /// Provides exact position index of a tab character
    /// </summary>
    public IList<int> TabKeyPositionsList { get; }
    /// <summary>
	/// If there is a mixture of<br/>-Carriage Return<br/>-Linefeed<br/>-CRLF<br/>
	/// Then this will be null.<br/><br/>
	/// If there are no line endings then this will be null.
	/// </summary>
	public RowEndingKind? OnlyRowEndingKind { get; }
	public RowEndingKind UsingRowEndingKind { get; }
    /// <summary>
    /// TODO: On (2023-10-02) Key&lt;TextEditorModel&gt; was removed, because it felt redundant...
    /// ...given only 1 <see cref="TextEditorModel"/> can exist for a given <see cref="ResourceUri"/>.
    /// This change however creates an issue regarding 'fake' resource uri's that are used for in-memory
    /// files. For example, <see cref="Options.Displays.TextEditorSettingsPreview"/> now has the resource
    /// URI of "__LUTHETUS_SETTINGS_PREVIEW__". This is an issue because could a user have on
    /// their filesystem the file "__LUTHETUS_SETTINGS_PREVIEW__"? (at that exact resource uri)
    /// </summary>
    public ResourceUri ResourceUri { get; }
	public DateTime ResourceLastWriteTime { get; }
    /// <summary>
	/// This is displayed within the<see cref="Displays.Internals.TextEditorFooter"/>.
	/// </summary>
	public string FileExtension { get; }
	public IDecorationMapper DecorationMapper { get; }
	public ILuthCompilerService CompilerService { get; }
	public TextEditorSaveFileHelper TextEditorSaveFileHelper { get; }
	public int EditBlockIndex { get; }
	public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple { get; }
	public Key<RenderState> RenderStateKey { get; }

    public int RowCount { get; }
    public int DocumentLength { get; }
}
