using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Stores the <see cref="RichCharacter"/> class instances that represent the text.<br/><br/>
/// Each TextEditorModel has a unique underlying resource uri.<br/><br/>
/// Therefore, if one has a text file named "myHomework.txt", then only one TextEditorModel
/// can exist with the resource uri of "myHomework.txt".<br/><br/>
/// 
/// In regards to the use of the words: "line", and "row":
/// A "line" is being defined as a contiguous block of characters, up to and including a <see cref="LineEnd"/>.<br/>
/// 
/// A "row" is being defined as a UI rendering term. Such that, without a line-wrap,
/// a line of text will be rendered at the same 'y-axis' position, with only a difference
/// in 'x-axis' between each character.
/// With line-wrap, a line can span many "row"(s) on the UI.
/// That is, the same line of text can be rendered as '1 row' up until the 'x-coordinate'
/// goes out of view. At that point the 'y-axis' will be incremented by the height of 1 'line',
/// and the 'x-coordinate' is reset, and more of the line can be rendered.<br/><br/>
/// 
/// <see cref="TextEditorModel"/> uses <see cref="ResourceUri"/> as its unique identifier.
/// Throughout this library, one finds <see cref="Key{T}"/> to be a unique identifier.
/// However, since <see cref="ResourceUri"/> should be unique,
/// <see cref="TextEditorModel"/> is an exception to this pattern.
/// </summary>
public interface ITextEditorModel
{
	/// <summary>
	/// Changed this property to an array from an ImmutableList (2024-08-13).
	/// =====================================================================
	/// The motivation for this change comes from calculating the virtualization result.
	///
	/// The storage for the underlying data of a 'ImmutableList' is believed to be more of
	/// a 'tree' structure than a contiguous array.
	///
	/// And therefore, when we virtualize vertically, and then horizontally after,
	/// this is an incredible amount of overhead if performed on a 'tree'-like structure.
	///
	/// A contiguous array is expected to be a dramatic improvement in performance
	/// when calculating the virtualization result.
	///
	/// A side note on this change: could .NET internally more easily leverage
	/// caching with this now being a contiguous array, rather than a 'tree'?
	///
	/// A worry: I'm not quite certain on the details of the idea in my head.
	/// It is something along lines of an array being treated as a struct
	/// and that this could cause a mess somehow?
	/// |
	/// Copying the entire list of rich characters versus just passing around a pointer
	/// kind of thing.
	/// |
	/// But, this array is located on an object, the text editor model, and it is the model that
	/// is being passed around in the code base. So this shouldn't be an issue.
	///
	/// Quite non-scientifically I simply took note of the memory usage that the Task Manager on windows
	/// reported for the IDE while this used to be an ImmutableList, before I made the change.
	/// |
	/// It was 1,744 MB of memory pre change.
	/// It is 1,568 MB of memory after change.
	///
	/// There is 0 control of variables going on here so the change of 200 MB perhaps
	/// is completely meaningless.
	///
	/// More so, I took a simple note of memory usage incase I see that the memory usage
	/// doubled or something absurd.
	///
	/// Would the app's memory go up, or would the so called ".NET Host" have its memory go up?
	/// I'm seeing in task manager a process called ".NET Host", separate to that of "Luthetus.Ide.Photino".
	///
	/// I'm now seeing 643 MB of memory usage after changing RichCharacter to a struct from a class
	/// </summary>
    public RichCharacter[] RichCharacterList { get; }
    public List<TextEditorPartition> PartitionList { get; }
    public List<ITextEditorEdit> EditBlockList { get; }
    /// <summary>
    /// Convert an index for a <see cref="LineEnd"/> to a <see cref="LineInformation"/>, use the method:
    /// <see cref="TextEditorModelExtensionMethods.GetLineInformation(ITextEditorModel, int)"/>
    /// </summary>
    public List<LineEnd> LineEndList { get; }
    public List<(LineEndKind lineEndKind, int count)> LineEndKindCountList { get; }
    public List<TextEditorPresentationModel> PresentationModelList { get; }
    /// <summary>
    /// Provides exact position index of a tab character
    /// </summary>
    public List<int> TabKeyPositionList { get; }
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
	/// This is now being used for <see cref="Displays.Internals.TextEditorFileExtensionHeaderDisplay"/>
	/// Whereas previously it was purely text (it only appeared as text in the <see cref="Displays.Internals.TextEditorFooter"/>)
	/// it will now be used to determine what header to display with see <see cref="ITextEditorHeaderRegistry"/>
	/// </summary>
	public string FileExtension { get; }
    public IDecorationMapper DecorationMapper { get; }
    public ICompilerService CompilerService { get; }
    public SaveFileHelper TextEditorSaveFileHelper { get; }
    public int EditBlockIndex { get; }
    public bool IsDirty { get; }
    public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple { get; }
    public Key<RenderState> RenderStateKey { get; }
    public string AllText { get; }

    public int LineCount { get; }
    public int CharCount { get; }
}
