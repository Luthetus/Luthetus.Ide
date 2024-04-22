using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

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
		modifier.SetContent(content);

        CharList = modifier.CharList;
        DecorationByteList = modifier.DecorationByteList;
        PartitionList = modifier.PartitionList;
        LineEndKindCountList = modifier.LineEndKindCountList.ToImmutableList();
		LineEndList = modifier.LineEndList.ToImmutableList();
		TabKeyPositionList = modifier.TabKeyPositionList.ToImmutableList();
		OnlyLineEndKind = modifier.OnlyLineEndKind;
		LineEndKindPreference = modifier.LineEndKindPreference;
		MostCharactersOnASingleLineTuple = modifier.MostCharactersOnASingleLineTuple;
	}

	public TextEditorModel(
        ImmutableList<char> charList,
        ImmutableList<byte> decorationByteList,
        int partitionSize,
        ImmutableList<TextEditorPartition> partitionList,
		ImmutableList<EditBlock> editBlocksList,
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
		ILuthCompilerService compilerService,
		SaveFileHelper textEditorSaveFileHelper,
		int editBlockIndex,
        bool isDirty,
        (int rowIndex, int rowLength) mostCharactersOnASingleRowTuple,
		Key<RenderState>  renderStateKey)
	{
        CharList = charList;
        DecorationByteList = decorationByteList;
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