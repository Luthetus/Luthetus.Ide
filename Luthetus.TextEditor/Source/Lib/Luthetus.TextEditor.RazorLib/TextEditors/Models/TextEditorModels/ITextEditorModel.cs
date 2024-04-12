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
    /// <summary>
    /// TODO: This needs to be separated out into an IReadOnlyList&lt;char&gt;...
    ///       ...and a IReadOnlyList&lt;byte&gt;
    ///       |
    ///       This change is needed because, a large part of optimizing the text editor is
    ///       to reduce the operations done on the entirety of the text editor.
    ///       |
    ///       And, currently one invokes textEditorModel.GetAllText() rather frequently, and redundantly.
    ///       |
    ///       This invocation of 'GetAllText()' needs to iterate over every character in the text editor,
    ///       and select its 'Value' property.
    ///       |
    ///       This is an operation done on the entirety of the text editor, and therefore
    ///       it should be optimized.
    ///       |
    ///       The result of the change will make 'GetAllText()' simply return a reference
    ///       to the underlying list of characters.
    /// </summary>
    public ImmutableList<char> CharList { get; }
    public ImmutableList<byte> DecorationByteList { get; }
    public ImmutableList<TextEditorPartition> PartitionList { get; }
	public IList<EditBlock> EditBlockList { get; }
    /// <summary>
    /// To get the ending position of LineIndex _lineEndPositions[LineIndex]<br /><br />
    /// _lineEndPositions returns the start of the NEXT line
    /// </summary>
    public IList<LineEnd> LineEndPositionList { get; }
	public IList<(LineEndKind lineEndingKind, int count)> LineEndingKindCountsList { get; }
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
	public LineEndKind? OnlyLineEndKind { get; }
	public LineEndKind UsingLineEndKind { get; }
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
    public bool IsDirty { get; }
    public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple { get; }
	public Key<RenderState> RenderStateKey { get; }

    public int LineCount { get; }
    public int DocumentLength { get; }
}
