using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <inheritdoc cref="ITextEditorModel"/>
public sealed partial class TextEditorModel : ITextEditorModel
{
    public TextEditorModel(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        IDecorationMapper? decorationMapper,
        ICompilerService? compilerService,
		int partitionSize = 4_096)
    {
        ResourceUri = resourceUri;
        ResourceLastWriteTime = resourceLastWriteTime;
        FileExtension = fileExtension;
        DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();
        CompilerService = compilerService ?? new CompilerServiceDoNothing();

		PartitionSize = partitionSize;
		var modifier = new TextEditorModel(this, __AllText);
		modifier.SetContent(content);

		__AllText = modifier.AllText;
        RichCharacterList = modifier.RichCharacterList;
        PartitionList = modifier.PartitionList;
        LineEndKindCountList = modifier.LineEndKindCountList;
		LineEndList = modifier.LineEndList;
		TabKeyPositionList = modifier.TabKeyPositionList;
		OnlyLineEndKind = modifier.OnlyLineEndKind;
		LineEndKindPreference = modifier.LineEndKindPreference;
		MostCharactersOnASingleLineTuple = modifier.MostCharactersOnASingleLineTuple;
		EditBlockList = modifier.EditBlockList;
		EditBlockIndex = modifier.EditBlockIndex;
	}

	public TextEditorModel(
        string allText,
        RichCharacter[] richCharacterList,
        int partitionSize,
        List<TextEditorPartition> partitionList,
		List<ITextEditorEdit> editBlocksList,
		List<LineEnd> rowEndingPositionsList,
		List<(LineEndKind rowEndingKind, int count)> rowEndingKindCountsList,
		List<TextEditorPresentationModel> presentationModelsList,
		List<int> tabKeyPositionsList,
		LineEndKind onlyRowEndingKind,
		LineEndKind usingRowEndingKind,
		ResourceUri resourceUri,
		DateTime resourceLastWriteTime,
		string fileExtension,
		IDecorationMapper decorationMapper,
		ICompilerService compilerService,
		SaveFileHelper textEditorSaveFileHelper,
		int editBlockIndex,
        bool isDirty,
        (int rowIndex, int rowLength) mostCharactersOnASingleRowTuple,
		Key<RenderState>  renderStateKey)
	{
		__AllText = allText;
        RichCharacterList = richCharacterList;
        PartitionSize = partitionSize;
        PartitionList = partitionList;
		EditBlockList = editBlocksList;
		LineEndList = rowEndingPositionsList;
		LineEndKindCountList = rowEndingKindCountsList;
		PresentationModelList = presentationModelsList;
		TabKeyPositionList = tabKeyPositionsList;
		OnlyLineEndKind = onlyRowEndingKind;
		LineEndKindPreference = usingRowEndingKind;
		ResourceUri = resourceUri;
		ResourceLastWriteTime = resourceLastWriteTime;
		FileExtension = fileExtension;
		DecorationMapper = decorationMapper;
		CompilerService = compilerService;
		TextEditorSaveFileHelper = textEditorSaveFileHelper;
		EditBlockIndex = editBlockIndex;
        IsDirty = isDirty;
		MostCharactersOnASingleLineTuple = mostCharactersOnASingleRowTuple;
		RenderStateKey = renderStateKey;
	}
}