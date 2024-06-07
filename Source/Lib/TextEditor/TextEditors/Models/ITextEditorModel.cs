using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Stores the <see cref="RichCharacter"/> class instances that represent the text.<br/><br/>
/// Each TextEditorModel has a unique underlying resource uri.<br/><br/>
/// Therefore, if one has a text file named "myHomework.txt", then only one TextEditorModel
/// can exist with the resource uri of "myHomework.txt".
/// <br/><br/>
/// In regards to the use of the words: "line", and "row":
/// A "line" is being defined as a contiguous block of characters, up to and including a <see cref="LineEnd"/>.
/// <br/>
/// A "row" is being defined as a UI rendering term. Such that, without a line-wrap,
/// a line of text will be rendered at the same 'y-axis' position, with only a difference
/// in 'x-axis' between each character.
/// With line-wrap, a line can span many "row"(s) on the UI.
/// That is, the same line of text can be rendered as '1 row' up until the 'x-coordinate'
/// goes out of view. At that point the 'y-axis' will be incremented by the height of 1 'line',
/// and the 'x-coordinate' is reset, and more of the line can be rendered.
/// <br/><br/>
/// <see cref="TextEditorModel"/> uses <see cref="ResourceUri"/> as its unique identifier.
/// Throughout this library, one finds <see cref="Key{T}"/> to be a unique identifier.
/// However, since <see cref="ResourceUri"/> should be unique,
/// <see cref="TextEditorModel"/> is an exception to this pattern.
/// </summary>
public interface ITextEditorModel
{
    public ImmutableList<RichCharacter> RichCharacterList { get; }
    public ImmutableList<TextEditorPartition> PartitionList { get; }
    public IList<ITextEditorEdit> EditBlockList { get; }
    /// <summary>
    /// Convert an index for a <see cref="LineEnd"/> to a <see cref="LineInformation"/>, use the method:
    /// <see cref="TextEditorModelExtensionMethods.GetLineInformation(ITextEditorModel, int)"/>
    /// </summary>
    public IList<LineEnd> LineEndList { get; }
    public IList<(LineEndKind lineEndKind, int count)> LineEndKindCountList { get; }
    public IList<TextEditorPresentationModel> PresentationModelList { get; }
    /// <summary>
    /// Provides exact position index of a tab character
    /// </summary>
    public IList<int> TabKeyPositionList { get; }
    /// <summary>
	/// If there is a mixture of<br/>-Carriage Return<br/>-Linefeed<br/>-CRLF<br/>
	/// Then this will be null.<br/><br/>
	/// If there are no line endings then this will be null.
	/// </summary>
	public LineEndKind? OnlyLineEndKind { get; }
    public LineEndKind LineEndKindPreference { get; }
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
    public SaveFileHelper TextEditorSaveFileHelper { get; }
    public int EditBlockIndex { get; }
    public bool IsDirty { get; }
    public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple { get; }
    public Key<RenderState> RenderStateKey { get; }
    public string AllText { get; }

    public int LineCount { get; }
    public int CharCount { get; }
}
