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
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value
///
/// When reading state, if the state had been 'null coallesce assigned' then the field will
/// be read. Otherwise, the existing TextEditorModel's value will be read.
/// <br/><br/>
/// <inheritdoc cref="ITextEditorModel"/>
/// </summary>
public partial class TextEditorModelModifier : ITextEditorModel
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

    public IList<EditBlock> EditBlockList => _editBlocksList is null ? _textEditorModel.EditBlockList : _editBlocksList;
    public IList<LineEnd> LineEndList => _lineEndList is null ? _textEditorModel.LineEndList : _lineEndList;
    public IList<(LineEndKind lineEndKind, int count)> LineEndKindCountList => _lineEndKindCountList is null ? _textEditorModel.LineEndKindCountList : _lineEndKindCountList;
    public IList<TextEditorPresentationModel> PresentationModelList => _presentationModelsList is null ? _textEditorModel.PresentationModelList : _presentationModelsList;
    public IList<int> TabKeyPositionList => _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionList : _tabKeyPositionsList;
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

    public int LineCount => LineEndList.Count;
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
    private List<LineEnd>? _lineEndList;
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
            _editBlocksList is null ? _textEditorModel.EditBlockList : _editBlocksList.ToImmutableList(),
            _lineEndList is null ? _textEditorModel.LineEndList : _lineEndList.ToImmutableList(),
            _lineEndKindCountList is null ? _textEditorModel.LineEndKindCountList : _lineEndKindCountList.ToImmutableList(),
            _presentationModelsList is null ? _textEditorModel.PresentationModelList : _presentationModelsList.ToImmutableList(),
            _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionList : _tabKeyPositionsList.ToImmutableList(),
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
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _lineEndKindCountList ??= _textEditorModel.LineEndKindCountList.ToList();
        }

        _mostCharactersOnASingleLineTuple = (0, TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

        _partitionList = new TextEditorPartition[] { TextEditorPartition.Empty }.ToImmutableList();

        _lineEndList = new List<LineEnd> 
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
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
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

                LineEndList.Insert(rowIndex, new(index, index + 1, LineEndKind.CarriageReturn));
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
                    var lineEnding = LineEndList[rowIndex - 1];
                    lineEnding.EndPositionIndexExclusive++;
                    lineEnding.LineEndKind = LineEndKind.CarriageReturnLineFeed;

                    carriageReturnCount--;
                    carriageReturnLinefeedCount++;
                }
                else
                {
                    LineEndList.Insert(rowIndex, new(index, index + 1, LineEndKind.LineFeed));
                    rowIndex++;

                    linefeedCount++;
                }

                charactersOnRow = 0;
            }

            if (character == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                TabKeyPositionList.Add(index);

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
            var endOfFile = _lineEndList[^1];

            if (endOfFile.LineEndKind != LineEndKind.EndOfFile)
                throw new ApplicationException($"The text editor model is malformed; the final entry of {nameof(_lineEndList)} must be the {nameof(LineEndKind)}.{nameof(LineEndKind.EndOfFile)}");

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
            if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventArgs.Key)
            {
                Delete(
                    cursorModifierBag,
                    1,
                    keyboardEventArgs.CtrlKey,
                    cancellationToken,
                    DeleteKind.Backspace);
            }
            else if (KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventArgs.Key)
            {
                Delete(
                    cursorModifierBag,
                    1,
                    keyboardEventArgs.CtrlKey,
                    cancellationToken,
                    DeleteKind.Delete);
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
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
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
                var lowerColumnIndex = lowerPositionIndexInclusive - lowerRowData.StartPositionIndexInclusive;

                Delete(
                    cursorModifierBag,
                    1,
                    false,
                    CancellationToken.None,
                    DeleteKind.Delete);

                // Move cursor to lower bound of text selection
                cursorModifier.LineIndex = lowerRowData.Index;
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
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
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

                MutateLineEndKindCount(lineEndKindToInsert, 1);

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
            for (var i = initialCursorLineIndex; i < LineEndList.Count; i++)
            {
                var rowEndingTuple = LineEndList[i];
                rowEndingTuple.StartPositionIndexInclusive += value.Length;
                rowEndingTuple.EndPositionIndexExclusive += value.Length;
            }
        }

        // Reposition the Tabs
        {
            var firstTabKeyPositionIndexToModify = _tabKeyPositionsList.FindIndex(x => x >= initialCursorPositionIndex);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < TabKeyPositionList.Count; i++)
                {
                    TabKeyPositionList[i] += value.Length;
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

            foreach (var presentationModel in PresentationModelList)
            {
                presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
            }
        }

        // TODO: Fix tracking the MostCharactersOnASingleRowTuple this way is possibly inefficient - should instead only check the rows that changed
        {
            (int rowIndex, int rowLength) localMostCharactersOnASingleRowTuple = (0, 0);

            for (var i = 0; i < LineEndList.Count; i++)
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
                _lineEndList.InsertRange(
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
    /// This method allows for a "RemoveRange" like operation on the text editor's contents.
    /// Any meta-data will automatically be updated (e.g. <see cref="ITextEditorModel.LineEndKindCountList"/>.
    /// </summary>
    /// <param name="cursorModifierBag">
    /// The list of cursors that indicate the positionIndex to start a "RemoveRange" operation.
    /// The cursors are iterated backwards, with each cursor being its own "RemoveRange" operation.
    /// </param>
    /// <param name="columnCount">
    /// The amount of columns to delete. If a the value of a column is of 2-char length (e.g. "\r\n"),
    /// then internally this columnCount will be converted to a 'charCount' of the corrected length.
    /// </param>
    /// <param name="expandWord">
    /// Applied after moving by the 'count' parameter.<br/>
    /// Ex:
    ///     count of 1, and expandWord of true;
    ///     will move 1 char-value from the initialPositionIndex.
    ///     Afterwards, if expandWord is true, then the cursor is checked to be within a word, or at the start or end of one.
    ///     If the cursor is at the start or end of one, then the selection to delete is expanded such that it contains
    ///     the entire word that the cursor ended at.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <param name="deleteKind">
    /// The default <see cref="DeleteKind.Delete"/> will do logic similar to a "RemoveRange" like
    /// operation on a collection type.<br/>
    /// If one has keyboard input from a user, one might want to have the user's backspace key
    /// pass in the <see cref="DeleteKind.Backspace"/> parameter.
    /// Then, for a user's delete key, pass in <see cref="DeleteKind.Delete"/>.
    /// </param>
    public void Delete(
        CursorModifierBagTextEditor cursorModifierBag,
        int columnCount,
        bool expandWord,
        CancellationToken cancellationToken,
        DeleteKind deleteKind = DeleteKind.Delete)
    {
        if (columnCount < 0)
            throw new ApplicationException($"{nameof(columnCount)} < 0");

        SetIsDirtyTrue();

        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        EnsureUndoPoint(TextEditKind.Deletion);

        for (var cursorIndex = cursorModifierBag.List.Count - 1; cursorIndex >= 0; cursorIndex--)
        {
            var cursorModifier = cursorModifierBag.List[cursorIndex];

            var tuple = DeleteMetadata(columnCount, cursorModifier, cancellationToken, deleteKind);

            if (tuple is null)
                return;

            var (positionIndex, charCount) = tuple.Value;
            DeleteValue(positionIndex, charCount, cancellationToken);
        }
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
    private (int positionIndex, int charCount)? DeleteMetadata(
        int columnCount,
        TextEditorCursorModifier cursorModifier,
        CancellationToken cancellationToken,
        DeleteKind deleteKind)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndList ??= _textEditorModel.LineEndList.ToList();
            _tabKeyPositionsList ??= _textEditorModel.TabKeyPositionList.ToList();
            _mostCharactersOnASingleLineTuple ??= _textEditorModel.MostCharactersOnASingleLineTuple;
        }

        var initialLineIndex = cursorModifier.LineIndex;
        var positionIndex = this.GetPositionIndex(cursorModifier);

        if (TextEditorSelectionHelper.HasSelectedText(cursorModifier) && cursorModifier.SelectionAnchorPositionIndex is not null)
        {
            // If user's cursor has a selection, then set the variables so the
            // positionIndex is the selection.AnchorPositionIndex and the
            // count is selection.EndPositionIndex - selection.AnchorPositionIndex.
            // and that the 'DeleteKind.Delete' logic runs.
            var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(cursorModifier.SelectionAnchorPositionIndex.Value);
            cursorModifier.LineIndex = lineIndex;
            initialLineIndex = cursorModifier.LineIndex;
            cursorModifier.SetColumnIndexAndPreferred(columnIndex);
            positionIndex = cursorModifier.SelectionAnchorPositionIndex.Value;
            columnCount = cursorModifier.SelectionEndingPositionIndex - cursorModifier.SelectionAnchorPositionIndex.Value;
            deleteKind = DeleteKind.Delete;
        }

        // If cursor is out of bounds then continue.
        // NOTE: '_charList.Count' is a valid position for the cursor.
        if (positionIndex > _charList.Count || positionIndex < 0)
            throw new ApplicationException($"{nameof(positionIndex)} > {nameof(_charList)}.{nameof(_charList.Count)} || {nameof(positionIndex)} < 0");

        (int? index, int count) lineEndPositionLazyRemoveRange = (null, 0);
        (int? index, int count) tabPositionLazyRemoveRange = (null, 0);

        var charCount = 0;

        if (deleteKind == DeleteKind.Delete)
        {
            for (int i = 0; i < columnCount; i++)
            {
                var toDeletePositionIndex = positionIndex + charCount;
                if (toDeletePositionIndex < 0 || toDeletePositionIndex >= DocumentLength)
                    break;

                var charToDelete = _charList[toDeletePositionIndex];

                if (KeyboardKeyFacts.IsLineEndingCharacter(charToDelete))
                {
                    // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndList
                    // is a starting index, and a count.
                    var indexLineEnd = _lineEndList.FindIndex(
                        x => x.StartPositionIndexInclusive == toDeletePositionIndex);

                    var lineEnd = LineEndList[indexLineEnd];

                    // Delete starts at the lowest index, therefore use '??=' to only assign once.
                    lineEndPositionLazyRemoveRange.index ??= indexLineEnd;
                    lineEndPositionLazyRemoveRange.count++;

                    var lengthOfLineEnd = LineEndList[indexLineEnd].LineEndKind.AsCharacters().Length;
                    charCount += lengthOfLineEnd;

                    MutateLineEndKindCount(lineEnd.LineEndKind, -1);
                }
                else
                {
                    charCount++;

                    if (charToDelete == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                    {
                        var indexTabKey = _tabKeyPositionsList.FindIndex(
                            x => x == toDeletePositionIndex);

                        // Delete starts at the lowest index, therefore use '??=' to only assign once.
                        tabPositionLazyRemoveRange.index ??= indexTabKey;
                        tabPositionLazyRemoveRange.count++;
                    }
                }
            }
        }
        else if (deleteKind == DeleteKind.Backspace)
        {
            for (int i = 0; i < columnCount; i++)
            {
                // Minus 1 here because 'Backspace' deletes the previous character.
                var toDeletePositionIndex = positionIndex - charCount - 1;
                if (toDeletePositionIndex < 0 || toDeletePositionIndex >= DocumentLength)
                    break;

                var charToDelete = _charList[toDeletePositionIndex];

                if (KeyboardKeyFacts.IsLineEndingCharacter(charToDelete))
                {
                    // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndList
                    // is a starting index, and a count.
                    //
                    // Here there is an 'or' expression.
                    // The left side checks for line endings of length '1' (i.e. "\n")
                    // The right side checks for line endings of length '2' (i.e. "\r\n")
                    var indexLineEnd = _lineEndList.FindIndex(
                        x => x.StartPositionIndexInclusive == toDeletePositionIndex ||
                             x.StartPositionIndexInclusive == toDeletePositionIndex - 1);

                    var lineEnd = LineEndList[indexLineEnd];

                    // Backspace starts at the highest index, therefore use '=' to only assign everytime.
                    lineEndPositionLazyRemoveRange.index = indexLineEnd;
                    lineEndPositionLazyRemoveRange.count++;

                    var lengthOfLineEnd = LineEndList[indexLineEnd].LineEndKind.AsCharacters().Length;
                    charCount += lengthOfLineEnd;

                    MutateLineEndKindCount(lineEnd.LineEndKind, -1);
                }
                else
                {
                    charCount++;

                    if (charToDelete == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                    {
                        var indexTabKey = _tabKeyPositionsList.FindIndex(
                            x => x == toDeletePositionIndex);

                        // Backspace starts at the highest index, therefore use '=' to only assign everytime.
                        tabPositionLazyRemoveRange.index = indexTabKey;
                        tabPositionLazyRemoveRange.count++;
                    }
                }
            }
        }
        else
        {
            throw new NotImplementedException();
        }

        // Reposition the LineEnd(s)
        {
            for (var i = initialLineIndex; i < LineEndList.Count; i++)
            {
                var lineEnd = LineEndList[i];
                lineEnd.StartPositionIndexInclusive -= charCount;
                lineEnd.EndPositionIndexExclusive -= charCount;
            }
        }

        // Reposition the Tab(s)
        {
            var firstTabKeyPositionIndexToModify = _tabKeyPositionsList.FindIndex(x => x >= positionIndex);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < TabKeyPositionList.Count; i++)
                {
                    TabKeyPositionList[i] -= charCount;
                }
            }
        }

        // Reposition the PresentationModel(s)
        {
            var textSpanForInsertion = new TextEditorTextSpan(
                positionIndex,
                positionIndex + charCount,
                0,
                new(string.Empty),
                string.Empty);

            var textModification = new TextEditorTextModification(false, textSpanForInsertion);

            foreach (var presentationModel in PresentationModelList)
            {
                presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
            }
        }

        // TODO: Fix tracking the MostCharactersOnASingleLineTuple this way is possibly inefficient - should instead only check the rows that changed
        {
            (int lineIndex, int lineLength) localMostCharactersOnASingleLineTuple = (0, 0);

            for (var i = 0; i < LineEndList.Count; i++)
            {
                var lengthOfLine = this.GetLengthOfLine(i);

                if (lengthOfLine > localMostCharactersOnASingleLineTuple.lineLength)
                {
                    localMostCharactersOnASingleLineTuple = (i, lengthOfLine);
                }
            }

            localMostCharactersOnASingleLineTuple = (
                localMostCharactersOnASingleLineTuple.lineIndex,
                localMostCharactersOnASingleLineTuple.lineLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

            _mostCharactersOnASingleLineTuple = localMostCharactersOnASingleLineTuple;
        }

        // Delete metadata
        {
            if (lineEndPositionLazyRemoveRange.index is not null)
            {
                _lineEndList.RemoveRange(
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

        if (deleteKind == DeleteKind.Delete)
        {
            // Reposition the cursor
            {
                var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(positionIndex);
                cursorModifier.LineIndex = lineIndex;
                cursorModifier.ColumnIndex = columnIndex;
            }

            return (positionIndex, charCount);
        }
        else if (deleteKind == DeleteKind.Backspace)
        {
            var calculatedPositionIndex = positionIndex - charCount;

            // Reposition the cursor
            {
                var(lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(calculatedPositionIndex);
                cursorModifier.LineIndex = lineIndex;
                cursorModifier.ColumnIndex = columnIndex;
            }

            return (calculatedPositionIndex, charCount);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void DeleteValue(int positionIndex, int count, CancellationToken cancellationToken)
    {
        // If cursor is out of bounds then continue
        if (positionIndex >= _charList.Count)
            return;

        __RemoveRange(positionIndex, count);
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
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
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
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
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
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
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

        if (!PresentationModelList.Any(x => x.TextEditorPresentationKey == presentationModel.TextEditorPresentationKey))
            PresentationModelList.Add(presentationModel);
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

        var presentationModel = PresentationModelList[indexOfPresentationModel];
        PresentationModelList[indexOfPresentationModel] = presentationModel with
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

        var presentationModel = PresentationModelList[indexOfPresentationModel];

        if (presentationModel.PendingCalculation is null)
            return;

        var calculation = presentationModel.PendingCalculation with
        {
            TextSpanList = calculatedTextSpans
        };

        PresentationModelList[indexOfPresentationModel] = presentationModel with
        {
            PendingCalculation = null,
            CompletedCalculation = calculation,
        };
    }

    public TextEditorModel ForceRerenderAction()
    {
        return ToModel();
    }

    private void MutateLineEndKindCount(LineEndKind rowEndingKind, int changeBy)
    {
        // Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value. When reading state, if the state had been 'null coallesce assigned' then the field will be read. Otherwise, the existing TextEditorModel's value will be read.
        {
            _lineEndKindCountList ??= _textEditorModel.LineEndKindCountList.ToList();
        }

        var indexOfRowEndingKindCount = _lineEndKindCountList.FindIndex(x => x.lineEndKind == rowEndingKind);
        var currentRowEndingKindCount = LineEndKindCountList[indexOfRowEndingKindCount].count;

        LineEndKindCountList[indexOfRowEndingKindCount] = (rowEndingKind, currentRowEndingKindCount + changeBy);

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

        var existingRowEndingsList = LineEndKindCountList
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
            _editBlocksList ??= _textEditorModel.EditBlockList.ToList();
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
        if (globalPositionIndex >= DocumentLength)
            return;

        int indexOfPartitionWithContent = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < _partitionList.Count; i++)
        {
            TextEditorPartition? partition = _partitionList[i];

            if (runningCount + partition.Count > globalPositionIndex)
            {
                // This is the partition we want to modify.
                relativePositionIndex = globalPositionIndex - runningCount;
                indexOfPartitionWithContent = i;
                break;
            }
            else
            {
                runningCount += partition.Count;
            }
        }

        if (indexOfPartitionWithContent == -1)
            throw new ApplicationException("if (indexOfPartitionWithContent == -1)");

        if (relativePositionIndex == -1)
            throw new ApplicationException("if (relativePositionIndex == -1)");

        var inPartition = _partitionList[indexOfPartitionWithContent];
        var outPartition = inPartition.RemoveAt(relativePositionIndex);

        _partitionList = _partitionList.SetItem(
            indexOfPartitionWithContent,
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