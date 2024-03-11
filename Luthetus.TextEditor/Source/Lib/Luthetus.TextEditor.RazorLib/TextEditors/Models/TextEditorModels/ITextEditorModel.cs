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
    // (2024-02-29) Plan to add text editor partitioning #Step 100:
    // --------------------------------------------------
    // Change 'ContentList' from 'List<RichCharacter>?' to 'List<List<RichCharacter>>?
    //
    // (2024-02-29) Plan to add text editor partitioning #Step 200:
    // --------------------------------------------------
    // Since 'ContentList' is now a 'List<List<RichCharacter>>?, then any enumeration on ContentList will
    // break. Since the type 'RichCharacter' was expected, but now they get List<RichCharacter>.
    //
    // So, a way to loop over all the rich characters themselves needs to be added.
    // Perhaps this is done by performing a '.SelectMany(...)' over the partitions,
    // and selecting out the rich characters.
    //
    // List<RichCharacter> RichCharacterList = ContentList.SelectMany(partition => partition);
    //
    // Once the 'TextEditorModelModifier' is finished, then the data becomes immutable in the form of
    // 'TextEditorModel'. As such, the 'TextEditorModelModifier' should pre-calculate
    // 'RichCharacterList = ContentList.SelectMany(partition => partition);'
    //
    // If one has the 'TextEditorModel' use 'RichCharacterList' then would one have to recreate
    // the partitions when next making a 'TextEditorModelModifier'?
    //
    // In otherwords, would one continually be constructing a 'List<RichCharacter>' to
    // a 'List<List<RichCharacter>>' to 'List<RichCharacter>', over and over each time an edit needs to be made?
    //
    // No, because 'List<RichCharacter>' can be a separate property from 'List<List<RichCharacter>>'.
    //
    // The 'List<List<RichCharacter>>' can always exist and be shared between the 'TextEditorModel' and
    // the 'TextEditorModelModifier'. The only 'new' would be when the 'modifier' returns a new instance of
    // 'TextEditorModel', as here the partitions will be flattened into a List<RichCharacter>. When the 'TextEditorModel'
    // makes an instance of the 'modifier', the shared partitions will be re-used.
    //
    // If the partitions are shared amongst the 'TextEditorModel' and the 'modifier', then the immutability could break.
    // In order to have thread safety it is important that the state is immutable.
    //
    // So, instead of "Change 'ContentList' from 'List<RichCharacter>?' to 'List<List<RichCharacter>>?"
    // one should "Change 'ContentList' from 'List<RichCharacter>?' to 'ImmutableList<ImmutableList<RichCharacter>>?"
    //
    // The goal of the partitioning is to reduce the amount of data being moved around each time an edit occurs.
    // For example, if an editor has 20,0000 characters in it. And the entire editor is a single 'ImmutableList<RichCharacter>',
    // then a new 'ImmutableList<RichCharacter>' needs to be made, then 20,000 pointers to a 'RichCharacter' need to be
    // added to that new 'ImmutableList<RichCharacter>'.
    //
    // It likely is the case that there is more going on behind the scenes when one edits the 'ImmutableList<RichCharacter>'.
    // But, I think what I described is a useful mental model for the issue. As you type more and more into a single editor,
    // it takes longer and longer to shift around all the data, even if it might only be copies of pointers.
    // How long does it take to add 20,000 entries to a List? How long would it then take for 40,000 entries?
    //
    // The efficiency of creating an 'ImmutableList<RichCharacter>' comes into play even more so, due to RichCharacter(s)
    // not being the only tracked datatype. One must also track the: line endings, tab characters, compiler service
    // diagnostics (squigglies), and more. With all these 'ImmutableList<T>', the amount of data being churned
    // every time an edit is made becomes un-bearable. In this inefficient state, the editor takes a noticeable
    // amount of time to write a character that one types to the screen. If one holds down a key, as to type
    // many of that key, the editor starts to "stutter" of sorts. It writes out a few, then the UI freezes,
    // then it writes some more, then the UI freezes, on and on until the user lets go of the key.
    //
    // If one sets the partition size to 5,000, and has a text editor with 20,000 character in it.
    // Then, every edit to the document would only require an 'ImmutableList<RichCharacter>' to be created with
    // 5,000 entries. This is in constrast to the entire text editor needing to be re-created which would mean 20,000 entries.
    //
    // (2024-02-29) Plan to add text editor partitioning #Step 300:
    // --------------------------------------------------
    // To finalize the plan for 'ContentList'. I want to
    // change 'ContentList' from 'List<RichCharacter>?' to 'ImmutableList<ImmutableList<RichCharacter>>?
    //
    // Following that change, I want to not touch any of the other tracked data. For example,
    // don't touch 'EditBlocksList'. Provided I do all my code in 'TextEditorModelModifier.Main.cs',
    // then 'EditBlocksList' will not break.
    public IReadOnlyList<RichCharacter> ContentList { get; }
    public ImmutableList<ImmutableList<RichCharacter>> PartitionList { get; }
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
    public bool IsDirty { get; }
    public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple { get; }
	public Key<RenderState> RenderStateKey { get; }

    public int RowCount { get; }
    public int DocumentLength { get; }
}
