using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

/// <summary>
/// Stores the <see cref="RichCharacter"/> class instances that represent the text.<br/><br/>
/// Each TextEditorModel has a unique underlying resource uri.<br/><br/>
/// Therefore, if one has a text file named "myHomework.txt", then only one TextEditorModel
/// can exist with the resource uri of "myHomework.txt".
/// </summary>
public partial class TextEditorModel
{
    public TextEditorModel(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        IDecorationMapper? decorationMapper,
        ICompilerService? compilerService)
    {
        ResourceUri = resourceUri;
        ResourceLastWriteTime = resourceLastWriteTime;
        FileExtension = fileExtension;
        DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();
        CompilerService = compilerService ?? new TextEditorCompilerServiceDefault();

		var modifier = new TextEditorModelModifier(this);

		modifier.ModifyContent(content);

		ContentBag = modifier.ContentBag.ToImmutableList();
		RowEndingKindCountsBag = modifier.RowEndingKindCountsBag.ToImmutableList();
		RowEndingPositionsBag = modifier.RowEndingPositionsBag.ToImmutableList();
		TabKeyPositionsBag = modifier.TabKeyPositionsBag.ToImmutableList();
		OnlyRowEndingKind = modifier.OnlyRowEndingKind;
		UsingRowEndingKind = modifier.UsingRowEndingKind;
		MostCharactersOnASingleRowTuple = modifier.MostCharactersOnASingleRowTuple;
	}

	public TextEditorModel(
		ImmutableList<RichCharacter> contentBag,
		ImmutableList<EditBlock> editBlocksBag,
		ImmutableList<(int positionIndex, RowEndingKind rowEndingKind)> rowEndingPositionsBag,
		ImmutableList<(RowEndingKind rowEndingKind, int count)> rowEndingKindCountsBag,
		ImmutableList<TextEditorPresentationModel> presentationModelsBag,
		ImmutableList<int> tabKeyPositionsBag,
		RowEndingKind? onlyRowEndingKind,
		RowEndingKind usingRowEndingKind,
		ResourceUri resourceUri,
		DateTime resourceLastWriteTime,
		string fileExtension,
		IDecorationMapper decorationMapper,
		ICompilerService compilerService,
		TextEditorSaveFileHelper textEditorSaveFileHelper,
		int editBlockIndex,
		(int rowIndex, int rowLength) mostCharactersOnASingleRowTuple,
		Key<RenderState>  renderStateKey)
	{

		ContentBag = contentBag;
		EditBlocksBag = editBlocksBag;
		RowEndingPositionsBag = rowEndingPositionsBag;
		RowEndingKindCountsBag = rowEndingKindCountsBag;
		PresentationModelsBag = presentationModelsBag;
		TabKeyPositionsBag = tabKeyPositionsBag;
		OnlyRowEndingKind = onlyRowEndingKind;
		UsingRowEndingKind = usingRowEndingKind;
		ResourceUri = resourceUri;
		ResourceLastWriteTime = resourceLastWriteTime;
		FileExtension = fileExtension;
		DecorationMapper = decorationMapper;
		CompilerService = compilerService;
		TextEditorSaveFileHelper = textEditorSaveFileHelper;
		EditBlockIndex = editBlockIndex;
		MostCharactersOnASingleRowTuple = mostCharactersOnASingleRowTuple;
		RenderStateKey = renderStateKey;
	}
}