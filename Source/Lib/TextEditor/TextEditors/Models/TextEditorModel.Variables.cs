using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorModel
{
    public const int TAB_WIDTH = 4;
    public const int GUTTER_PADDING_LEFT_IN_PIXELS = 5;
    public const int GUTTER_PADDING_RIGHT_IN_PIXELS = 15;
    public const int MAXIMUM_EDIT_BLOCKS = 10;
    public const int MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN = 5;

    public string? __AllText;

    public RichCharacter[] RichCharacterList { get; init; }
    public List<TextEditorPartition> PartitionList { get; init; } = new List<TextEditorPartition> { new TextEditorPartition(new List<RichCharacter>()) };

    public List<ITextEditorEdit> EditBlockList { get; init; } = new();

    /// <inheritdoc cref="ITextEditorModel.LineEndList"/>
	public List<LineEnd> LineEndList { get; init; } = new();
	public List<(LineEndKind lineEndKind, int count)> LineEndKindCountList { get; init; } = new();
	public List<TextEditorPresentationModel> PresentationModelList { get; init; } = new();
    
    /// <inheritdoc cref="ITextEditorModel.TabKeyPositionList"/>
	public List<int> TabKeyPositionList { get; } = new();
    
    /// <inheritdoc cref="ITextEditorModel.OnlyLineEndKind"/>
    public LineEndKind OnlyLineEndKind { get; init; }
    public LineEndKind LineEndKindPreference { get; init; }
    
    /// <inheritdoc cref="ITextEditorModel.ResourceUri"/>
    public ResourceUri ResourceUri { get; init; }
    public DateTime ResourceLastWriteTime { get; init; }
    public int PartitionSize { get; init; }
    
    /// <inheritdoc cref="ITextEditorModel.FileExtension"/>
    public string FileExtension { get; init; }
    public IDecorationMapper DecorationMapper { get; init; }
    public ICompilerService CompilerService { get; init; }
    public SaveFileHelper TextEditorSaveFileHelper { get; init; } = new();
    public int EditBlockIndex { get; init; }
    public bool IsDirty { get; init; }
	public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple { get; init; }
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();

    public string AllText => __AllText ??= new string (RichCharacterList.Select(x => x.Value).ToArray());

    public int LineCount => LineEndList.Count;
    public int DocumentLength => RichCharacterList.Length;
    
    RichCharacter[] ITextEditorModel.RichCharacterList => RichCharacterList;

    int ITextEditorModel.LineCount => LineCount;
    int ITextEditorModel.CharCount => DocumentLength;
}