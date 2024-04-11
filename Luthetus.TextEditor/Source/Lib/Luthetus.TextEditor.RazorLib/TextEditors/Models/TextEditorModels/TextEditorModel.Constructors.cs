using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

/// <inheritdoc cref="ITextEditorModel"/>
public partial class TextEditorModel
{
    public TextEditorModel(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        IDecorationMapper? decorationMapper,
        ILuthCompilerService? compilerService,
		int partitionSize = 4_096)
    {
        ResourceUri = resourceUri;
        ResourceLastWriteTime = resourceLastWriteTime;
        FileExtension = fileExtension;
        DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();
        CompilerService = compilerService ?? new LuthCompilerService(null);

		PartitionSize = partitionSize;
		var modifier = new TextEditorModelModifier(this);
		modifier.ModifyContent(content);

        CharList = modifier.CharList;
        DecorationByteList = modifier.DecorationByteList;
        PartitionList = modifier.PartitionList;
        RowEndingKindCountsList = modifier.RowEndingKindCountsList.ToImmutableList();
		RowEndingPositionsList = modifier.RowEndingPositionsList.ToImmutableList();
		TabKeyPositionsList = modifier.TabKeyPositionsList.ToImmutableList();
		OnlyRowEndingKind = modifier.OnlyRowEndingKind;
		UsingRowEndingKind = modifier.UsingRowEndingKind;
		MostCharactersOnASingleRowTuple = modifier.MostCharactersOnASingleRowTuple;
	}

	public TextEditorModel(
        ImmutableList<char> charList,
        ImmutableList<byte> decorationByteList,
        int partitionSize,
        ImmutableList<TextEditorPartition> partitionList,
		ImmutableList<EditBlock> editBlocksList,
		ImmutableList<RowEnding> rowEndingPositionsList,
		ImmutableList<(RowEndingKind rowEndingKind, int count)> rowEndingKindCountsList,
		ImmutableList<TextEditorPresentationModel> presentationModelsList,
		ImmutableList<int> tabKeyPositionsList,
		RowEndingKind? onlyRowEndingKind,
		RowEndingKind usingRowEndingKind,
		ResourceUri resourceUri,
		DateTime resourceLastWriteTime,
		string fileExtension,
		IDecorationMapper decorationMapper,
		ILuthCompilerService compilerService,
		TextEditorSaveFileHelper textEditorSaveFileHelper,
		int editBlockIndex,
        bool isDirty,
        (int rowIndex, int rowLength) mostCharactersOnASingleRowTuple,
		Key<RenderState>  renderStateKey)
	{
        CharList = charList;
        DecorationByteList = decorationByteList;
        PartitionSize = partitionSize;
        PartitionList = partitionList;
		EditBlocksList = editBlocksList;
		RowEndingPositionsList = rowEndingPositionsList;
		RowEndingKindCountsList = rowEndingKindCountsList;
		PresentationModelsList = presentationModelsList;
		TabKeyPositionsList = tabKeyPositionsList;
		OnlyRowEndingKind = onlyRowEndingKind;
		UsingRowEndingKind = usingRowEndingKind;
		ResourceUri = resourceUri;
		ResourceLastWriteTime = resourceLastWriteTime;
		FileExtension = fileExtension;
		DecorationMapper = decorationMapper;
		CompilerService = compilerService;
		TextEditorSaveFileHelper = textEditorSaveFileHelper;
		EditBlockIndex = editBlockIndex;
        IsDirty = isDirty;
		MostCharactersOnASingleRowTuple = mostCharactersOnASingleRowTuple;
		RenderStateKey = renderStateKey;
	}
}