using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Characters.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <inheritdoc cref="ITextEditorModel"/>
public partial class TextEditorModel
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
		var modifier = new TextEditorModelModifier(this);
		modifier.SetContent(content);

		_allText = modifier.AllText;
        RichCharacterList = modifier.RichCharacterList;
        PartitionList = modifier.PartitionList;
        LineEndKindCountList = modifier.LineEndKindCountList.ToImmutableList();
		LineEndList = modifier.LineEndList.ToImmutableList();
		TabKeyPositionList = modifier.TabKeyPositionList.ToImmutableList();
		OnlyLineEndKind = modifier.OnlyLineEndKind;
		LineEndKindPreference = modifier.LineEndKindPreference;
		MostCharactersOnASingleLineTuple = modifier.MostCharactersOnASingleLineTuple;
		EditBlockList = modifier.EditBlockList.ToImmutableList();
		EditBlockIndex = modifier.EditBlockIndex;
	}

	public TextEditorModel(
        string allText,
        ImmutableList<RichCharacter> richCharacterList,
        int partitionSize,
        ImmutableList<TextEditorPartition> partitionList,
		ImmutableList<ITextEditorEdit> editBlocksList,
		ImmutableList<LineEnd> rowEndingPositionsList,
		ImmutableList<(LineEndKind rowEndingKind, int count)> rowEndingKindCountsList,
		ImmutableList<TextEditorPresentationModel> presentationModelsList,
		ImmutableList<int> tabKeyPositionsList,
		LineEndKind? onlyRowEndingKind,
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
		_allText = allText;
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