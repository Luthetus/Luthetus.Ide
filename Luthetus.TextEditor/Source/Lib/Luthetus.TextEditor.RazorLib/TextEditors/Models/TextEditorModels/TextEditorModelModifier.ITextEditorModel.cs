﻿using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Partitions.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModelModifier : ITextEditorModel
{
	public PartitionedImmutableList<RichCharacter> ContentList => _contentList is null ? _textEditorModel.ContentList : _contentList;
	public IList<EditBlock> EditBlocksList => _editBlocksList is null ? _textEditorModel.EditBlocksList : _editBlocksList;
	public IList<RowEnding> RowEndingPositionsList => _rowEndingPositionsList is null ? _textEditorModel.RowEndingPositionsList : _rowEndingPositionsList;
	public IList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountsList => _rowEndingKindCountsList is null ? _textEditorModel.RowEndingKindCountsList : _rowEndingKindCountsList;
	public IList<TextEditorPresentationModel> PresentationModelsList => _presentationModelsList is null ? _textEditorModel.PresentationModelsList : _presentationModelsList;
	public IList<int> TabKeyPositionsList => _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionsList : _tabKeyPositionsList;
	public RowEndingKind? OnlyRowEndingKind => _onlyRowEndingKindWasModified ? _onlyRowEndingKind : _textEditorModel.OnlyRowEndingKind;
	public RowEndingKind UsingRowEndingKind => _usingRowEndingKind ?? _textEditorModel.UsingRowEndingKind;
	public ResourceUri ResourceUri => _resourceUri ?? _textEditorModel.ResourceUri;
	public DateTime ResourceLastWriteTime => _resourceLastWriteTime ?? _textEditorModel.ResourceLastWriteTime;
	public string FileExtension => _fileExtension ?? _textEditorModel.FileExtension;
	public IDecorationMapper DecorationMapper => _decorationMapper ?? _textEditorModel.DecorationMapper;
	public ILuthCompilerService CompilerService => _compilerService ?? _textEditorModel.CompilerService;
	public TextEditorSaveFileHelper TextEditorSaveFileHelper => _textEditorSaveFileHelper ?? _textEditorModel.TextEditorSaveFileHelper;
	public int EditBlockIndex => _editBlockIndex ?? _textEditorModel.EditBlockIndex;
	public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple => _mostCharactersOnASingleRowTuple ?? _textEditorModel.MostCharactersOnASingleRowTuple;
	public Key<RenderState> RenderStateKey => _renderStateKey ?? _textEditorModel.RenderStateKey;

    public int RowCount => RowEndingPositionsList.Count;
    public int DocumentLength => ContentList.Count;
}