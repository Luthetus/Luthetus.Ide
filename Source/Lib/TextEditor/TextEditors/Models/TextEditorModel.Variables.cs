﻿using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModel
{
    public const int TAB_WIDTH = 4;
    public const int GUTTER_PADDING_LEFT_IN_PIXELS = 5;
    public const int GUTTER_PADDING_RIGHT_IN_PIXELS = 15;
    public const int MAXIMUM_EDIT_BLOCKS = 10;
    public const int MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN = 5;

    public ImmutableList<char> CharList { get; init; }
    public ImmutableList<byte> DecorationByteList { get; init; }
    public ImmutableList<TextEditorPartition> PartitionList { get; init; }

    public ImmutableList<EditBlock> EditBlocksList { get; init; } = ImmutableList<EditBlock>.Empty;

    /// <inheritdoc cref="IModelTextEditor.LineEndPositionList"/>
	public ImmutableList<LineEnd> LineEndPositionList { get; init; } = ImmutableList<LineEnd>.Empty;
	public ImmutableList<(LineEndKind lineEndKind, int count)> LineEndKindCountList { get; init; } = ImmutableList<(LineEndKind lineEndingKind, int count)>.Empty;
	public ImmutableList<TextEditorPresentationModel> PresentationModelList { get; init; } = ImmutableList<TextEditorPresentationModel>.Empty;
    
    /// <inheritdoc cref="IModelTextEditor.TabKeyPositionsList"/>
	public ImmutableList<int> TabKeyPositionsList = ImmutableList<int>.Empty;
    
    /// <inheritdoc cref="IModelTextEditor.OnlyLineEndKind"/>
    public LineEndKind? OnlyLineEndKind { get; init; }
    public LineEndKind UsingLineEndKind { get; init; }
    
    /// <inheritdoc cref="IModelTextEditor.ResourceUri"/>
    public ResourceUri ResourceUri { get; init; }
    public DateTime ResourceLastWriteTime { get; init; }
    public int PartitionSize { get; init; }
    
    /// <inheritdoc cref="IModelTextEditor.FileExtension"/>
    public string FileExtension { get; init; }
    public IDecorationMapper DecorationMapper { get; init; }
    public ILuthCompilerService CompilerService { get; init; }
    public SaveFileHelper TextEditorSaveFileHelper { get; init; } = new();
    public int EditBlockIndex { get; init; }
    public bool IsDirty { get; init; }
	public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple { get; init; }
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();

    public int LineCount => LineEndPositionList.Count;
    public int DocumentLength => CharList.Count;
}