using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value
///
/// When reading state, if the state had been 'null coallesce assigned' then the field will
/// be read. Otherwise, the existing TextEditorModel's value will be read.
/// <br/><br/>
/// <inheritdoc cref="IModelTextEditor"/>
/// </summary>
public partial class TextEditorModelModifier : IModelTextEditor
{
    private readonly TextEditorModel _textEditorModel;

    public TextEditorModelModifier(TextEditorModel model)
    {
        if (model.PartitionSize < 2)
            throw new ApplicationException($"{nameof(model)}.{nameof(PartitionSize)} must be >= 2");

        PartitionSize = model.PartitionSize;
        WasDirty = model.IsDirty;

        _isDirty = model.IsDirty;

        _textEditorModel = model;
        _partitionList = _textEditorModel.PartitionList;
    }

    public ImmutableList<char> CharList => _charList is null ? _textEditorModel.CharList : _charList;
    public ImmutableList<byte> DecorationByteList => _decorationByteList is null ? _textEditorModel.DecorationByteList : _decorationByteList;
    public ImmutableList<TextEditorPartition> PartitionList => _partitionList is null ? _textEditorModel.PartitionList : _partitionList;

    public IList<EditBlock> EditBlockList => _editBlocksList is null ? _textEditorModel.EditBlocksList : _editBlocksList;
    public IList<LineEnd> LineEndPositionList => _lineEndPositionList is null ? _textEditorModel.LineEndPositionList : _lineEndPositionList;
    public IList<(LineEndKind lineEndKind, int count)> LineEndKindCountsList => _lineEndKindCountList is null ? _textEditorModel.LineEndKindCountList : _lineEndKindCountList;
    public IList<TextEditorPresentationModel> PresentationModelsList => _presentationModelsList is null ? _textEditorModel.PresentationModelList : _presentationModelsList;
    public IList<int> TabKeyPositionsList => _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionsList : _tabKeyPositionsList;
    public LineEndKind? OnlyLineEndKind => _onlyLineEndKindWasModified ? _onlyLineEndKind : _textEditorModel.OnlyLineEndKind;
    public LineEndKind UsingLineEndKind => _usingLineEndKind ?? _textEditorModel.UsingLineEndKind;
    public ResourceUri ResourceUri => _resourceUri ?? _textEditorModel.ResourceUri;
    public DateTime ResourceLastWriteTime => _resourceLastWriteTime ?? _textEditorModel.ResourceLastWriteTime;
    public string FileExtension => _fileExtension ?? _textEditorModel.FileExtension;
    public IDecorationMapper DecorationMapper => _decorationMapper ?? _textEditorModel.DecorationMapper;
    public ILuthCompilerService CompilerService => _compilerService ?? _textEditorModel.CompilerService;
    public SaveFileHelper TextEditorSaveFileHelper => _textEditorSaveFileHelper ?? _textEditorModel.TextEditorSaveFileHelper;
    public int EditBlockIndex => _editBlockIndex ?? _textEditorModel.EditBlockIndex;
    public bool IsDirty => _isDirty;
    public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple => _mostCharactersOnASingleLineTuple ?? _textEditorModel.MostCharactersOnASingleLineTuple;
    public Key<RenderState> RenderStateKey => _renderStateKey ?? _textEditorModel.RenderStateKey;

    public int LineCount => LineEndPositionList.Count;
    public int DocumentLength => _charList.Count;

    /// <summary>
    /// TODO: Awkward naming convention is being used here. This is an expression bound property,...
    ///       ...with the naming conventions of a private field.
    ///       |
    ///       The reason for breaking convention here is that every other purpose of this type is done
    ///       through a private field.
    ///       |
    ///       Need to revisit naming this.
    /// </summary>
    private ImmutableList<char> _charList => _partitionList.SelectMany(x => x.CharList).ToImmutableList();
    /// <summary>
    /// TODO: Awkward naming convention is being used here. This is an expression bound property,...
    ///       ...with the naming conventions of a private field.
    ///       |
    ///       The reason for breaking convention here is that every other purpose of this type is done
    ///       through a private field.
    ///       |
    ///       Need to revisit naming this.
    /// </summary>
    private ImmutableList<byte> _decorationByteList => _partitionList.SelectMany(x => x.DecorationByteList).ToImmutableList();
    private ImmutableList<TextEditorPartition> _partitionList = new TextEditorPartition[] { TextEditorPartition.Empty }.ToImmutableList();

    private List<EditBlock>? _editBlocksList;
    private List<LineEnd>? _lineEndPositionList;
    private List<(LineEndKind lineEndKind, int count)>? _lineEndKindCountList;
    private List<TextEditorPresentationModel>? _presentationModelsList;
    private List<int>? _tabKeyPositionsList;

    private LineEndKind? _onlyLineEndKind;
    /// <summary>
    /// Awkward special case here: <see cref="_onlyLineEndKind"/> is allowed to be null.
    /// So, the design of this class where null means unmodified, doesn't work well here.
    /// </summary>
    private bool _onlyLineEndKindWasModified;

    private LineEndKind? _usingLineEndKind;
    private ResourceUri? _resourceUri;
    private DateTime? _resourceLastWriteTime;
    private string? _fileExtension;
    private IDecorationMapper? _decorationMapper;
    private ILuthCompilerService? _compilerService;
    private SaveFileHelper? _textEditorSaveFileHelper;
    private int? _editBlockIndex;
    private bool _isDirty;
    private (int rowIndex, int rowLength)? _mostCharactersOnASingleLineTuple;
    private Key<RenderState>? _renderStateKey = Key<RenderState>.NewKey();
    private Keymap? _textEditorKeymap;
    private TextEditorOptions? _textEditorOptions;

    /// <summary>
    /// This property optimizes the dirty state tracking. If _wasDirty != _isDirty then track the state change.
    /// This involves writing to dependency injectable state, then triggering a re-render in the <see cref="Edits.Displays.DirtyResourceUriInteractiveIconDisplay"/>
    /// </summary>
    public bool WasDirty { get; }

    private int PartitionSize { get; }
    public bool WasModified { get; internal set; }

    public TextEditorModel ToModel()
    {
        return new TextEditorModel(
            _charList is null ? _textEditorModel.CharList : _charList,
            _decorationByteList is null ? _textEditorModel.DecorationByteList : _decorationByteList,
            PartitionSize,
            _partitionList is null ? _textEditorModel.PartitionList : _partitionList,
            _editBlocksList is null ? _textEditorModel.EditBlocksList : _editBlocksList.ToImmutableList(),
            _lineEndPositionList is null ? _textEditorModel.LineEndPositionList : _lineEndPositionList.ToImmutableList(),
            _lineEndKindCountList is null ? _textEditorModel.LineEndKindCountList : _lineEndKindCountList.ToImmutableList(),
            _presentationModelsList is null ? _textEditorModel.PresentationModelList : _presentationModelsList.ToImmutableList(),
            _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionsList : _tabKeyPositionsList.ToImmutableList(),
            _onlyLineEndKindWasModified ? _onlyLineEndKind : _textEditorModel.OnlyLineEndKind,
            _usingLineEndKind ?? _textEditorModel.UsingLineEndKind,
            _resourceUri ?? _textEditorModel.ResourceUri,
            _resourceLastWriteTime ?? _textEditorModel.ResourceLastWriteTime,
            _fileExtension ?? _textEditorModel.FileExtension,
            _decorationMapper ?? _textEditorModel.DecorationMapper,
            _compilerService ?? _textEditorModel.CompilerService,
            _textEditorSaveFileHelper ?? _textEditorModel.TextEditorSaveFileHelper,
            _editBlockIndex ?? _textEditorModel.EditBlockIndex,
            IsDirty,
            _mostCharactersOnASingleLineTuple ?? _textEditorModel.MostCharactersOnASingleLineTuple,
            _renderStateKey ?? _textEditorModel.RenderStateKey);
    }

    public void ClearContent()
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
            _lineEndPositionList ??= _textEditorModel.LineEndPositionList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionsList.ToList();
            _lineEndKindCountList ??= _textEditorModel.LineEndKindCountList.ToList();
        }

        _mostCharactersOnASingleLineTuple = (0, TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

        _partitionList = new TextEditorPartition[] { TextEditorPartition.Empty }.ToImmutableList();

        _lineEndPositionList = new List<LineEnd> 
        {
            new LineEnd(0, 0, LineEndKind.EndOfFile)
        };

        _lineEndKindCountList = new List<(LineEndKind rowEndingKind, int count)>
        {
            (LineEndKind.CarriageReturn, 0),
            (LineEndKind.LineFeed, 0),
            (LineEndKind.CarriageReturnLineFeed, 0),
        };

        _tabKeyPositionsList = new List<int>();

        SetIsDirtyTrue();
    }

    public void ClearOnlyRowEndingKind()
    {
        _onlyLineEndKind = null;
        _onlyLineEndKindWasModified = true;
    }

    public void SetUsingLineEndKind(LineEndKind rowEndingKind)
    {
        _usingLineEndKind = rowEndingKind;
    }

    public void SetResourceData(ResourceUri resourceUri, DateTime resourceLastWriteTime)
    {
        _resourceUri = resourceUri;
        _resourceLastWriteTime = resourceLastWriteTime;
    }

    public void SetDecorationMapper(IDecorationMapper decorationMapper)
    {
        _decorationMapper = decorationMapper;
    }

    public void SetCompilerService(ILuthCompilerService compilerService)
    {
        _compilerService = compilerService;
    }

    public void SetTextEditorSaveFileHelper(SaveFileHelper textEditorSaveFileHelper)
    {
        _textEditorSaveFileHelper = textEditorSaveFileHelper;
    }

    public void SetContent(string content)
    {
        SetIsDirtyTrue();

        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
            _lineEndPositionList ??= _textEditorModel.LineEndPositionList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionsList.ToList();
            _lineEndKindCountList ??= _textEditorModel.LineEndKindCountList.ToList();
        }

        ClearAllStatesButKeepEditHistory();

        var rowIndex = 0;
        var previousCharacter = '\0';

        var charactersOnRow = 0;

        var carriageReturnCount = 0;
        var linefeedCount = 0;
        var carriageReturnLinefeedCount = 0;

        for (var index = 0; index < content.Length; index++)
        {
            var character = content[index];

            charactersOnRow++;

            if (character == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
            {
                if (charactersOnRow > MostCharactersOnASingleLineTuple.lineLength - TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
                    _mostCharactersOnASingleLineTuple = (rowIndex, charactersOnRow + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                LineEndPositionList.Insert(rowIndex, new(index, index + 1, LineEndKind.CarriageReturn));
                rowIndex++;

                charactersOnRow = 0;

                carriageReturnCount++;
            }
            else if (character == KeyboardKeyFacts.WhitespaceCharacters.NEW_LINE)
            {
                if (charactersOnRow > MostCharactersOnASingleLineTuple.lineLength - TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
                    _mostCharactersOnASingleLineTuple = (rowIndex, charactersOnRow + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

                if (previousCharacter == KeyboardKeyFacts.WhitespaceCharacters.CARRIAGE_RETURN)
                {
                    var lineEnding = LineEndPositionList[rowIndex - 1];
                    lineEnding.EndPositionIndexExclusive++;
                    lineEnding.LineEndKind = LineEndKind.CarriageReturnLineFeed;

                    carriageReturnCount--;
                    carriageReturnLinefeedCount++;
                }
                else
                {
                    LineEndPositionList.Insert(rowIndex, new(index, index + 1, LineEndKind.LineFeed));
                    rowIndex++;

                    linefeedCount++;
                }

                charactersOnRow = 0;
            }

            if (character == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                TabKeyPositionsList.Add(index);

            previousCharacter = character;
        }

        __InsertRange(0, content.Select(x => new RichCharacter
        {
            Value = x,
            DecorationByte = default,
        }));

        // Update the line end count list (TODO: Fix the awkward tuple not a variable logic going on here)
        {
            {
                var indexCarriageReturn = _lineEndKindCountList.FindIndex(x => x.lineEndKind == LineEndKind.CarriageReturn);
                _lineEndKindCountList[indexCarriageReturn] = (LineEndKind.CarriageReturn, carriageReturnCount);
            }
            {
                var indexLineFeed = _lineEndKindCountList.FindIndex(x => x.lineEndKind == LineEndKind.LineFeed);
                _lineEndKindCountList[indexLineFeed] = (LineEndKind.LineFeed, linefeedCount);
            }
            {
                var indexCarriageReturnLineFeed = _lineEndKindCountList.FindIndex(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed);
                _lineEndKindCountList[indexCarriageReturnLineFeed] = (LineEndKind.CarriageReturnLineFeed, carriageReturnLinefeedCount);
            }
        }

        // Update the EndOfFile line end.
        {
            var endOfFile = _lineEndPositionList[^1];

            if (endOfFile.LineEndKind != LineEndKind.EndOfFile)
                throw new ApplicationException($"The text editor model is malformed; the final entry of {nameof(_lineEndPositionList)} must be the {nameof(LineEndKind)}.{nameof(LineEndKind.EndOfFile)}");

            endOfFile.StartPositionIndexInclusive = content.Length;
            endOfFile.EndPositionIndexExclusive = content.Length;
        }

        CheckRowEndingPositions(true);
    }

    public void ClearAllStatesButKeepEditHistory()
    {
        ClearContent();
        ClearOnlyRowEndingKind();
        SetUsingLineEndKind(LineEndKind.Unset);
    }

    public void HandleKeyboardEvent(
        KeyboardEventArgs keyboardEventArgs,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        if (KeyboardKeyFacts.IsMetaKey(keyboardEventArgs))
        {
            if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventArgs.Key ||
                KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventArgs.Key)
            {
                Delete(
                    keyboardEventArgs,
                    cursorModifierBag,
                    1,
                    cancellationToken);
            }
        }
        else
        {
            for (int i = cursorModifierBag.List.Count - 1; i >= 0; i--)
            {
                var cursor = cursorModifierBag.List[i];

                var singledCursorModifierBag = new CursorModifierBagTextEditor(
                    cursorModifierBag.ViewModelKey,
                    new List<TextEditorCursorModifier> { cursor });

                var valueToInsert = keyboardEventArgs.Key.First().ToString();

                if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
                    valueToInsert = UsingLineEndKind.AsCharacters();
                else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
                    valueToInsert = "\t";

                Insert(
                    valueToInsert,
                    singledCursorModifierBag,
                    cancellationToken);
            }
        }
    }

    /// <summary>
    /// Use <see cref="Insert(string, CursorModifierBagTextEditor, CancellationToken)"/> instead.
    /// This version is unsafe because it won't respect the user's cursor.
    /// </summary>
    public void Insert_Unsafe(
        string value,
        int rowIndex,
        int columnIndex,
        CancellationToken cancellationToken)
    {
        var cursor = new TextEditorCursor(rowIndex, columnIndex, true);
        var cursorModifier = new TextEditorCursorModifier(cursor);

        var cursorModifierBag = new CursorModifierBagTextEditor(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>() { cursorModifier });

        Insert(value, cursorModifierBag, cancellationToken);
    }

    public void Insert(
        string value,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        SetIsDirtyTrue();

        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndPositionList ??= _textEditorModel.LineEndPositionList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionsList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        EnsureUndoPoint(TextEditKind.Insertion);

        for (var i = cursorModifierBag.List.Count - 1; i >= 0; i--)
        {
            var cursorModifier = cursorModifierBag.List[i];

            if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
            {
                var (lowerPositionIndexInclusive, upperPositionIndexExclusive) = TextEditorSelectionHelper.GetSelectionBounds(cursorModifier);

                var lowerRowData = this.GetLineInformationFromPositionIndex(lowerPositionIndexInclusive);
                var lowerColumnIndex = lowerPositionIndexInclusive - lowerRowData.LineStartPositionIndexInclusive;

                Delete(
                    new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MetaKeys.DELETE,
                        Key = KeyboardKeyFacts.MetaKeys.DELETE,
                    },
                    cursorModifierBag,
                    1,
                    CancellationToken.None);

                // Move cursor to lower bound of text selection
                cursorModifier.LineIndex = lowerRowData.LineIndex;
                cursorModifier.SetColumnIndexAndPreferred(lowerColumnIndex);
            }

            // Validate the text to be inserted
            {
                // TODO: Do not convert '\r' and '\r\n' to '\n'. Instead take the string and insert it...
                //       ...as it was given.
                //       |
                //       The reason for converting to '\n' is, if one inserts a carriage return character,
                //       meanwhile the text editor model happens to have a line feed character at the position
                //       you are inserting (example: Insert("\r", ...)).
                //       |
                //       Then, the '\r' takes the position of the '\n', and the '\n' is shifted further
                //       by 1 position in order to allow space the '\r'.
                //       |
                //       Well, now the text editor model sees its contents as "\r\n".
                //       What is to be done in this scenario?
                //
                // The order of these replacements is important.
                value = value
                    .Replace("\r\n", "\n")
                    .Replace("\r", "\n");
            }

            // Remember the cursorPositionIndex
            var initialCursorPositionIndex = this.GetPositionIndex(cursorModifier);

            // Track metadata with the cursorModifier itself
            //
            // Metadata must be done prior to 'InsertValue'
            InsertMetadata(value, cursorModifier, cancellationToken);

            // Now the text still needs to be inserted.
            // The cursorModifier is invalid, because the metadata step moved its position.
            // So, use the 'cursorPositionIndex' variable that was calculated prior to the metadata step.
            InsertValue(value, initialCursorPositionIndex, cancellationToken);
        }
    }

    private void InsertMetadata(
        string value,
        TextEditorCursorModifier cursorModifier,
        CancellationToken cancellationToken)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndPositionList ??= _textEditorModel.LineEndPositionList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionsList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        var initialCursorPositionIndex = this.GetPositionIndex(cursorModifier);
        var initialCursorLineIndex = cursorModifier.LineIndex;

        // If cursor is out of bounds then continue
        if (initialCursorPositionIndex > _charList.Count)
            return;

        bool isTab = false;
        bool isCarriageReturn = false;
        bool isLinefeed = false;
        bool isCarriageReturnLineFeed = false;

        // The insertion is contiguous, therefore, the to-be-inserted metadata can be delt with after re-calculating the existing metadata.
        // That is, Insert("\n\n\n") is the same as Insert("\n Hello World!\n\n") as far as the metadata for LineEnd(s) is concerned.
        //
        // If there is a second LineEnd, they all can be added as 'AddRange' where the index starts at the first inserted line end.
        // If one performs a for loop from index 0 of the string to-be-inserted to the end of the string, then the line endings will already know their meta data.
        // Any remainder of the insertion string won't impact their metadata entry since it has a positionIndex greater than its own.
        // Then after the for loop, since one hasn't modified the public metadata, one can safely iterate over it and add 'characterCountInserted'.
        //
        // After that, one would then do the lazy insert range for the new metadata.
        (int? index, List<LineEnd> localLineEndList) lineEndPositionLazyInsertRange = (null, new());
        (int? index, List<int> localTabPositionList) tabPositionLazyInsertRange = (null, new());

        for (int characterIndex = 0; characterIndex < value.Length; characterIndex++)
        {
            var character = value[characterIndex];

            isTab = character == '\t';
            isCarriageReturn = character == '\r';
            isLinefeed = character == '\n';
            isCarriageReturnLineFeed = isCarriageReturn && characterIndex != value.Length - 1 && value[1 + characterIndex] == '\n';

            if (isCarriageReturn || isCarriageReturnLineFeed)
            {
                // TODO: Do not convert '\r' and '\r\n' to '\n'. Instead take the string and insert it...
                //       ...as it was given.
                //       |
                //       The reason for converting to '\n' is, if one inserts a carriage return character,
                //       meanwhile the text editor model happens to have a line feed character at the position
                //       you are inserting (example: Insert("\r", ...)).
                //       |
                //       Then, the '\r' takes the position of the '\n', and the '\n' is shifted further
                //       by 1 position in order to allow space the '\r'.
                //       |
                //       Well, now the text editor model sees its contents as "\r\n".
                //       What is to be done in this scenario?
                //
                // NOTE: This conversion is done in the 'Insert(...)' method,
                //       which goes on to invoke this method with the converted line endings.
                throw new NotImplementedException("TODO: Do not convert '\r' and '\r\n' to '\n'.Instead take the string and insert it as it was given.");
            }
            else if (isLinefeed)
            {
                var lineEndKindToInsert = LineEndKind.LineFeed;

                lineEndPositionLazyInsertRange.index ??= cursorModifier.LineIndex;

                lineEndPositionLazyInsertRange.localLineEndList.Add(new LineEnd(
                    initialCursorPositionIndex + characterIndex,
                    1 + initialCursorPositionIndex + characterIndex,
                    LineEndKind.LineFeed));

                MutateRowEndingKindCount(lineEndKindToInsert, 1);

                cursorModifier.LineIndex++;
                cursorModifier.SetColumnIndexAndPreferred(0);
            }
            else
            {
                if (isTab)
                {
                    if (tabPositionLazyInsertRange.index is null)
                    {
                        tabPositionLazyInsertRange.index = _tabKeyPositionsList.FindIndex(x => x >= initialCursorPositionIndex);

                        if (tabPositionLazyInsertRange.index == -1)
                            tabPositionLazyInsertRange.index = _tabKeyPositionsList.Count;
                    }

                    tabPositionLazyInsertRange.localTabPositionList.Add(initialCursorPositionIndex + characterIndex);
                }

                cursorModifier.SetColumnIndexAndPreferred(1 + cursorModifier.ColumnIndex);
            }
        }

        // Reposition the Row Endings
        {
            for (var i = initialCursorLineIndex; i < LineEndPositionList.Count; i++)
            {
                var rowEndingTuple = LineEndPositionList[i];
                rowEndingTuple.StartPositionIndexInclusive += value.Length;
                rowEndingTuple.EndPositionIndexExclusive += value.Length;
            }
        }

        // Reposition the Tabs
        {
            var firstTabKeyPositionIndexToModify = _tabKeyPositionsList.FindIndex(x => x >= initialCursorPositionIndex);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < TabKeyPositionsList.Count; i++)
                {
                    TabKeyPositionsList[i] += value.Length;
                }
            }
        }

        // Reposition the Diagnostic Squigglies
        {
            var textSpanForInsertion = new TextEditorTextSpan(
                initialCursorPositionIndex,
                initialCursorPositionIndex + value.Length,
                0,
                new(string.Empty),
                string.Empty);

            var textModification = new TextEditorTextModification(true, textSpanForInsertion);

            foreach (var presentationModel in PresentationModelsList)
            {
                presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
            }
        }

        // TODO: Fix tracking the MostCharactersOnASingleRowTuple this way is possibly inefficient - should instead only check the rows that changed
        {
            (int rowIndex, int rowLength) localMostCharactersOnASingleRowTuple = (0, 0);

            for (var i = 0; i < LineEndPositionList.Count; i++)
            {
                var lengthOfRow = this.GetLengthOfLine(i);

                if (lengthOfRow > localMostCharactersOnASingleRowTuple.rowLength)
                    localMostCharactersOnASingleRowTuple = (i, lengthOfRow);
            }

            localMostCharactersOnASingleRowTuple = (localMostCharactersOnASingleRowTuple.rowIndex,
                localMostCharactersOnASingleRowTuple.rowLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

            _mostCharactersOnASingleLineTuple = localMostCharactersOnASingleRowTuple;
        }

        // Add in any new metadata
        {
            if (lineEndPositionLazyInsertRange.index is not null)
            {
                _lineEndPositionList.InsertRange(
                    lineEndPositionLazyInsertRange.index.Value,
                    lineEndPositionLazyInsertRange.localLineEndList);
            }

            if (tabPositionLazyInsertRange.index is not null)
            {
                _tabKeyPositionsList.InsertRange(
                    tabPositionLazyInsertRange.index.Value,
                    tabPositionLazyInsertRange.localTabPositionList);
            }
        }
    }

    private void InsertValue(
        string value,
        int cursorPositionIndex,
        CancellationToken cancellationToken)
    {
        // If cursor is out of bounds then continue
        if (cursorPositionIndex > _charList.Count)
            return;

        __InsertRange(
            cursorPositionIndex,
            value.Select(character => new RichCharacter
            {
                Value = character,
                DecorationByte = 0,
            }));
    }

    /// <summary>
    /// Use <see cref="Delete(CursorModifierBagTextEditor, int, bool, CancellationToken, DeleteKind)"/> instead.
    /// This version is unsafe because it won't respect the user's cursor.
    /// </summary>
    public void Delete_Unsafe(
        int rowIndex,
        int columnIndex,
        int count,
        bool expandWord,
        CancellationToken cancellationToken,
        DeleteKind deleteKind = DeleteKind.Delete)
    {
        var cursor = new TextEditorCursor(rowIndex, columnIndex, true);
        var cursorModifier = new TextEditorCursorModifier(cursor);

        var cursorModifierBag = new CursorModifierBagTextEditor(
            Key<TextEditorViewModel>.Empty,
            new List<TextEditorCursorModifier>() { cursorModifier });

        Delete(cursorModifierBag, count, expandWord, cancellationToken, deleteKind);
    }

    /// <summary>
    /// The 'expandWord' parameter is applied after moving by the 'count' parameter.<br/>
    /// 
    /// Ex:
    ///     count of 1, and expandWord of true;
    ///     will move 1 char-value from the initialPositionIndex.
    ///     Afterwards, if expandWord is true, then the cursor is checked to be within a word, or at the start or end of one.
    ///     If the cursor is at the start or end of one, then the selection to delete is expanded such that it contains
    ///     the entire word that the cursor ended at.
    /// </summary>
    public void Delete(
        CursorModifierBagTextEditor cursorModifierBag,
        int count,
        bool expandWord,
        CancellationToken cancellationToken,
        DeleteKind deleteKind = DeleteKind.Delete)
    {
        SetIsDirtyTrue();

        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndPositionList ??= _textEditorModel.LineEndPositionList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionsList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        EnsureUndoPoint(TextEditKind.Deletion);

        for (var cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
        {
            var cursorModifier = cursorModifierBag.List[cursorIndex];

            // Remember the cursorPositionIndex
            var initialCursorPositionIndex = this.GetPositionIndex(cursorModifier);

            HackyStepMetadata(deleteKind, expandWord, cursorModifier);

            // Delete metadata with the cursorModifier itself
            //
            // Metadata must be done prior to 'DeleteValue'
            var charValueCount = DeleteMetadata(count, initialCursorPositionIndex, cursorModifier, cancellationToken);

            // Now the text still needs to be deleted.
            // The cursorModifier is invalid, because the metadata step moved its position.
            // So, use the 'cursorPositionIndex' variable that was calculated prior to the metadata step.
            DeleteValue(count, initialCursorPositionIndex, cancellationToken);
        }
    }

    /// <summary>
    /// The 'expandWord' parameter is applied after moving by the 'count' parameter.<br/>
    /// 
    /// Ex:
    ///     count of 1, and expandWord of true;
    ///     will move 1 char-value from the initialPositionIndex.
    ///     Afterwards, if expandWord is true, then the cursor is checked to be within a word, or at the start or end of one.
    ///     If the cursor is at the start or end of one, then the selection to delete is expanded such that it contains
    ///     the entire word that the cursor ended at.
    /// </summary>
    private void HackyStepMetadata(DeleteKind deleteKind, bool expandWord, TextEditorCursorModifier cursorModifier)
    {
        var initialCursorPositionIndex = this.GetPositionIndex(cursorModifier);

        // If cursor is out of bounds then continue
        if (initialCursorPositionIndex > _charList.Count)
            return;

        int startingPositionIndexToRemoveInclusive;
        int countToRemove;

        // Cannot calculate this after text was deleted - it would be wrong
        int? selectionUpperBoundRowIndex = null;
        // Needed for when text selection is deleted
        (int rowIndex, int columnIndex)? selectionLowerBoundIndexCoordinates = null;

        // TODO: The deletion logic should be the same whether it be 'Delete' 'Backspace' 'CtrlModified' or 'DeleteSelection'. What should change is one needs to calculate the starting and ending index appropriately foreach case.
        if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
        {
            var lowerPositionIndexInclusiveBound = cursorModifier.SelectionAnchorPositionIndex ?? 0;
            var upperPositionIndexExclusive = cursorModifier.SelectionEndingPositionIndex;

            if (lowerPositionIndexInclusiveBound > upperPositionIndexExclusive)
                (lowerPositionIndexInclusiveBound, upperPositionIndexExclusive) = (upperPositionIndexExclusive, lowerPositionIndexInclusiveBound);

            var lowerRowMetaData = this.GetLineInformationFromPositionIndex(lowerPositionIndexInclusiveBound);
            var upperRowMetaData = this.GetLineInformationFromPositionIndex(upperPositionIndexExclusive);

            // Value is needed when knowing what row ending positions to update after deletion is done
            selectionUpperBoundRowIndex = upperRowMetaData.LineIndex;

            // Value is needed when knowing where to position the cursor after deletion is done
            selectionLowerBoundIndexCoordinates = (lowerRowMetaData.LineIndex,
                lowerPositionIndexInclusiveBound - lowerRowMetaData.LineStartPositionIndexInclusive);

            startingPositionIndexToRemoveInclusive = upperPositionIndexExclusive - 1;
            countToRemove = upperPositionIndexExclusive - lowerPositionIndexInclusiveBound;

            cursorModifier.SelectionAnchorPositionIndex = null;
        }
        else if (DeleteKind.Backspace == deleteKind)
        {
            if (expandWord)
            {
                var columnIndexOfCharacterWithDifferingKind = this.GetColumnIndexOfCharacterWithDifferingKind(
                    cursorModifier.LineIndex,
                    cursorModifier.ColumnIndex,
                    true);

                columnIndexOfCharacterWithDifferingKind = columnIndexOfCharacterWithDifferingKind == -1
                    ? 0
                    : columnIndexOfCharacterWithDifferingKind;

                countToRemove = cursorModifier.ColumnIndex -
                    columnIndexOfCharacterWithDifferingKind;

                countToRemove = countToRemove == 0
                    ? 1
                    : countToRemove;
            }
            else
            {
                countToRemove = 1;
            }

            startingPositionIndexToRemoveInclusive = initialCursorPositionIndex - 1;
        }
        else if (DeleteKind.Delete == deleteKind)
        {
            if (expandWord)
            {
                var columnIndexOfCharacterWithDifferingKind = this.GetColumnIndexOfCharacterWithDifferingKind(
                    cursorModifier.LineIndex,
                    cursorModifier.ColumnIndex,
                    false);

                columnIndexOfCharacterWithDifferingKind = columnIndexOfCharacterWithDifferingKind == -1
                    ? this.GetLengthOfLine(cursorModifier.LineIndex)
                    : columnIndexOfCharacterWithDifferingKind;

                countToRemove = columnIndexOfCharacterWithDifferingKind -
                    cursorModifier.ColumnIndex;

                countToRemove = countToRemove == 0
                    ? 1
                    : countToRemove;
            }
            else
            {
                countToRemove = 1;
            }

            startingPositionIndexToRemoveInclusive = initialCursorPositionIndex;
        }
        else
        {
            throw new ApplicationException($"{nameof(HackyStepMetadata)} failed.");
        }

        var charactersRemovedCount = 0;
        var rowsRemovedCount = 0;

        var indexToRemove = startingPositionIndexToRemoveInclusive;
    }

    /// <summary>
    /// The text editor sees "\r\n" as 1 character, even though that is made up of 2 char values.
    /// The <see cref="TextEditorPartition"/> however, sees "\r\n" as 2 char values.<br/><br/>
    /// 
    /// This different means, to delete "\r\n" one tells the text editor to delete 1 character,
    /// where as one tells the <see cref="TextEditorPartition"/> to delete 2 char values.<br/><br/>
    /// 
    /// This method returns the 'int charValueCount', so that it can be used
    /// in the <see cref="DeleteValue(int, int, CancellationToken)"/> method.
    /// </summary>
    private int DeleteMetadata(
        int count,
        int initialCursorPositionIndex,
        TextEditorCursorModifier cursorModifier,
        CancellationToken cancellationToken)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndPositionList ??= _textEditorModel.LineEndPositionList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionsList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        // If cursor is out of bounds then continue
        if (initialCursorPositionIndex > _charList.Count)
            return 0;

        int charValueCount = count;

        // Cannot calculate this after text was deleted - it would be wrong
        int? selectionUpperBoundRowIndex = null;
        // Needed for when text selection is deleted
        (int rowIndex, int columnIndex)? selectionLowerBoundIndexCoordinates = null;

        (int? index, int count) lineEndPositionLazyRemoveRange = (null, 0);
        (int? index, int count) tabPositionLazyRemoveRange = (null, 0);

        for (int i = 0; i < charValueCount - 1; i++)
        {
            var deletePositionIndex = initialCursorPositionIndex + i;

            if (deletePositionIndex < 0 || deletePositionIndex > _charList.Count - 1)
                break;

            var charToDelete = _charList[deletePositionIndex];

            if (KeyboardKeyFacts.IsLineEndingCharacter(charToDelete))
            {
                // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndPositionList
                // is a starting index, and a count.
                var indexLineEnd = _lineEndPositionList.FindIndex(
                    x => x.EndPositionIndexExclusive == deletePositionIndex);

                var lineEnd = LineEndPositionList[indexLineEnd];

                lineEndPositionLazyRemoveRange.index ??= indexLineEnd;
                lineEndPositionLazyRemoveRange.count++;

                var lengthOfRowEnding = LineEndPositionList[indexLineEnd].LineEndKind.AsCharacters().Length;

                // Minus one here because each 'character' is presumed to be 1 'char value'.
                // At this step however, one might find a CarriageReturnNewLine "\r\n",
                // which in reality is 2 'char value'(s).
                //
                // So the 1 length that is already accounted for needs to be subtracted.
                var notAccountedForLength = lengthOfRowEnding - 1; ;
                charValueCount += notAccountedForLength;
                i += notAccountedForLength;

                MutateRowEndingKindCount(lineEnd.LineEndKind, -1);

                if (!selectionUpperBoundRowIndex.HasValue)
                {
                    cursorModifier.LineIndex--;
                    var startCurrentRowPositionIndex = this.GetLineEndPositionIndexExclusive(cursorModifier.LineIndex);
                    var endingPositionIndex = initialCursorPositionIndex - charValueCount;

                    cursorModifier.SetColumnIndexAndPreferred(endingPositionIndex - startCurrentRowPositionIndex);
                }
            }
            else
            {
                if (charToDelete == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                {
                    var indexTabKey = _tabKeyPositionsList.FindIndex(
                        x => x == deletePositionIndex);

                    tabPositionLazyRemoveRange.index = indexTabKey;
                    tabPositionLazyRemoveRange.count++;
                }
            }
        }

        int firstRowIndexToModify;

        if (selectionUpperBoundRowIndex.HasValue)
        {
            firstRowIndexToModify = selectionLowerBoundIndexCoordinates!.Value.rowIndex;
            cursorModifier.LineIndex = selectionLowerBoundIndexCoordinates!.Value.rowIndex;
            cursorModifier.SetColumnIndexAndPreferred(selectionLowerBoundIndexCoordinates!.Value.columnIndex);
        }
        else
        {
            firstRowIndexToModify = cursorModifier.LineIndex;
        }

        for (var i = firstRowIndexToModify; i < LineEndPositionList.Count; i++)
        {
            var rowEndingTuple = LineEndPositionList[i];
            rowEndingTuple.StartPositionIndexInclusive -= charValueCount;
            rowEndingTuple.EndPositionIndexExclusive -= charValueCount;
        }

        var firstTabKeyPositionIndexToModify = _tabKeyPositionsList.FindIndex(x => x >= initialCursorPositionIndex);

        if (firstTabKeyPositionIndexToModify != -1)
        {
            for (var i = firstTabKeyPositionIndexToModify; i < TabKeyPositionsList.Count; i++)
            {
                TabKeyPositionsList[i] -= charValueCount;
            }
        }

        // Reposition the Diagnostic Squigglies
        {
            var textSpanForInsertion = new TextEditorTextSpan(
                initialCursorPositionIndex,
                initialCursorPositionIndex + charValueCount,
                0,
                new(string.Empty),
                string.Empty);

            var textModification = new TextEditorTextModification(false, textSpanForInsertion);

            foreach (var presentationModel in PresentationModelsList)
            {
                presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
            }
        }

        // TODO: Fix tracking the MostCharactersOnASingleRowTuple this way is possibly inefficient - should instead only check the rows that changed
        {
            (int rowIndex, int rowLength) localMostCharactersOnASingleRowTuple = (0, 0);

            for (var i = 0; i < LineEndPositionList.Count; i++)
            {
                var lengthOfRow = this.GetLengthOfLine(i);

                if (lengthOfRow > localMostCharactersOnASingleRowTuple.rowLength)
                {
                    localMostCharactersOnASingleRowTuple = (i, lengthOfRow);
                }
            }

            localMostCharactersOnASingleRowTuple = (localMostCharactersOnASingleRowTuple.rowIndex,
                localMostCharactersOnASingleRowTuple.rowLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

            _mostCharactersOnASingleLineTuple = localMostCharactersOnASingleRowTuple;
        }

        // Delete metadata
        {
            if (lineEndPositionLazyRemoveRange.index is not null)
            {
                _lineEndPositionList.RemoveRange(
                    lineEndPositionLazyRemoveRange.index.Value,
                    lineEndPositionLazyRemoveRange.count);
            }

            if (tabPositionLazyRemoveRange.index is not null)
            {
                _tabKeyPositionsList.RemoveRange(
                    tabPositionLazyRemoveRange.index.Value,
                    tabPositionLazyRemoveRange.count);
            }
        }

        return charValueCount;
    }

    private void DeleteValue(int count, int initialCursorPositionIndex, CancellationToken cancellationToken)
    {
        // If cursor is out of bounds then continue
        if (initialCursorPositionIndex > _charList.Count)
            return;

        __RemoveRange(initialCursorPositionIndex, count);
    }

    public void DeleteTextByMotion(
        MotionKind motionKind,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        var keyboardEventArgs = motionKind switch
        {
            MotionKind.Backspace => new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.BACKSPACE },
            MotionKind.Delete => new KeyboardEventArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
            _ => throw new ApplicationException($"The {nameof(MotionKind)}: {motionKind} was not recognized.")
        };

        HandleKeyboardEvent(
            keyboardEventArgs,
            cursorModifierBag,
            CancellationToken.None);
    }

    public void DeleteByRange(
        int count,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        for (int cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
        {
            var cursor = cursorModifierBag.List[cursorIndex];

            var singledCursorModifierBag = new CursorModifierBagTextEditor(
                cursorModifierBag.ViewModelKey,
                new List<TextEditorCursorModifier> { cursor });

            // TODO: This needs to be rewritten everything should be deleted at the same time not a foreach loop for each character
            for (var deleteIndex = 0; deleteIndex < count; deleteIndex++)
            {
                HandleKeyboardEvent(
                    new KeyboardEventArgs
                    {
                        Code = KeyboardKeyFacts.MetaKeys.DELETE,
                        Key = KeyboardKeyFacts.MetaKeys.DELETE,
                    },
                    singledCursorModifierBag,
                    CancellationToken.None);
            }
        }
    }

    public void ClearEditBlocks()
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlocksList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

        _editBlockIndex = 0;
        EditBlockList.Clear();
    }

    /// <summary>
    /// The, "if (EditBlockIndex == _editBlocksPersisted.Count)", is done because the active EditBlock is not yet persisted.<br/><br/>
    /// The active EditBlock is instead being 'created' as the user continues to make edits of the same <see cref="TextEditKind"/>.<br/><br/>
    /// For complete clarity, this comment refers to one possibly expecting to see, "if (EditBlockIndex == _editBlocksPersisted.Count - 1)".
    /// </summary>
    public void UndoEdit()
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlocksList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

        if (!this.CanUndoEdit())
            return;

        if (EditBlockIndex == EditBlockList.Count)
        {
            // If the edit block is pending then persist it before
            // reverting back to the previous persisted edit block.
            EnsureUndoPoint(TextEditKind.ForcePersistEditBlock);
            _editBlockIndex--;
        }

        _editBlockIndex--;

        var restoreEditBlock = EditBlockList[EditBlockIndex];

        SetContent(restoreEditBlock.ContentSnapshot);
    }

    public void RedoEdit()
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlocksList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

        if (!this.CanRedoEdit())
            return;

        _editBlockIndex++;

        var restoreEditBlock = EditBlockList[EditBlockIndex];

        SetContent(restoreEditBlock.ContentSnapshot);
    }

    public void SetIsDirtyTrue()
    {
        _isDirty = true;
    }

    public void SetIsDirtyFalse()
    {
        _isDirty = false;
    }

    public void PerformRegisterPresentationModelAction(
    TextEditorPresentationModel presentationModel)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _presentationModelsList ??= _textEditorModel.PresentationModelList.ToList();
        }

        if (!PresentationModelsList.Any(x => x.TextEditorPresentationKey == presentationModel.TextEditorPresentationKey))
            PresentationModelsList.Add(presentationModel);
    }

    public void StartPendingCalculatePresentationModel(
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _presentationModelsList ??= _textEditorModel.PresentationModelList.ToList();
        }

        // If the presentation model has not yet been registered, then this will register it.
        PerformRegisterPresentationModelAction(emptyPresentationModel);

        var indexOfPresentationModel = _presentationModelsList.FindIndex(
            x => x.TextEditorPresentationKey == presentationKey);

        // The presentation model is expected to always be registered at this point.

        var presentationModel = PresentationModelsList[indexOfPresentationModel];
        PresentationModelsList[indexOfPresentationModel] = presentationModel with
        {
            PendingCalculation = new(this.GetAllText())
        };
    }

    public void CompletePendingCalculatePresentationModel(
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel,
        ImmutableArray<TextEditorTextSpan> calculatedTextSpans)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _presentationModelsList ??= _textEditorModel.PresentationModelList.ToList();
        }

        // If the presentation model has not yet been registered, then this will register it.
        PerformRegisterPresentationModelAction(emptyPresentationModel);

        var indexOfPresentationModel = _presentationModelsList.FindIndex(
            x => x.TextEditorPresentationKey == presentationKey);

        // The presentation model is expected to always be registered at this point.

        var presentationModel = PresentationModelsList[indexOfPresentationModel];

        if (presentationModel.PendingCalculation is null)
            return;

        var calculation = presentationModel.PendingCalculation with
        {
            TextSpanList = calculatedTextSpans
        };

        PresentationModelsList[indexOfPresentationModel] = presentationModel with
        {
            PendingCalculation = null,
            CompletedCalculation = calculation,
        };
    }

    public TextEditorModel ForceRerenderAction()
    {
        return ToModel();
    }

    private void MutateRowEndingKindCount(LineEndKind rowEndingKind, int changeBy)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndKindCountList ??= _textEditorModel.LineEndKindCountList.ToList();
        }

        var indexOfRowEndingKindCount = _lineEndKindCountList.FindIndex(x => x.lineEndKind == rowEndingKind);
        var currentRowEndingKindCount = LineEndKindCountsList[indexOfRowEndingKindCount].count;

        LineEndKindCountsList[indexOfRowEndingKindCount] = (rowEndingKind, currentRowEndingKindCount + changeBy);

        CheckRowEndingPositions(false);
    }

    private void CheckRowEndingPositions(bool setUsingRowEndingKind)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndKindCountList ??= _textEditorModel.LineEndKindCountList.ToList();
            _onlyLineEndKind ??= _textEditorModel.OnlyLineEndKind;
            _usingLineEndKind ??= _textEditorModel.UsingLineEndKind;
        }

        var existingRowEndingsList = LineEndKindCountsList
            .Where(x => x.count > 0)
            .ToArray();

        if (!existingRowEndingsList.Any())
        {
            _onlyLineEndKind = LineEndKind.Unset;
            _usingLineEndKind = LineEndKind.LineFeed;
        }
        else
        {
            if (existingRowEndingsList.Length == 1)
            {
                var rowEndingKind = existingRowEndingsList.Single().lineEndKind;

                if (setUsingRowEndingKind)
                    _usingLineEndKind = rowEndingKind;

                _onlyLineEndKind = rowEndingKind;
            }
            else
            {
                if (setUsingRowEndingKind)
                    _usingLineEndKind = existingRowEndingsList.MaxBy(x => x.count).lineEndKind;

                _onlyLineEndKind = null;
            }
        }
    }

    private void EnsureUndoPoint(TextEditKind textEditKind, string? otherTextEditKindIdentifier = null)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _editBlocksList ??= _textEditorModel.EditBlocksList.ToList();
            _editBlockIndex ??= _textEditorModel.EditBlockIndex;
        }

        if (textEditKind == TextEditKind.Other && otherTextEditKindIdentifier is null)
            TextEditorCommand.ThrowOtherTextEditKindIdentifierWasExpectedException(textEditKind);

        var mostRecentEditBlock = EditBlockList.LastOrDefault();

        if (mostRecentEditBlock is null || mostRecentEditBlock.TextEditKind != textEditKind)
        {
            var newEditBlockIndex = EditBlockIndex;

            EditBlockList.Insert(newEditBlockIndex, new EditBlock(
                textEditKind,
                textEditKind.ToString(),
                this.GetAllText(),
                otherTextEditKindIdentifier));

            var removeBlocksStartingAt = newEditBlockIndex + 1;

            _editBlocksList.RemoveRange(removeBlocksStartingAt, EditBlockList.Count - removeBlocksStartingAt);

            _editBlockIndex++;
        }

        while (EditBlockList.Count > TextEditorModel.MAXIMUM_EDIT_BLOCKS && EditBlockList.Count != 0)
        {
            _editBlockIndex--;
            EditBlockList.RemoveAt(0);
        }
    }

    public void __Insert(int globalPositionIndex, RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            TextEditorPartition? partition = _partitionList[i];

            if (runningCount + partition.Count >= globalPositionIndex)
            {
                // This is the partition we want to modify.
                // But, we must first check if it has available space.
                if (partition.Count >= PartitionSize)
                {
                    __SplitIntoTwoPartitions(i);
                    i--;
                    continue;
                }

                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new ApplicationException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new ApplicationException("if (relativePositionIndex == -1)");

        var inPartition = _partitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.Insert(relativePositionIndex, richCharacter.Value, richCharacter.DecorationByte);

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            outPartition);
    }

    /// <summary>
    /// If either character or decoration byte are 'null', then the respective
    /// collection will be left unchanged.
    /// 
    /// i.e.: to change ONLY a character value invoke this method with decorationByte set to null,
    ///       and only the <see cref="CharList"/> will be changed.
    /// </summary>
    public void __SetItem(
        int globalPositionIndex,
        char? character,
        byte? decorationByte)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            TextEditorPartition? partition = _partitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new ApplicationException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new ApplicationException("if (relativePositionIndex == -1)");

        var inPartition = _partitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.SetItem(relativePositionIndex, character, decorationByte);

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            outPartition);
    }

    /// <summary>
    /// To change ONLY a character value, or ONLY a decorationByte,
    /// one would need to use the overload: <see cref="__SetItem(int, char?, byte?)"/>.
    /// </summary>
    public void __SetItem(
        int globalPositionIndex,
        RichCharacter richCharacter)
    {
        __SetItem(globalPositionIndex, richCharacter.Value, richCharacter.DecorationByte);
    }

    public void __RemoveAt(int globalPositionIndex)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            TextEditorPartition? partition = _partitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithAvailableSpace = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithAvailableSpace == -1)
            throw new ApplicationException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new ApplicationException("if (relativePositionIndex == -1)");

        var inPartition = _partitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.RemoveAt(relativePositionIndex);

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithAvailableSpace,
            outPartition);
    }

    private void __InsertNewPartition(int partitionIndex)
    {
        _partitionList = _partitionList.Insert(partitionIndex, TextEditorPartition.Empty);
    }

    private void __SplitIntoTwoPartitions(int partitionIndex)
    {
        var originalPartition = _partitionList[partitionIndex];

        var firstUnevenSplit = PartitionSize / 2 + PartitionSize % 2;
        var secondUnevenSplit = PartitionSize / 2;

        // Replace old
        {

            var partition = TextEditorPartition.Empty.AddRange(
                originalPartition.GetRichCharacters(
                    skip: 0,
                    take: firstUnevenSplit));

            _partitionList = _partitionList.SetItem(
                partitionIndex,
                partition);
        }

        // Insert new
        {
            var partition = TextEditorPartition.Empty.AddRange(
                originalPartition.GetRichCharacters(
                    skip: firstUnevenSplit,
                    take: secondUnevenSplit));

            _partitionList = _partitionList.Insert(
                partitionIndex + 1,
                partition);
        }
    }

    public void __InsertRange(int globalPositionIndex, IEnumerable<RichCharacter> richCharacterList)
    {
        var richCharacterEnumerator = richCharacterList.GetEnumerator();

        while (richCharacterEnumerator.MoveNext())
        {
            int indexOfPartitionWithAvailableSpace = -1;
            int relativePositionIndex = -1;
            var runningCount = 0;
            TextEditorPartition? partition;

            for (int i = 0; i < _partitionList.Count; i++)
            {
                partition = _partitionList[i];

                if (runningCount + partition.Count >= globalPositionIndex)
                {
                    if (partition.Count >= PartitionSize)
                    {
                        __SplitIntoTwoPartitions(i);
                        i--;
                        continue;
                    }

                    relativePositionIndex = globalPositionIndex - runningCount;
                    indexOfPartitionWithAvailableSpace = i;
                    break;
                }
                else
                {
                    runningCount += partition.Count;
                }
            }

            if (indexOfPartitionWithAvailableSpace == -1)
                throw new ApplicationException("if (indexOfPartitionWithAvailableSpace == -1)");

            if (relativePositionIndex == -1)
                throw new ApplicationException("if (relativePositionIndex == -1)");

            partition = _partitionList[indexOfPartitionWithAvailableSpace];
            var partitionAvailableSpace = PartitionSize - partition.Count;

            var richCharacterBatchInsertList = new List<RichCharacter> { richCharacterEnumerator.Current };

            while (richCharacterBatchInsertList.Count < partitionAvailableSpace && richCharacterEnumerator.MoveNext())
            {
                richCharacterBatchInsertList.Add(richCharacterEnumerator.Current);
            }

            var inPartition = _partitionList[indexOfPartitionWithAvailableSpace];
            var outPartition = inPartition.InsertRange(relativePositionIndex, richCharacterBatchInsertList);

            _partitionList = _partitionList.SetItem(
                indexOfPartitionWithAvailableSpace,
                outPartition);

            globalPositionIndex += richCharacterBatchInsertList.Count;
        }
    }

    /// <summary>
    /// This method modifies the <see cref="TextEditorPartition"/>.
    /// The method only understands singular char values, 
    /// as opposed to the text editor which interprets "\r\n" as a single character,
    /// while encompassing 2 'char' values.<br/><br/>
    /// 
    /// One needs to be cautious with this method. The line ending: "\r\n"
    /// (or any other '2 char' long character),
    /// one can remove 1 of the two characters, and the other will still remain.<br/><br/>
    /// 
    /// If the text editor tells this method to remove "\r\n",
    /// then that is a count of 2 here. Even though for the text
    /// editor, it would describe "\r\n" as a count of 1.
    /// </summary>
    public void __RemoveRange(int globalPositionIndex, int count)
    {
        for (int i = 0; i < count; i++)
        {
            __RemoveAt(globalPositionIndex);
        }
    }

    public void __Add(RichCharacter richCharacter)
    {
        __Insert(DocumentLength, richCharacter);
    }

    public enum DeleteKind
    {
        Backspace,
        Delete,
    }
}