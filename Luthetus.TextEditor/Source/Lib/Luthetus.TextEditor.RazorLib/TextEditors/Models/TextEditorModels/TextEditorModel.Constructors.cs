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

        // (2024-02-29) Plan to add text editor partitioning #Step 100:
        // --------------------------------------------------
        // Change 'ContentList' from 'List<RichCharacter>?' to 'List<List<RichCharacter>>?
        //
        // (2024-02-29) Plan to add text editor partitioning #Step 900:
        // --------------------------------------------------
        // I'm receiving a compilation error that 'ContentList' cannot be assigned to,
        // because it is readonly.
        //
        // For this reasoning, I'm going to remove all the code statements
        // of 'ContentList = modifier.ContentList;'
        //
        // (2024-02-29) Plan to add text editor partitioning #Step 1,500:
        // --------------------------------------------------
        // When invoking this constructor, one is finalizing their modifications.
        // Therefore, the ContentList is too finalized.
        // So, instead of recalculating each time the getter is referenced,
        // the TextEditorModel should set its ContentList.
        // 
        // We need to keep the TextEditorModelModifier using an expression bound property however,
        // since its content is constantly changing.
        ContentList = modifier.ContentList;
        PartitionList = modifier.PartitionList;
        RowEndingKindCountsList = modifier.RowEndingKindCountsList.ToImmutableList();
		RowEndingPositionsList = modifier.RowEndingPositionsList.ToImmutableList();
		TabKeyPositionsList = modifier.TabKeyPositionsList.ToImmutableList();
		OnlyRowEndingKind = modifier.OnlyRowEndingKind;
		UsingRowEndingKind = modifier.UsingRowEndingKind;
		MostCharactersOnASingleRowTuple = modifier.MostCharactersOnASingleRowTuple;
	}

	public TextEditorModel(
        // (2024-02-29) Plan to add text editor partitioning #Step 100:
        // --------------------------------------------------
        // Change 'contentList' from 'List<RichCharacter>?' to 'List<List<RichCharacter>>?
        IReadOnlyList<RichCharacter> contentList,
        int partitionSize,
        ImmutableList<ImmutableList<RichCharacter>> partitionList,
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
        // (2024-02-29) Plan to add text editor partitioning #Step 900:
        // --------------------------------------------------
        // I'm receiving a compilation error that 'ContentList' cannot be assigned to,
        // because it is readonly.
        //
        // For this reasoning, I'm going to remove all the code statements
        // of 'ContentList = contentList;'
        //
        // (2024-02-29) Plan to add text editor partitioning #Step 1,500:
        // --------------------------------------------------
        // When invoking this constructor, one is finalizing their modifications.
        // Therefore, the ContentList is too finalized.
        // So, instead of recalculating each time the getter is referenced,
        // the TextEditorModel should set its ContentList.
        // 
        // We need to keep the TextEditorModelModifier using an expression bound property however,
        // since its content is constantly changing.
        ContentList = contentList;
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