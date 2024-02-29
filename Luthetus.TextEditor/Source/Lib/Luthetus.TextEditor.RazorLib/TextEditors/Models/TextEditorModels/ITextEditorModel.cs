using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

/// <summary>
/// Stores the <see cref="RichCharacter"/> class instances that represent the text.<br/><br/> Each TextEditorModel has a unique underlying resource uri.<br/><br/>
/// Therefore, if one has a text file named "myHomework.txt", then only one TextEditorModel can exist with the resource uri of "myHomework.txt".<br/><br/>
/// <see cref="TextEditorModel"/> uses <see cref="ResourceUri"/> as its unique identifier. Throughout this library, one finds <see cref="Key{T}"/> to be a unique identifier.
/// However, since <see cref="ResourceUri"/> should be unique, <see cref="TextEditorModel"/> is an exception to this pattern.
/// </summary>
public interface ITextEditorModel
{
    public ITextEditorContent Content { get; }
	public List<EditBlock> EditBlockList { get; }
	public List<TextEditorPresentationModel> PresentationModelList { get; }
	public RowEndingKind UsingRowEndingKind { get; }

    /// <summary>
    /// The resource uri acts as the <see cref="Key{T}"/> to uniquely identify a model.
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
    
}
