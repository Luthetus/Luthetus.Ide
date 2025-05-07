using System.Text;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lines.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Do not mutate state on this type unless you
/// have a TextEditorEditContext.
///
/// TODO: 2 interfaces, 1 mutable one readonly?
/// </summary>
public partial class TextEditorModel
{
	#region TextEditorModelMain
    
    /// <summary>new model</summary>
    public TextEditorModel(
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string fileExtension,
        string content,
        IDecorationMapper? decorationMapper,
        ICompilerService? compilerService,
        ITextEditorService textEditorService,
		int partitionSize = 4_096)
    {
    	if (partitionSize < MINIMUM_PARTITION_SIZE)
            throw new LuthetusTextEditorException($"{nameof(PartitionSize)} must be >= {MINIMUM_PARTITION_SIZE}");
    
    	__LocalLineEndList = textEditorService.__LocalLineEndList;
        __LocalTabPositionList = textEditorService.__LocalTabPositionList;
        __TextEditorViewModelLiason = textEditorService.__TextEditorViewModelLiason;
    
    	// Initialize
	    _partitionList = new List<TextEditorPartition> { new TextEditorPartition(new List<RichCharacter>()) };
	    _richCharacterList = Array.Empty<RichCharacter>();
	    EditBlockList = new();
	    ViewModelKeyList = new();
	    LineEndList = new();
	    PresentationModelList = new();
	    TabCharPositionIndexList = new();
	    OnlyLineEndKind = LineEndKind.Unset;
	    LineEndKindPreference = LineEndKind.Unset;
	    ResourceUri = resourceUri;
	    ResourceLastWriteTime = resourceLastWriteTime;
	    FileExtension = fileExtension;
	    DecorationMapper = decorationMapper ?? new TextEditorDecorationMapperDefault();
	    CompilerService = compilerService ?? new CompilerServiceDoNothing();
	    TextEditorSaveFileHelper = new();
	    EditBlockIndex = 0;
	    MostCharactersOnASingleLineTuple = (0, 0);
	    PreviousMostCharactersOnASingleLineTuple = (0, 0);
	    RenderStateSequence = 0;
	    PreviousLineCount = 0;
	    PartitionSize = partitionSize;
	    _allText = string.Empty;
	    _charCount = 0;
        // LineCount => LineEndList.Count;
        
		SetContent(content);
		IsDirty = false;
	}
	
	/// <summary>model -> modifier</summary>
	public TextEditorModel(TextEditorModel other)
    {
		// There are various lists.
		// Some are "safe" from causing an enumeration was modified
		// if copied between the model instances as they get edited
		// because the UI doesn't use them.
		
		__LocalLineEndList = other.__LocalLineEndList;
		__LocalTabPositionList = other.__LocalTabPositionList;
		__TextEditorViewModelLiason = other.__TextEditorViewModelLiason;

	    _partitionList = other.PartitionList;
	    _richCharacterList = other.RichCharacterList;
	    EditBlockList = other.EditBlockList;
	    ViewModelKeyList = other.ViewModelKeyList;
	    LineEndList = other.LineEndList;
	    PresentationModelList = other.PresentationModelList;
	    TabCharPositionIndexList = other.TabCharPositionIndexList;
        OnlyLineEndKind = other.OnlyLineEndKind;
	    LineEndKindPreference = other.LineEndKindPreference;
	    ResourceUri = other.ResourceUri;
	    ResourceLastWriteTime = other.ResourceLastWriteTime;
	    FileExtension = other.FileExtension;
	    DecorationMapper = other.DecorationMapper;
	    CompilerService = other.CompilerService;
	    TextEditorSaveFileHelper = other.TextEditorSaveFileHelper;
	    EditBlockIndex = other.EditBlockIndex;
	    IsDirty = other.IsDirty;
	    TagDoNotRemove = other.TagDoNotRemove;
	    MostCharactersOnASingleLineTuple = other.MostCharactersOnASingleLineTuple;
	    PreviousMostCharactersOnASingleLineTuple = other.MostCharactersOnASingleLineTuple;
	    {
	    	RenderStateSequence = other.RenderStateSequence == int.MaxValue - 1
	    		? 0
	    		: other.RenderStateSequence + 1;
	    }
	    PreviousLineCount = other.LineEndList.Count;
	    WasDirty = other.IsDirty;
        PartitionSize = other.PartitionSize;
	    _allText = other._allText;
	    _charCount = other._charCount;
	    
	    /*if (other.ShouldReloadVirtualizationResult)
	    {
	    	WriteEditBlockListToConsole();
	    }*/
    }
    
    /*private void WriteEditBlockListToConsole()
    {
    	Console.WriteLine($"Index:{EditBlockIndex}, Count:{EditBlockList.Count}, TagDoNotRemove:{(TagDoNotRemove is null ? "null" : TagDoNotRemove)} MAXIMUM_EDIT_BLOCKS:{MAXIMUM_EDIT_BLOCKS} ResourceUri:{ResourceUri.Value}");
    	
    	for (int i = 0; i < EditBlockList.Count; i++)
    	{
    		var entry = EditBlockList[i];
    	
    		Console.WriteLine($"\tIndex: {i}:");
    		Console.WriteLine($"\t\tEditKind:       {entry.EditKind}");
    		Console.WriteLine($"\t\tTag:            {entry.Tag}");
    		Console.WriteLine($"\t\tCursor:         {entry.BeforeCursor}");
    		Console.WriteLine($"\t\tBeforePositionIndex:  {entry.BeforePositionIndex}");
    		Console.WriteLine($"\t\tEditedTextBuilder: {entry.EditedTextBuilder}");
    	}
    }*/
	
	/// <summary>
	/// You have to check if the '_partitionListChanged'
	/// when finalizing an edit to a text editor
	/// in order to ensure there is no race condition
	/// where two threads try to read the RichCharacterList
	/// while this is false, and thus double the calculation.
	/// </summary>
	private bool _partitionListChanged;
    private bool _partitionListIsShallowCopy;
    public List<TextEditorPartition> _partitionList;
    public List<TextEditorPartition> PartitionList
    {
    	get
    	{
    		return _partitionList;
    	}
    	set
    	{
    		_partitionListChanged = true;
    		_partitionList = value;
    	}
    }
    
	public RichCharacter[] _richCharacterList;
    public RichCharacter[] RichCharacterList
    {
    	get
    	{
    		if (_partitionListChanged)
    		{
    			_partitionListChanged = false;
    			_allText = null;
    			_charCount = -1;
    			_richCharacterList = PartitionList.SelectMany(x => x.RichCharacterList).ToArray();
    		}
    		
    		return _richCharacterList;
    	}
    	set
    	{
    		_partitionListChanged = false;
    		_richCharacterList = value;
    	}
    }
    
    /// <summary>
    /// Do not solely use '_partitionListChanged'
    /// to lazily calculate the text.
    ///
    /// '_partitionListChanged' is only here
    /// incase during an edit you try to read your text.
    ///
    /// Preferably this would only get triggered by
    /// the UI reading the text after an edit, and '_allText'
    /// is null upon trying to read the value.
    ///
    /// Reason being that the ListRichCharacter> are needed
    /// to render the UI.
    ///
    /// Whereas a string representation of the text editor
    /// is not nearly as likely.
    ///
    /// Similarly, just because you edit the text editor model,
    /// that doesn't mean you will render the text editor view model.
    /// BUT that opens up what is believed to be a far more common case
    /// where two view models are rendering the same text and they both
    /// try to calculate the List<RichCharacter> instead of sharing the result.
    ///
    /// As such, List<RichCharacter> can probably be lazier but
    /// it isn't at the moment.
    /// </summary>
    private string? _allText;
    public string? AllText
    {
    	get
    	{
    		if (_partitionListChanged || _allText is null)
    		{
    			// TODO: Difference between 'new string(char[])' and a StringBuilder? Also can this be IEnumerable instead of array?
    			_allText = new string(RichCharacterList.Select(x => x.Value).ToArray());
    		}
    		
    		return _allText;
    	}
    }
    
    /// <inheritdoc cref="_allText"/>
    private int _charCount;
    public int CharCount
    {
    	get
    	{
    		if (_partitionListChanged || _charCount == -1)
    			_charCount = PartitionList.Sum(x => x.Count);
    		
    		return _charCount;
    	}
    }

    public List<TextEditorEdit> EditBlockList { get; set; }
    public List<Key<TextEditorViewModel>> ViewModelKeyList { get; set; }
    public List<LineEnd> LineEndList { get; set; }
    
    private bool _presentationModelListIsShallowCopy;
    public List<TextEditorPresentationModel> PresentationModelList { get; set; }
    
    public List<int> TabCharPositionIndexList { get; set; }
    
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    private List<LineEnd> __LocalLineEndList;
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    private List<int> __LocalTabPositionList;
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    private TextEditorViewModelLiason __TextEditorViewModelLiason;
    
    public LineEndKind OnlyLineEndKind { get; set; }
    public LineEndKind LineEndKindPreference { get; private set; }
    public ResourceUri ResourceUri { get; set; }
    public DateTime ResourceLastWriteTime { get; set; }
    public string FileExtension { get; set; }
    public IDecorationMapper DecorationMapper { get; set; }
    public ICompilerService CompilerService { get; set; }
    public SaveFileHelper TextEditorSaveFileHelper { get; set; }
    public int EditBlockIndex { get; set; }
    public bool IsDirty { get; set; }
    /// <summary>
    /// Used to allow edits of 'TextEditorEditKind.Other' to span
    /// a count of edits which is greater than the MAXIMUM_EDIT_BLOCKS.
    /// </summary>
    public string TagDoNotRemove { get; set; }
    public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple { get; set; }
    public (int lineIndex, int lineLength) PreviousMostCharactersOnASingleLineTuple { get; set; }
    public int RenderStateSequence { get; set; }

    public int PreviousLineCount { get; set; }
    
    /// <summary>
    /// Be sure to set this to true in order to have the override used.
    /// See 'UnsetOverrideLineEndKind'.
    /// To get this applied on the initial 'SetContent(...)' invocation, use object initialization syntax.
    /// </summary>
    public bool UseUnsetOverride { get; set; }
    /// <summary>
    /// Be sure to set 'UseUnsetOverride' to true in order to have the override used.
    /// To get this applied on the initial 'SetContent(...)' invocation, use object initialization syntax.
    /// </summary>
    public LineEndKind UnsetOverrideLineEndKind { get; set; } = LineEndKind.LineFeed;
    
    public int LineCount => LineEndList.Count;

    /// <summary>
    /// This property optimizes the dirty state tracking. If _wasDirty != _isDirty then track the state change.
    /// This involves writing to dependency injectable state, then triggering a re-render in the <see cref="Edits.Displays.DirtyResourceUriInteractiveIconDisplay"/>
    /// </summary>
    public bool WasDirty { get; }

    private int PartitionSize { get; }
	
	/// <summary>
	/// This property decides whether or not to re-calculate the virtualization result that gets displayed on the UI.
	/// </summary>
    public bool ShouldReloadVirtualizationResult { get; set; }

    public int DocumentLength => RichCharacterList.Length;
    
    public const int TAB_WIDTH = 4;
    public const int GUTTER_PADDING_LEFT_IN_PIXELS = 3;
    public const int GUTTER_PADDING_RIGHT_IN_PIXELS = 17;
    public const int MAXIMUM_EDIT_BLOCKS = 6;
    public const int MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN = 5;
    
    /// <summary>
    /// <see cref="__SplitIntoTwoPartitions(int)"/> will divide by 2 and give the first split the remainder,
    /// then add 1 to the first split if there is a multibyte scenario.
    /// Therefore partition size of 3 would infinitely try to split itself.
    /// </summary>
    public const int MINIMUM_PARTITION_SIZE = 4;

    public enum DeleteKind
    {
        Backspace,
        Delete,
    }
    #endregion

	#region TextEditorModelBad
	/// <summary>
	/// (2024-06-08) I belive there are too many ways to edit a text editor. There should only be 'Insert(...)' and 'Remove(...)' methods.
	///              Any code I currently have in 'TextEditorModelModifier.cs' that I deem as technical debt or a bad idea will be put in this file.
	///              Then, once organized I hope to make sense of what the "lean" solution is.
	/// </summary>
	
	public void ClearContent()
    {
        MostCharactersOnASingleLineTuple = (0, TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

        PartitionList = new List<TextEditorPartition> { new TextEditorPartition(new List<RichCharacter>()) };
        _partitionListChanged = true;

        LineEndList = new List<LineEnd> 
        {
            new LineEnd(0, 0, LineEndKind.EndOfFile)
        };

        TabCharPositionIndexList = new();

        SetIsDirtyTrue();
    }

	public void HandleKeyboardEvent(
        KeymapArgs keymapArgs,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        if (KeyboardKeyFacts.IsMetaKey(keymapArgs))
        {
            if (KeyboardKeyFacts.MetaKeys.BACKSPACE == keymapArgs.Key)
            {
                Delete(
                	cursorModifierBag,
                    1,
                    keymapArgs.CtrlKey,
                    DeleteKind.Backspace,
                    cancellationToken);
            }
            else if (KeyboardKeyFacts.MetaKeys.DELETE == keymapArgs.Key)
            {
                Delete(
                    cursorModifierBag,
                    1,
                    keymapArgs.CtrlKey,
                    DeleteKind.Delete,
                    cancellationToken);
            }
        }
        else
        {
            var valueToInsert = keymapArgs.Key.First().ToString();

            if (keymapArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
			{
                valueToInsert = LineEndKindPreference.AsCharacters();
				
				// GOAL: Match indentation on newline keystroke (2024-07-04)
				var line = this.GetLineInformation(cursorModifierBag.CursorModifier.LineIndex);

				var cursorPositionIndex = line.Position_StartInclusiveIndex + cursorModifierBag.CursorModifier.ColumnIndex;
				var indentationPositionIndex = line.Position_StartInclusiveIndex;

				var indentationBuilder = new StringBuilder();

				while (indentationPositionIndex < cursorPositionIndex)
				{
					var possibleIndentationChar = RichCharacterList[indentationPositionIndex++].Value;

					if (possibleIndentationChar == '\t' || possibleIndentationChar == ' ')
						indentationBuilder.Append(possibleIndentationChar);
					else
						break;
				}

				valueToInsert += indentationBuilder.ToString();
			}
            else if (keymapArgs.Code == KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
			{
                valueToInsert = "\t";
			}

            Insert(
                valueToInsert,
                cursorModifierBag,
                cancellationToken: cancellationToken);
        }
    }

	private void PerformInsert(CursorModifierBagTextEditor cursorModifierBag, int positionIndex, string content)
	{
		var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(positionIndex);
		
		cursorModifierBag.CursorModifier.LineIndex = lineIndex;
		cursorModifierBag.CursorModifier.SetColumnIndexAndPreferred(columnIndex);
		cursorModifierBag.CursorModifier.SelectionAnchorPositionIndex = -1;
		
		Insert(content, cursorModifierBag, CancellationToken.None, shouldCreateEditHistory: false);
	}

	private void PerformBackspace(CursorModifierBagTextEditor cursorModifierBag, int positionIndex, int count)
	{
		var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(positionIndex);
		
		cursorModifierBag.CursorModifier.LineIndex = lineIndex;
		cursorModifierBag.CursorModifier.SetColumnIndexAndPreferred(columnIndex);
		cursorModifierBag.CursorModifier.SelectionAnchorPositionIndex = -1;

		Delete(
			cursorModifierBag,
			count,
			false,
			DeleteKind.Backspace,
			CancellationToken.None,
			shouldCreateEditHistory: false,
			usePositionIndex: true);
	}

	private void PerformDelete(CursorModifierBagTextEditor cursorModifierBag, int positionIndex, int count)
	{
		var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(positionIndex);
		
		cursorModifierBag.CursorModifier.LineIndex = lineIndex;
		cursorModifierBag.CursorModifier.SetColumnIndexAndPreferred(columnIndex);
		cursorModifierBag.CursorModifier.SelectionAnchorPositionIndex = -1;

		Delete(
			cursorModifierBag,
			count,
			false,
			DeleteKind.Delete,
			CancellationToken.None,
			shouldCreateEditHistory: false,
			usePositionIndex: true);
	}

	public void DeleteTextByMotion(
        MotionKind motionKind,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        var keymapArgs = motionKind switch
        {
            MotionKind.Backspace => new KeymapArgs { Key = KeyboardKeyFacts.MetaKeys.BACKSPACE },
            MotionKind.Delete => new KeymapArgs { Key = KeyboardKeyFacts.MetaKeys.DELETE },
            _ => throw new LuthetusTextEditorException($"The {nameof(MotionKind)}: {motionKind} was not recognized.")
        };

        HandleKeyboardEvent(
            keymapArgs,
            cursorModifierBag,
            CancellationToken.None);
    }

	public void DeleteByRange(
        int count,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken)
    {
        // TODO: This needs to be rewritten everything should be deleted at the same time not a foreach loop for each character
        for (var deleteIndex = 0; deleteIndex < count; deleteIndex++)
        {
            HandleKeyboardEvent(
                new KeymapArgs
                {
                    Code = KeyboardKeyFacts.MetaKeys.DELETE,
                    Key = KeyboardKeyFacts.MetaKeys.DELETE,
                },
                cursorModifierBag,
                CancellationToken.None);
        }
    }

	/// <summary>
	/// Does this work with 'TextEditorComponentData.VirtualizedLineCacheSpanList'?
	/// (it seems to but this needs to be investigated further).
	/// </summary>
	public void SetContent(string content)
    {
        ClearAllStatesButKeepEditHistory();

		if (EditBlockList.Count == 0 && EditBlockIndex == 0)
		{
			EditBlockList.Add(new TextEditorEdit(
				TextEditorEditKind.Constructor,
				tag: string.Empty,
				0,
				TextEditorCursor.Empty,
				TextEditorCursor.Empty,
				editedTextBuilder: null));
		}
		
        var rowIndex = 0;
        var charactersOnLine = 0;

        List<RichCharacter> richCharacterList = new();
        var richCharacterIndex = 0;

        for (var contentIndex = 0; contentIndex < content.Length; contentIndex++)
        {
            var character = content[contentIndex];
            charactersOnLine++;

            LineEndKind currentLineEndKind = LineEndKind.Unset;

            if ((character == '\r') && (contentIndex < content.Length - 1) && (content[contentIndex + 1] == '\n'))
            {
            	contentIndex++;
            	currentLineEndKind = LineEndKind.CarriageReturnLineFeed;

            	if (LineEndKindPreference == LineEndKind.Unset)
        			LineEndKindPreference = currentLineEndKind; // Do not use 'SetLineEndKindPreference(...)' here.

            }
            else if (character == '\r')
            {
            	currentLineEndKind = LineEndKind.CarriageReturn;

            	if (LineEndKindPreference == LineEndKind.Unset)

        			LineEndKindPreference = currentLineEndKind; // Do not use 'SetLineEndKindPreference(...)' here.
            }
            else if (character == '\n')
            {
            	currentLineEndKind = LineEndKind.LineFeed;

            	if (LineEndKindPreference == LineEndKind.Unset)
        			LineEndKindPreference = currentLineEndKind; // Do not use 'SetLineEndKindPreference(...)' here.
            }

            if (currentLineEndKind != LineEndKind.Unset)
            {
				if (charactersOnLine > MostCharactersOnASingleLineTuple.lineLength - TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN)
					MostCharactersOnASingleLineTuple = (rowIndex, charactersOnLine + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

            	if (LineEndKindPreference == LineEndKind.CarriageReturnLineFeed)
            	{
            		LineEndList.Insert(rowIndex, new(richCharacterIndex, richCharacterIndex + 2, LineEndKind.CarriageReturnLineFeed));
	            	richCharacterList.Add(new(character, default));
	            	richCharacterList.Add(new('\n', default));
	            	richCharacterIndex += 2;
            	}
            	else if (LineEndKindPreference == LineEndKind.CarriageReturn)
            	{
            		LineEndList.Insert(rowIndex, new(richCharacterIndex, richCharacterIndex + 1, LineEndKind.CarriageReturn));
					richCharacterList.Add(new(character, default));
	            	richCharacterIndex++;
            	}
            	else if (LineEndKindPreference == LineEndKind.LineFeed)
            	{
            		LineEndList.Insert(rowIndex, new(richCharacterIndex, richCharacterIndex + 1, LineEndKind.LineFeed));
					richCharacterList.Add(new(character, default));
	            	richCharacterIndex++;
            	}
            	else
            	{
            		throw new NotImplementedException("only CarriageReturnLineFeed, CarriageReturn, and LineFeed are expected.");
            	}

				rowIndex++;
	            charactersOnLine = 0;
            }
			else if (character == KeyboardKeyFacts.WhitespaceCharacters.TAB)
			{
                TabCharPositionIndexList.Add(richCharacterIndex);
                richCharacterList.Add(new(character, default));
            	richCharacterIndex++;
            }
            else
            {
            	richCharacterList.Add(new(character, default));
            	richCharacterIndex++;
            }
        }
        
        if (LineEndKindPreference == LineEndKind.Unset)
        {
        	if (UseUnsetOverride)
        	{
        		LineEndKindPreference = UnsetOverrideLineEndKind;
        	}
        	else
        	{
	        	switch (Environment.NewLine)
	        	{
	        		case "\r":
	        			LineEndKindPreference = LineEndKind.CarriageReturn;
	        			break;
	        		case "\n":
	        			LineEndKindPreference = LineEndKind.LineFeed;
	        			break;
	        		case "\r\n":
	        			LineEndKindPreference = LineEndKind.CarriageReturnLineFeed;
	        			break;
	        		default:
	        			LineEndKindPreference = LineEndKind.LineFeed;
	        			break;
	        	}
	        }
        }

        __InsertRange(0, richCharacterList);

        // Update the EndOfFile line end.
        var endOfFile = LineEndList[^1];
        if (endOfFile.LineEndKind != LineEndKind.EndOfFile)
            throw new LuthetusTextEditorException($"The text editor model is malformed; the final entry of {nameof(LineEndList)} must be the {nameof(LineEndKind)}.{nameof(LineEndKind.EndOfFile)}");
        LineEndList[^1] = endOfFile with
		{
			Position_StartInclusiveIndex = richCharacterList.Count,
			Position_EndExclusiveIndex = richCharacterList.Count,
		};

        SetIsDirtyTrue();
		ShouldReloadVirtualizationResult = true;
    }

	public void ClearEditBlocks()
    {
        EditBlockIndex = 0;
        EditBlockList.Clear();
    }
    
	public void EnsureUndoPoint(TextEditorEdit newEdit)
	{
		// Clear redo history
		// TODO: Check how this interacts with an 'Other' edit group. (2025-04-20)
		if (EditBlockIndex < EditBlockList.Count - 1)
		{
			for (int i = EditBlockIndex + 1; i < EditBlockList.Count; i++)
			{
				EditBlockList.RemoveAt(i);
			}
		}

		var previousEdit = EditBlockList[EditBlockIndex];
		var shouldAddNewEdit = false;

		switch (newEdit.EditKind)
		{
			case TextEditorEditKind.Insert:
			{
				// Batch if consecutive && contiguous.
				if ((previousEdit.EditKind == TextEditorEditKind.Insert) &&
				    (newEdit.BeforePositionIndex == previousEdit.BeforePositionIndex + previousEdit.EditedTextBuilder.Length))
				{
					previousEdit.EditedTextBuilder!.Append(newEdit.EditedTextBuilder!.ToString());
					break;
				}
				
				shouldAddNewEdit = true;
				break;
			}
			case TextEditorEditKind.Backspace:
			{
				// Batch if consecutive && contiguous.
				if ((previousEdit.EditKind == TextEditorEditKind.Backspace) &&
				    (newEdit.BeforePositionIndex == previousEdit.BeforePositionIndex - previousEdit.EditedTextBuilder!.Length))
				{
					previousEdit.Add(newEdit.EditedTextBuilder!.ToString());
					break;
				}
				
				shouldAddNewEdit = true;
				break;
			}
			case TextEditorEditKind.Delete:
			{
				// Batch if consecutive && contiguous.
				if ((previousEdit.EditKind == TextEditorEditKind.Delete) &&
				    (newEdit.BeforePositionIndex == previousEdit.BeforePositionIndex))
				{
					previousEdit.Add(newEdit.EditedTextBuilder!.ToString());
					break;
				}
				
				shouldAddNewEdit = true;
				break;
			}
			case TextEditorEditKind.DeleteSelection:
			{
				shouldAddNewEdit = true;
				break;
			}
			case TextEditorEditKind.OtherOpen:
			{
				TagDoNotRemove = newEdit.Tag;
				shouldAddNewEdit = true;
				break;
			}
			case TextEditorEditKind.OtherClose:
			{
				// You cannot set 'TagDoNotRemove' to 'null' here.
				// The TagDoNotRemove logic relies on the set occurring
				// after the while loop that runs if 'EditBlockList.Count > MAXIMUM_EDIT_BLOCKS'.
				shouldAddNewEdit = true;
				break;
			}
			// 'TextEditorEditKind.Constructor' is expected to NOT invoke this method, but instead add to 'EditBlockList' on its own.
		}
		
		if (shouldAddNewEdit)
		{
			EditBlockList.Add(newEdit);
			EditBlockIndex++;
		}
		
		while (EditBlockList.Count > MAXIMUM_EDIT_BLOCKS)
	    {
	    	if ((EditBlockList[0].EditKind == TextEditorEditKind.OtherOpen || EditBlockList[0].EditKind == TextEditorEditKind.OtherClose) &&
	    	    EditBlockList[0].Tag == TagDoNotRemove)
	    	{
	    		break;
	    	}
	    	
	    	EditBlockIndex--;
	        EditBlockList.RemoveAt(0);
	    }
	    
	    if (newEdit.EditKind == TextEditorEditKind.OtherClose)
			TagDoNotRemove = null;
	}
	
	public void UndoEdit()
	{
		var cursorModifierBag = new CursorModifierBagTextEditor(
			Key<TextEditorViewModel>.Empty,
			new(TextEditorCursor.Empty));
			
		UndoEditWithCursor(cursorModifierBag);
	}

	public void UndoEditWithCursor(CursorModifierBagTextEditor cursorModifierBag)
	{
		if (EditBlockIndex <= 0)
			throw new LuthetusTextEditorException("No edits are available to perform 'undo' on");

		var mostRecentEdit = EditBlockList[EditBlockIndex];
		var undoEdit = mostRecentEdit.ToUndo();
		RestoreAfterCursor(cursorModifierBag, undoEdit);
		
		// In case the 'ToUndo(...)' throws an exception, the decrement to the EditIndex
		// is being done only after a successful ToUndo(...)
		EditBlockIndex--;

		switch (undoEdit.EditKind)
		{
			case TextEditorEditKind.Insert:
				PerformInsert(cursorModifierBag, undoEdit.BeforePositionIndex, undoEdit.EditedTextBuilder.ToString());
				RestoreBeforeCursor(cursorModifierBag, undoEdit);
				break;
			case TextEditorEditKind.Backspace:
				PerformBackspace(cursorModifierBag, undoEdit.BeforePositionIndex, undoEdit.EditedTextBuilder.Length);
				RestoreBeforeCursor(cursorModifierBag, undoEdit);
				break;
			case TextEditorEditKind.Delete: 
				PerformDelete(cursorModifierBag, undoEdit.BeforePositionIndex, undoEdit.EditedTextBuilder.Length);
				RestoreBeforeCursor(cursorModifierBag, undoEdit);
				break;
			case TextEditorEditKind.DeleteSelection: 
				PerformDelete(cursorModifierBag, undoEdit.BeforePositionIndex, undoEdit.EditedTextBuilder.Length);
				RestoreBeforeCursor(cursorModifierBag, undoEdit);
				break;
			case TextEditorEditKind.OtherOpen:
				break;
			case TextEditorEditKind.OtherClose:
				while (true)
				{
					if (EditBlockIndex <= 0)
						break;
				
					mostRecentEdit = EditBlockList[EditBlockIndex];

					if (mostRecentEdit.EditKind == TextEditorEditKind.OtherOpen)
					{
						if (mostRecentEdit.Tag == undoEdit.Tag)
							break;
					}
					else
					{
						UndoEditWithCursor(cursorModifierBag);
					}
				}
				break;
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {undoEdit.EditKind} was not recognized.");
		}
	}
	
	public void RedoEdit()
	{
		var cursorModifierBag = new CursorModifierBagTextEditor(
			Key<TextEditorViewModel>.Empty,
			new(TextEditorCursor.Empty));
	
		RedoEditWithCursor(cursorModifierBag);
	}

	public void RedoEditWithCursor(CursorModifierBagTextEditor cursorModifierBag, bool recursive = false)
	{
		// If there is no next then throw exception
		if (EditBlockIndex >= EditBlockList.Count - 1)
			throw new LuthetusTextEditorException("No edits are available to perform 'redo' on");

		TextEditorEdit redoEdit;

		if (recursive || EditBlockList[EditBlockIndex].EditKind != TextEditorEditKind.OtherOpen)
			EditBlockIndex++;
		
		redoEdit = EditBlockList[EditBlockIndex];
		RestoreBeforeCursor(cursorModifierBag, redoEdit);
		
		switch (redoEdit.EditKind)
		{
			case TextEditorEditKind.Insert:
				PerformInsert(cursorModifierBag, redoEdit.BeforePositionIndex, redoEdit.EditedTextBuilder.ToString());
				RestoreAfterCursor(cursorModifierBag, redoEdit);
				break;
			case TextEditorEditKind.Backspace:
				PerformBackspace(cursorModifierBag, redoEdit.BeforePositionIndex, redoEdit.EditedTextBuilder.Length);
				// Do not restore after cursor for 'Backspace'.
				break;
			case TextEditorEditKind.Delete: 
				PerformDelete(cursorModifierBag, redoEdit.BeforePositionIndex, redoEdit.EditedTextBuilder.Length);
				RestoreAfterCursor(cursorModifierBag, redoEdit);
				break;
			case TextEditorEditKind.DeleteSelection: 
				PerformDelete(cursorModifierBag, redoEdit.BeforePositionIndex, redoEdit.EditedTextBuilder.Length);
				RestoreAfterCursor(cursorModifierBag, redoEdit);
				break;
			case TextEditorEditKind.OtherOpen:
				while (true)
				{
					if (EditBlockIndex >= EditBlockList.Count - 1)
						break;

					var nextEdit = EditBlockList[EditBlockIndex + 1];

					if (nextEdit.EditKind == TextEditorEditKind.OtherOpen)
					{
						// Ignore nested 'OtherOpen'.
						EditBlockIndex++;
					}
					else if (nextEdit.EditKind == TextEditorEditKind.OtherClose)
					{
						EditBlockIndex++;
					
						if (nextEdit.Tag == redoEdit.Tag)
							break;
					}
					else
					{
						RedoEditWithCursor(cursorModifierBag, recursive: true);
					}
				}
				break;
			case TextEditorEditKind.OtherClose:
				break;
			default:
				throw new NotImplementedException($"The {nameof(TextEditorEditKind)}: {redoEdit.EditKind} was not recognized.");
		}
	}
	
	private void RestoreBeforeCursor(CursorModifierBagTextEditor cursorModifierBag, TextEditorEdit edit)
	{
		cursorModifierBag.CursorModifier.LineIndex = edit.BeforeCursor.LineIndex;
		cursorModifierBag.CursorModifier.SetColumnIndexAndPreferred(edit.BeforeCursor.ColumnIndex);
		
		cursorModifierBag.CursorModifier.SelectionAnchorPositionIndex = edit.BeforeCursor.Selection.AnchorPositionIndex;
		cursorModifierBag.CursorModifier.SelectionEndingPositionIndex = edit.BeforeCursor.Selection.EndingPositionIndex;
	}
	
	private void RestoreAfterCursor(CursorModifierBagTextEditor cursorModifierBag, TextEditorEdit edit)
	{
		cursorModifierBag.CursorModifier.LineIndex = edit.AfterCursor.LineIndex;
		cursorModifierBag.CursorModifier.SetColumnIndexAndPreferred(edit.AfterCursor.ColumnIndex);
		
		cursorModifierBag.CursorModifier.SelectionAnchorPositionIndex = edit.AfterCursor.Selection.AnchorPositionIndex;
		cursorModifierBag.CursorModifier.SelectionEndingPositionIndex = edit.AfterCursor.Selection.EndingPositionIndex;
	}
	#endregion
	
	#region TextEditorModelEditMethods
	/// <summary>
	/// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value
	///
	/// When reading state, if the state had been 'null coallesce assigned' then the field will
	/// be read. Otherwise, the existing TextEditorModel's value will be read.
	/// <br/><br/>
	/// <inheritdoc cref="TextEditorModel"/>
	/// </summary>

	public void ClearOnlyLineEndKind()
    {
        OnlyLineEndKind = LineEndKind.Unset;
    }

    public void SetLineEndKindPreference(LineEndKind lineEndKind)
    {
    	if (LineEndKindPreference == lineEndKind)
	    	return;
    	
        LineEndKindPreference = lineEndKind;

        if (lineEndKind == LineEndKind.CarriageReturnLineFeed ||
        	lineEndKind == LineEndKind.CarriageReturn ||
        	lineEndKind == LineEndKind.LineFeed)
        {
        	ClearEditBlocks();
        
        	UseUnsetOverride = true;
        	UnsetOverrideLineEndKind = lineEndKind;
        
        	SetContent(AllText.ReplaceLineEndings(lineEndKind.AsCharacters()));
        }
    }

    public void SetResourceData(ResourceUri resourceUri, DateTime resourceLastWriteTime)
    {
        ResourceUri = resourceUri;
        ResourceLastWriteTime = resourceLastWriteTime;
    }

    public void SetDecorationMapper(IDecorationMapper decorationMapper)
    {
        DecorationMapper = decorationMapper;
    }

    public void SetCompilerService(ICompilerService compilerService)
    {
        CompilerService = compilerService;
    }

    public void SetTextEditorSaveFileHelper(SaveFileHelper textEditorSaveFileHelper)
    {
        TextEditorSaveFileHelper = textEditorSaveFileHelper;
    }

    public void ClearAllStatesButKeepEditHistory()
    {
        ClearContent();
        ClearOnlyLineEndKind();
        SetLineEndKindPreference(LineEndKind.Unset);
    }     

    public void SetIsDirtyTrue()
    {
        // Setting _allText to null will clear the 'cache' for the all 'AllText' property.
        // ^ is true but what about _partitionListChanged?
        _allText = null;
        IsDirty = true;
    }

    public void SetIsDirtyFalse()
    {
        IsDirty = false;
    }

    public void PerformRegisterPresentationModelAction(
    	TextEditorPresentationModel presentationModel)
    {
    	if (!PresentationModelList.Any(x => x.TextEditorPresentationKey == presentationModel.TextEditorPresentationKey))
        {
        	if (!_presentationModelListIsShallowCopy)
	    	{
	        	_presentationModelListIsShallowCopy = true;
	        	PresentationModelList = new(PresentationModelList);
	        }
        
            PresentationModelList.Add(presentationModel);
        }
    }

    public void StartPendingCalculatePresentationModel(
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel)
    {
    	// If the presentation model has not yet been registered, then this will register it.
        PerformRegisterPresentationModelAction(emptyPresentationModel);

        var indexOfPresentationModel = PresentationModelList.FindIndex(
            x => x.TextEditorPresentationKey == presentationKey);

        // The presentation model is expected to always be registered at this point.

		if (!_presentationModelListIsShallowCopy)
    	{
        	_presentationModelListIsShallowCopy = true;
        	PresentationModelList = new(PresentationModelList);
        }
        
        var presentationModel = PresentationModelList[indexOfPresentationModel];
        PresentationModelList[indexOfPresentationModel] = presentationModel with
        {
            PendingCalculation = new(this.GetAllText())
        };
    }

    public void CompletePendingCalculatePresentationModel(
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel,
        List<TextEditorTextSpan> calculatedTextSpans)
    {
        // If the presentation model has not yet been registered, then this will register it.
        PerformRegisterPresentationModelAction(emptyPresentationModel);

        var indexOfPresentationModel = PresentationModelList.FindIndex(
            x => x.TextEditorPresentationKey == presentationKey);

        // The presentation model is expected to always be registered at this point.

        var presentationModel = PresentationModelList[indexOfPresentationModel];

        if (presentationModel.PendingCalculation is null)
            return;

		if (!_presentationModelListIsShallowCopy)
    	{
        	_presentationModelListIsShallowCopy = true;
        	PresentationModelList = new(PresentationModelList);
        }

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
    #endregion
    
    #region TextEditorModelInProgress
	/// <summary>
	/// (2024-06-08) I've been dogfooding the IDE, and the 'TextEditorModelModifier.cs' file demonstrates some clear issues regarding text editor optimization.
	///              Im breaking up the 80,000 character file a bit here into partial classes for now. TODO: merge the partial classes back?
	/// </summary>

	/// <param name="useLineEndKindPreference">
    /// If false, then the string will be inserted as is.
    /// If true, then the string will have its line endings replaced with the <see cref="LineEndKindPreference"/>
    /// </param>
    public void Insert(
        string value,
        CursorModifierBagTextEditor cursorModifierBag,
        CancellationToken cancellationToken = default,
		bool shouldCreateEditHistory = true)
    {
        var cursorModifier = cursorModifierBag.CursorModifier;
        var originalCursor = cursorModifier.ToCursor();

        if (TextEditorSelectionHelper.HasSelectedText(cursorModifier))
        {
            Delete(
				// TODO: 'cursorModifierBag' is not the correct parameter here...
				//       ...one needs to create a new cursor modifier bag which contains the single cursor that is being looked at. 
                cursorModifierBag,
                1,
                false,
                DeleteKind.Delete,
                CancellationToken.None);
        }

        // Remember the cursorPositionIndex
        var initialCursorPositionIndex = this.GetPositionIndex(cursorModifier);

        // Track metadata with the cursorModifier itself
        //
        // Metadata must be done prior to 'InsertValue'
        //
        // 'value' is replaced by the original with any line endings changed (based on 'useLineEndKindPreference').
        value = InsertMetadata(value, cursorModifier, cancellationToken);

        // Now the text still needs to be inserted.
        // The cursorModifier is invalid, because the metadata step moved its position.
        // So, use the 'cursorPositionIndex' variable that was calculated prior to the metadata step.
        InsertValue(value, initialCursorPositionIndex, cancellationToken);

		if (shouldCreateEditHistory)
		{
			EnsureUndoPoint(new TextEditorEdit(
				TextEditorEditKind.Insert,
				tag: string.Empty,
				initialCursorPositionIndex,
				originalCursor,
				TextEditorCursor.Empty,
				new StringBuilder(value)));
		}

        // NOTE: One cannot obtain the 'MostCharactersOnASingleLineTuple' from within the 'InsertMetadata(...)'
        //       method because this specific metadata is being calculated by counting the characters, which
        //       in the case of 'InsertMetadata(...)' wouldn't have been inserted yet.
        //
        // TODO: Fix tracking the MostCharactersOnASingleLineTuple this way is possibly inefficient - should instead only check the rows that changed
        {
            (int rowIndex, int rowLength) localMostCharactersOnASingleLineTuple = (0, 0);

            for (var i = 0; i < LineEndList.Count; i++)
            {
                var lengthOfLine = this.GetLineLength(i);

                if (lengthOfLine > localMostCharactersOnASingleLineTuple.rowLength)
                    localMostCharactersOnASingleLineTuple = (i, lengthOfLine);
            }

            localMostCharactersOnASingleLineTuple = (localMostCharactersOnASingleLineTuple.rowIndex,
                localMostCharactersOnASingleLineTuple.rowLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

            MostCharactersOnASingleLineTuple = localMostCharactersOnASingleLineTuple;
        }

        SetIsDirtyTrue();
		ShouldReloadVirtualizationResult = true;
    }

	private string InsertMetadata(
        string value,
        TextEditorCursorModifier cursorModifier,
        CancellationToken cancellationToken)
    {
        var initialCursorPositionIndex = this.GetPositionIndex(cursorModifier);
        var initialCursorLineIndex = cursorModifier.LineIndex;

        this.AssertPositionIndex(initialCursorPositionIndex);

        bool isTab = false;
        bool isCarriageReturn = false;
        bool isLineFeed = false;
        bool isCarriageReturnLineFeed = false;

		// Use 'int.MinValue' to represent null.
		__LocalLineEndList.Clear();
		__LocalTabPositionList.Clear();
        (int index, List<LineEnd> localLineEndList) lineEndPositionLazyInsertRange = (int.MinValue, __LocalLineEndList);
        (int index, List<int> localTabPositionList) tabPositionLazyInsertRange = (int.MinValue, __LocalTabPositionList);

        var lineEndingsChangedValueBuilder = new StringBuilder();

        for (int charIndex = 0; charIndex < value.Length; charIndex++)
        {
            var charValue = value[charIndex];

            isTab = charValue == '\t';
            isCarriageReturn = charValue == '\r';
            isLineFeed = charValue == '\n';
            // The CRLF boolean must be checked prior to CR, as one is a "substring" of the other
            isCarriageReturnLineFeed = isCarriageReturn && charIndex != value.Length - 1 && value[1 + charIndex] == '\n';

            if (isLineFeed || isCarriageReturn || isCarriageReturnLineFeed)
            {
                // Regardless of which line ending is used, since the source text
                // is CRLF, one must increment the for loop one character further.
                if (isCarriageReturnLineFeed)
                    charIndex++;

                LineEndKind lineEndKind;
                
                switch (LineEndKindPreference)
                {
                	case LineEndKind.CarriageReturn:
                		lineEndKind = LineEndKind.CarriageReturn;
                		break;
				    case LineEndKind.LineFeed:
				    	lineEndKind = LineEndKind.LineFeed;
                		break;
				    case LineEndKind.CarriageReturnLineFeed:
				    	lineEndKind = LineEndKind.CarriageReturnLineFeed;
                		break;
            		case LineEndKind.Unset:
				    case LineEndKind.EndOfFile:
				    case LineEndKind.StartOfFile:
				    	lineEndKind = LineEndKind.LineFeed;
                		break;
                	default:
                		throw new NotImplementedException($"The {nameof(LineEndKind)}: '{LineEndKindPreference}' was not recognized.");
                }
                
                // The LineEndKindPreference can invalidate the booleans
                //
                // Additionally, by clearing all the booleans and then setting only one of them,
                //
                //     -"CRLF must be checked prior to CR, as one is a "substring" of the other"
                //
                // can be avoided.
                {
                    isCarriageReturnLineFeed = false;
                    isCarriageReturn = false;
                    isLineFeed = false;

                    if (lineEndKind == LineEndKind.CarriageReturnLineFeed)
                        isCarriageReturnLineFeed = true;
                    else if (lineEndKind == LineEndKind.CarriageReturn)
                        isCarriageReturn = true;
                    else if (lineEndKind == LineEndKind.LineFeed)
                        isLineFeed = true;
                }

				if (lineEndPositionLazyInsertRange.index == int.MinValue)
                	lineEndPositionLazyInsertRange.index = cursorModifier.LineIndex;

                var lineEndCharacters = lineEndKind.AsCharacters();

                lineEndPositionLazyInsertRange.localLineEndList.Add(new LineEnd(
                    initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length,
                    lineEndCharacters.Length + initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length,
                    lineEndKind));

                lineEndingsChangedValueBuilder.Append(lineEndCharacters);

                // MutateLineEndKindCount(lineEndKind, 1);

                cursorModifier.LineIndex++;
                cursorModifier.SetColumnIndexAndPreferred(0);
            }
            else
            {
                if (isTab)
                {
                    if (tabPositionLazyInsertRange.index == int.MinValue)
                    {
                        tabPositionLazyInsertRange.index = TabCharPositionIndexList.FindIndex(x => x >= initialCursorPositionIndex);

                        if (tabPositionLazyInsertRange.index == -1)
                            tabPositionLazyInsertRange.index = TabCharPositionIndexList.Count;
                    }

                    tabPositionLazyInsertRange.localTabPositionList.Add(
                    	initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length);
                }

                lineEndingsChangedValueBuilder.Append(charValue);
                cursorModifier.SetColumnIndexAndPreferred(1 + cursorModifier.ColumnIndex);
            }
        }

        // Reposition the Line Endings
        {
            for (var i = initialCursorLineIndex; i < LineEndList.Count; i++)
            {
                var rowEndingTuple = LineEndList[i];

                LineEndList[i] = rowEndingTuple with
                {
                    Position_StartInclusiveIndex = rowEndingTuple.Position_StartInclusiveIndex + lineEndingsChangedValueBuilder.Length,
                    Position_EndExclusiveIndex = rowEndingTuple.Position_EndExclusiveIndex + lineEndingsChangedValueBuilder.Length,
                };
            }
        }

        // Reposition the Tabs
        {
            var firstTabKeyPositionIndexToModify = TabCharPositionIndexList.FindIndex(x => x >= initialCursorPositionIndex);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < TabCharPositionIndexList.Count; i++)
                {
                	var tabCharPositionIndex = TabCharPositionIndexList[i];
                    TabCharPositionIndexList[i] = tabCharPositionIndex + lineEndingsChangedValueBuilder.Length;
                }
            }
        }

        // Reposition the Diagnostic Squigglies
        {
            var textSpanForInsertion = new TextEditorTextSpan(
                initialCursorPositionIndex,
                initialCursorPositionIndex + lineEndingsChangedValueBuilder.Length,
                0,
                ResourceUri.Empty,
                string.Empty);

            var textModification = new TextEditorTextModification(true, textSpanForInsertion);

            foreach (var presentationModel in PresentationModelList)
            {
                presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
            }
        }
        
        // Reposition the ViewModel InlineUiList
        {
        	__TextEditorViewModelLiason.InsertRepositionInlineUiList(
        		initialCursorPositionIndex,
        		lineEndingsChangedValueBuilder.Length,
        		ViewModelKeyList,
        		initialCursorLineIndex,
        		lineEndPositionWasAdded: lineEndPositionLazyInsertRange.index != int.MinValue);
        }

        // Add in any new metadata
        {
            if (lineEndPositionLazyInsertRange.index != int.MinValue)
            {
                LineEndList.InsertRange(
                    lineEndPositionLazyInsertRange.index,
                    lineEndPositionLazyInsertRange.localLineEndList);
            }

            if (tabPositionLazyInsertRange.index != int.MinValue)
            {
                TabCharPositionIndexList.InsertRange(
                    tabPositionLazyInsertRange.index,
                    tabPositionLazyInsertRange.localTabPositionList);
            }
        }

        return lineEndingsChangedValueBuilder.ToString();
    }

	private void InsertValue(
        string value,
        int cursorPositionIndex,
        CancellationToken cancellationToken)
    {
        // If cursor is out of bounds then continue
        if (cursorPositionIndex > CharCount)
            return;

        __InsertRange(
            cursorPositionIndex,
            value.Select(character => new RichCharacter(character, 0)));
    }

	/// <summary>
    /// This method allows for a "RemoveRange" like operation on the text editor's contents.
    /// Any meta-data will automatically be updated (e.g. <see cref="TextEditorModel.LineEndKindCountList"/>.
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
        DeleteKind deleteKind,
        CancellationToken cancellationToken = default,
		bool shouldCreateEditHistory = true,
		bool usePositionIndex = false)
	{
        if (columnCount < 0)
            throw new LuthetusTextEditorException($"{nameof(columnCount)} < 0");

        var cursorModifier = cursorModifierBag.CursorModifier;
        var originalCursor = cursorModifier.ToCursor();

		var initialPositionIndex = this.GetPositionIndex(cursorModifier);
		var initiallyHadSelection = TextEditorSelectionHelper.HasSelectedText(cursorModifier);

        var tuple = DeleteMetadata(columnCount, cursorModifier, expandWord, deleteKind, usePositionIndex, cancellationToken);

        if (tuple is null)
        {
            SetIsDirtyTrue();
            return;
		}

        var (calculatedPositionIndex, charCount) = tuple.Value;

		var textRemoved = this.GetString(calculatedPositionIndex, charCount);

        DeleteValue(calculatedPositionIndex, charCount, cancellationToken);

		if (shouldCreateEditHistory)
		{
			var (lineIndex, columnIndex) = GetLineAndColumnIndicesFromPositionIndex(calculatedPositionIndex);
			var afterCursor = new TextEditorCursor(
				lineIndex,
				columnIndex,
				isPrimaryCursor: true);
			
			if (initiallyHadSelection)
			{
				EnsureUndoPoint(new TextEditorEdit(
					TextEditorEditKind.DeleteSelection,
					tag: string.Empty,
					calculatedPositionIndex,
					originalCursor,
					afterCursor,
					new StringBuilder(textRemoved)));
			}
			else if (deleteKind == DeleteKind.Delete)
			{
				// WARNING: If 'GetString(...)' ever returns an empty string erroneously then this if makes things very confusing...
				// ...the alternatives however involved getting the final position index and this might result in an extra .SelectMany on the partitions.
				// There is no proof for the extra .SelectMany, it is just a worry and I'm quite tired at the moment.
				if (textRemoved != string.Empty)
				{
					EnsureUndoPoint(new TextEditorEdit(
						TextEditorEditKind.Delete,
						tag: string.Empty,
						calculatedPositionIndex,
						originalCursor,
						afterCursor,
						new StringBuilder(textRemoved)));
				}
			}
			else if (deleteKind == DeleteKind.Backspace)
			{
				// WARNING: If 'GetString(...)' ever returns an empty string erroneously then this if makes things very confusing...
				// ...the alternatives however involved getting the final position index and this might result in an extra .SelectMany on the partitions.
				// There is no proof for the extra .SelectMany, it is just a worry and I'm quite tired at the moment.
				if (textRemoved != string.Empty)
				{
					EnsureUndoPoint(new TextEditorEdit(
						TextEditorEditKind.Backspace,
						tag: string.Empty,
						initialPositionIndex, // NOTE: this is different
						originalCursor,
						afterCursor,
						new StringBuilder(textRemoved)));
				}
			}
			else
			{
				throw new NotImplementedException($"The {nameof(DeleteKind)}: {deleteKind} was not recognized.");
			}
		}

        // NOTE: One cannot obtain the 'MostCharactersOnASingleLineTuple' from within the 'DeleteMetadata(...)'
        //       method because this specific metadata is being calculated by counting the characters, which
        //       in the case of 'DeleteMetadata(...)' wouldn't have been deleted yet.
        //
        // TODO: Fix tracking the MostCharactersOnASingleLineTuple this way is possibly inefficient - should instead only check the rows that changed
        {
            (int lineIndex, int lineLength) localMostCharactersOnASingleLineTuple = (0, 0);

            for (var i = 0; i < LineEndList.Count; i++)
            {
                var lengthOfLine = this.GetLineLength(i);

                if (lengthOfLine > localMostCharactersOnASingleLineTuple.lineLength)
                {
                    localMostCharactersOnASingleLineTuple = (i, lengthOfLine);
                }
			
            }

            localMostCharactersOnASingleLineTuple = (
                localMostCharactersOnASingleLineTuple.lineIndex,
                localMostCharactersOnASingleLineTuple.lineLength + TextEditorModel.MOST_CHARACTERS_ON_A_SINGLE_ROW_MARGIN);

            MostCharactersOnASingleLineTuple = localMostCharactersOnASingleLineTuple;
        }

        SetIsDirtyTrue();
		ShouldReloadVirtualizationResult = true;
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
        bool expandWord,
        DeleteKind deleteKind,
        bool usePositionIndex,
        CancellationToken cancellationToken)
	{
        var initiallyHadSelection = TextEditorSelectionHelper.HasSelectedText(cursorModifier);
        var initialLineIndex = cursorModifier.LineIndex;
        var positionIndex = this.GetPositionIndex(cursorModifier);

        if (initiallyHadSelection && cursorModifier.SelectionAnchorPositionIndex != -1)
        {
            // If user's cursor has a selection, then set the variables so the positionIndex is the
            // selection.AnchorPositionIndex and the count is selection.EndPositionIndex - selection.AnchorPositionIndex
            // and that the 'DeleteKind.Delete' logic runs.
            var (position_LowerIndexInclusive, position_UpperIndexExclusive) = TextEditorSelectionHelper.GetSelectionBounds(cursorModifier);

            var lowerLineData = this.GetLineInformationFromPositionIndex(position_LowerIndexInclusive);
            var lowerColumnIndex = position_LowerIndexInclusive - lowerLineData.Position_StartInclusiveIndex;

            cursorModifier.LineIndex = lowerLineData.Index;
            initialLineIndex = cursorModifier.LineIndex;
            cursorModifier.SetColumnIndexAndPreferred(lowerColumnIndex);
            positionIndex = position_LowerIndexInclusive;

            // The deletion of a selection logic does not check for multibyte characters.
            // Therefore, later in this method, if a multibyte character is found, the columnCount must be reduced. (2024-05-01)
            columnCount = position_UpperIndexExclusive - position_LowerIndexInclusive;
            deleteKind = DeleteKind.Delete;

            cursorModifier.SelectionAnchorPositionIndex = -1;
            cursorModifier.SelectionEndingPositionIndex = 0;
		}

        this.AssertPositionIndex(positionIndex);

        (int? index, int count) lineEndPositionLazyRemoveRange = (null, 0);
        (int? index, int count) tabPositionLazyRemoveRange = (null, 0);

        var charCount = 0;

        if (deleteKind == DeleteKind.Delete)
        {
            if (expandWord && !initiallyHadSelection)
            {
                var columnIndexOfCharacterWithDifferingKind = this.GetColumnIndexOfCharacterWithDifferingKind(
                    cursorModifier.LineIndex,
                    cursorModifier.ColumnIndex,
                    false);

                // -1 implies that no differing kind was found on the current line.
                if (columnIndexOfCharacterWithDifferingKind == -1)
                {
                    var line = this.GetLineInformation(cursorModifier.LineIndex);
                    columnIndexOfCharacterWithDifferingKind = line.LastValidColumnIndex;
                }

                columnCount = columnIndexOfCharacterWithDifferingKind - cursorModifier.ColumnIndex;

                // Cursor is at the start of a row
                if (columnCount == 0)
                    columnCount = 1;
			}

            for (int i = 0; i < columnCount; i++)
            {
                var toDeletePositionIndex = positionIndex + charCount;
                if (toDeletePositionIndex < 0 || toDeletePositionIndex >= CharCount)
                    break;

                var richCharacterToDelete = RichCharacterList[toDeletePositionIndex];

                if (KeyboardKeyFacts.IsLineEndingCharacter(richCharacterToDelete.Value))
                {
                    // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndList
                    // is a starting index, and a count.
                    var indexLineEnd = LineEndList.FindIndex(
                        x => x.Position_StartInclusiveIndex == toDeletePositionIndex);

                    var lineEnd = LineEndList[indexLineEnd];

                    // Delete starts at the lowest index, therefore use '??=' to only assign once.
                    lineEndPositionLazyRemoveRange.index ??= indexLineEnd;
                    lineEndPositionLazyRemoveRange.count++;

					var lengthOfLineEnd = LineEndList[indexLineEnd].LineEndKind.AsCharacters().Length;
                    charCount += lengthOfLineEnd;
	                    
					if (usePositionIndex)
					{
						// -1 since the for loop always will increment at least once.
						i += (lengthOfLineEnd - 1);
					}

                    // MutateLineEndKindCount(lineEnd.LineEndKind, -1);

                    if (lineEnd.LineEndKind == LineEndKind.CarriageReturnLineFeed && initiallyHadSelection)
                    {
                        // The deletion of a selection logic does not check for multibyte characters.
                        // Therefore, if a multibyte character is found, the columnCount must be reduced. (2024-05-01)
                        columnCount--;
                    }
                }
				else
                {
                    charCount++;

                    if (richCharacterToDelete.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                    {
                        var indexTabKey = TabCharPositionIndexList.FindIndex(x => x == toDeletePositionIndex);

                        // Delete starts at the lowest index, therefore use '??=' to only assign once.
                        tabPositionLazyRemoveRange.index ??= indexTabKey;
                        tabPositionLazyRemoveRange.count++;
                    }
                }
            }
        }
		else if (deleteKind == DeleteKind.Backspace)
        {
            if (expandWord && !initiallyHadSelection)
            {
                var columnIndexOfCharacterWithDifferingKind = this.GetColumnIndexOfCharacterWithDifferingKind(
                    cursorModifier.LineIndex,
                    cursorModifier.ColumnIndex,
                    true);

                // -1 implies that no differing kind was found on the current line.
                if (columnIndexOfCharacterWithDifferingKind == -1)
                    columnIndexOfCharacterWithDifferingKind = 0;

                columnCount = cursorModifier.ColumnIndex - columnIndexOfCharacterWithDifferingKind;

                // Cursor is at the start of a row
                if (columnCount == 0)
                    columnCount = 1;
			}

            for (int i = 0; i < columnCount; i++)
            {
                // Minus 1 here because 'Backspace' deletes the previous character.
                var toDeletePositionIndex = positionIndex - charCount - 1;
                if (toDeletePositionIndex < 0 || toDeletePositionIndex >= CharCount)
                    break;

                var richCharacterToDelete = RichCharacterList[toDeletePositionIndex];

                if (KeyboardKeyFacts.IsLineEndingCharacter(richCharacterToDelete.Value))
                {
                    // A delete is a contiguous operation. Therefore, all that is needed to update the LineEndList
                    // is a starting index, and a count.
                    var indexLineEnd = LineEndList.FindIndex(
                        // Check for '\n' or '\r'
                        x => x.Position_EndExclusiveIndex == toDeletePositionIndex + 1 ||
                        // Check for "\r\n"
                        x.Position_EndExclusiveIndex == toDeletePositionIndex + 2);

                    var lineEnd = LineEndList[indexLineEnd];

                    // Backspace starts at the highest index, therefore use '=' to only assign everytime.
                    lineEndPositionLazyRemoveRange.index = indexLineEnd;
                    lineEndPositionLazyRemoveRange.count++;

					var lengthOfLineEnd = LineEndList[indexLineEnd].LineEndKind.AsCharacters().Length;
					charCount += lengthOfLineEnd;
					
					if (usePositionIndex)
					{
						// -1 since the for loop always will increment at least once.
						i += (lengthOfLineEnd - 1);
					}

                    // MutateLineEndKindCount(lineEnd.LineEndKind, -1);
                }
                else
                {
                    charCount++;

                    if (richCharacterToDelete.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                    {
                        var indexTabKey = TabCharPositionIndexList.FindIndex(x => x == toDeletePositionIndex);

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

                LineEndList[i] = lineEnd with
                {
                    Position_StartInclusiveIndex = lineEnd.Position_StartInclusiveIndex - charCount,
                    Position_EndExclusiveIndex = lineEnd.Position_EndExclusiveIndex - charCount,
                };
            }
        }

        // Reposition the Tab(s)
        {
            var firstTabKeyPositionIndexToModify = TabCharPositionIndexList.FindIndex(x => x >= positionIndex);

            if (firstTabKeyPositionIndexToModify != -1)
            {
                for (var i = firstTabKeyPositionIndexToModify; i < TabCharPositionIndexList.Count; i++)
                {
                	var tabCharPositionIndex = TabCharPositionIndexList[i];
                    TabCharPositionIndexList[i] = tabCharPositionIndex - charCount;
                }
            }
        }

		// Reposition the PresentationModel(s)
        {
            var textSpanForInsertion = new TextEditorTextSpan(
                positionIndex,
                positionIndex + charCount,
                0,
                ResourceUri.Empty,
                string.Empty);

            var textModification = new TextEditorTextModification(false, textSpanForInsertion);

            foreach (var presentationModel in PresentationModelList)
            {
                presentationModel.CompletedCalculation?.TextModificationsSinceRequestList.Add(textModification);
                presentationModel.PendingCalculation?.TextModificationsSinceRequestList.Add(textModification);
            }
        }
        
        // Reposition the ViewModel InlineUiList
        {
        	__TextEditorViewModelLiason.DeleteRepositionInlineUiList(
        		positionIndex,
        		positionIndex + charCount,
        		ViewModelKeyList,
        		initialLineIndex,
        		lineEndPositionLazyRemoveRange.index is not null);
        }

		// Delete metadata
        {
            if (lineEndPositionLazyRemoveRange.index is not null)
            {
                LineEndList.RemoveRange(
                    lineEndPositionLazyRemoveRange.index.Value,
                    lineEndPositionLazyRemoveRange.count);
            }

            if (tabPositionLazyRemoveRange.index is not null)
            {
                TabCharPositionIndexList.RemoveRange(
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
                cursorModifier.SetColumnIndexAndPreferred(columnIndex);
            }

            return (positionIndex, charCount);
        }
        else if (deleteKind == DeleteKind.Backspace)
        {
            var calculatedPositionIndex = positionIndex - charCount;

            // Reposition the cursor
            {
                var (lineIndex, columnIndex) = this.GetLineAndColumnIndicesFromPositionIndex(calculatedPositionIndex);
                
                cursorModifier.LineIndex = lineIndex;
                cursorModifier.SetColumnIndexAndPreferred(columnIndex);
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
        if (positionIndex >= CharCount)
            return;

        __RemoveRange(positionIndex, count);
	}
	#endregion
	
	#region TextEditorModelMethods
    /// <summary>
	/// Returns the Length of a line however it does not include the line ending characters by default.
	/// To include line ending characters the parameter <see cref="includeLineEndingCharacters" /> must be true.
	/// </summary>
    public int GetLineLength(int lineIndex, bool includeLineEndingCharacters = false)
    {
        if (!LineEndList.Any())
            return 0;

        if (lineIndex > LineEndList.Count - 1)
            lineIndex = LineEndList.Count - 1;

        if (lineIndex < 0)
            lineIndex = 0;

        var line = GetLineInformation(lineIndex);
        var lineLengthWithLineEndings = line.Position_EndExclusiveIndex - line.Position_StartInclusiveIndex;

        if (includeLineEndingCharacters)
            return lineLengthWithLineEndings;

        return lineLengthWithLineEndings - line.UpperLineEnd.LineEndKind.AsCharacters().Length;
    }

    /// <summary>
	/// Line endings are included in the individual lines which get returned.
	/// </summary>
	/// <param name="startingLineIndex">The starting index of the lines to return</param>
    /// <param name="count">
    /// A count of 0 returns 0 rows.<br/>
    /// A count of 1 returns lines[startingLineIndex] only.<br/>
    /// A count of 2 returns lines[startingLineIndex] and lines[startingLineIndex + 1].<br/>
    /// </param>
    public RichCharacter[][] GetLineRichCharacterRange(int startingLineIndex, int count)
    {
        var lineCountAvailable = LineEndList.Count - startingLineIndex;

        var lineCountToReturn = count < lineCountAvailable
            ? count
            : lineCountAvailable;

        var endingLineIndexExclusive = startingLineIndex + lineCountToReturn;
        var lineList = new RichCharacter[lineCountToReturn][];

        if (lineCountToReturn < 0 || startingLineIndex < 0 || endingLineIndexExclusive < 0)
            return lineList;

		var addIndex = 0;

        for (var i = startingLineIndex; i < endingLineIndexExclusive; i++)
        {
            // Previous line's end-position-exclusive is this row's start.
            var startOfLineInclusive = GetLineInformation(i).Position_StartInclusiveIndex;
            var endOfLineExclusive = LineEndList[i].Position_EndExclusiveIndex;

			// TODO: LINQ used in a hot path (the virtualization invokes this method)
            var line = RichCharacterList
                .Skip(startOfLineInclusive)
                .Take(endOfLineExclusive - startOfLineInclusive)
                .ToArray();

            lineList[addIndex++] = line;
        }

        return lineList;
    }

    public int GetTabCountOnSameLineBeforeCursor(int lineIndex, int columnIndex)
    {
        var line = GetLineInformation(lineIndex);

        AssertColumnIndex(line, columnIndex);
        
        var count = 0;
        var foundSpan = false;
        
        foreach (var tabCharPositionIndex in TabCharPositionIndexList)
        {
        	if (!foundSpan && tabCharPositionIndex < line.Position_StartInclusiveIndex)
        		continue;
        	else
        		foundSpan = true;
        		
        	if (tabCharPositionIndex < line.Position_StartInclusiveIndex + columnIndex)
        		count++;
        	else
        		break;
        }
        
        return count;
    }

    /// <summary>
    /// Implementations of this method are expected to have caching.
    /// </summary>
    public string GetAllText()
    {
        return AllText;
    }

    public int GetPositionIndex(TextEditorCursor cursor)
    {
        return GetPositionIndex(cursor.LineIndex, cursor.ColumnIndex);
    }

    public int GetPositionIndex(TextEditorCursorModifier cursorModifier)
    {
        return GetPositionIndex(cursorModifier.LineIndex, cursorModifier.ColumnIndex);
    }

    public int GetPositionIndex(int lineIndex, int columnIndex)
    {
        var line = GetLineInformation(lineIndex);

        AssertColumnIndex(line, columnIndex);

        return line.Position_StartInclusiveIndex + columnIndex;
    }

    public (int lineIndex, int columnIndex) GetLineAndColumnIndicesFromPositionIndex(
        int positionIndex)
    {
        var lineInformation = GetLineInformationFromPositionIndex(positionIndex);

        return (
            lineInformation.Index,
            positionIndex - lineInformation.Position_StartInclusiveIndex);
    }

    /// <summary>
    /// To receive a <see cref="string"/> value, one can use <see cref="GetString"/> instead.
    /// </summary>
    public char GetCharacter(int positionIndex)
    {
        AssertPositionIndex(positionIndex);

        if (positionIndex == CharCount)
            return ParserFacts.END_OF_FILE;

        return RichCharacterList[positionIndex].Value;
    }

    /// <summary>
    /// To receive a <see cref="char"/> value, one can use <see cref="GetCharacter"/> instead.
    /// </summary>
    public string GetString(int positionIndex, int count)
    {
        AssertPositionIndex(positionIndex);
        AssertCount(count);

        return new string(RichCharacterList
            .Skip(positionIndex)
            .Take(count)
            .Select(x => x.Value)
            .ToArray());
    }

    public string GetLineTextRange(int lineIndex, int count)
    {
        AssertCount(count);

        var startPositionIndexInclusive = GetPositionIndex(lineIndex, 0);
        var lastLineIndexExclusive = lineIndex + count;
        int endPositionIndexExclusive;

        if (lastLineIndexExclusive > LineCount - 1)
        {
            endPositionIndexExclusive = CharCount;
        }
        else
        {
            endPositionIndexExclusive = GetPositionIndex(lastLineIndexExclusive, 0);
        }

        return GetString(
            startPositionIndexInclusive,
            endPositionIndexExclusive - startPositionIndexInclusive);
    }

    /// <summary>
    /// Given a <see cref="TextEditorModel"/> with a preference for the right side of the cursor, the following conditional branch will play out:<br/><br/>
    ///     -IF the cursor is amongst a word, that word will be returned.<br/><br/>
    ///     -ELSE IF the start of a word is to the right of the cursor that word will be returned.<br/><br/>
    ///     -ELSE IF the end of a word is to the left of the cursor that word will be returned.</summary>
    public TextEditorTextSpan? GetWordTextSpan(int positionIndex)
    {
        var previousCharacter = GetCharacter(positionIndex - 1);
        var currentCharacter = GetCharacter(positionIndex);

        var previousCharacterKind = CharacterKindHelper.CharToCharacterKind(previousCharacter);
        var currentCharacterKind = CharacterKindHelper.CharToCharacterKind(currentCharacter);

        var lineInformation = GetLineInformationFromPositionIndex(positionIndex);
        var columnIndex = positionIndex - lineInformation.Position_StartInclusiveIndex;

        if (previousCharacterKind == CharacterKind.LetterOrDigit && currentCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.Index,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            var wordColumnIndexEndExclusive = GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.Index,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = GetLineLength(lineInformation.Index);

            return new TextEditorTextSpan(
                wordColumnIndexStartInclusive + lineInformation.Position_StartInclusiveIndex,
                wordColumnIndexEndExclusive + lineInformation.Position_StartInclusiveIndex,
                0,
                ResourceUri,
                GetAllText());
        }
        else if (currentCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexEndExclusive = GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.Index,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = GetLineLength(lineInformation.Index);

            return new TextEditorTextSpan(
                columnIndex + lineInformation.Position_StartInclusiveIndex,
                wordColumnIndexEndExclusive + lineInformation.Position_StartInclusiveIndex,
                0,
                ResourceUri,
                GetAllText());
        }
        else if (previousCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = GetColumnIndexOfCharacterWithDifferingKind(
                lineInformation.Index,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            return new TextEditorTextSpan(
                wordColumnIndexStartInclusive + lineInformation.Position_StartInclusiveIndex,
                columnIndex + lineInformation.Position_StartInclusiveIndex,
                0,
                ResourceUri,
                GetAllText());
        }

        return null;
    }

    public List<TextEditorTextSpan> FindMatches(string query)
    {
        var text = GetAllText();
        var matchedTextSpans = new List<TextEditorTextSpan>();

        for (int outerI = 0; outerI < text.Length; outerI++)
        {
            if (outerI + query.Length <= text.Length)
            {
                int innerI = 0;
                for (; innerI < query.Length; innerI++)
                {
                    if (text[outerI + innerI] != query[innerI])
                        break;
                }

                if (innerI == query.Length)
                {
                    // Then the entire query was matched
                    matchedTextSpans.Add(new TextEditorTextSpan(
                        outerI,
                        outerI + innerI,
                        (byte)FindOverlayDecorationKind.LongestCommonSubsequence,
                        ResourceUri,
                        text));
                }
            }
        }

        return matchedTextSpans;
    }

    /// <summary>
    /// 'lineIndex' equal to '0' returns the first line.<br/><br/>
    /// 
    /// An index for <see cref="TextEditorModel.LineEndList"/> maps 1 to 1 with this method.
    /// (i.e.) the 0th line-end index will end up returning the 0th line.<br/><br/>
    /// 
    /// Given a 'lineIndex', return the <see cref="LineEnd"/> at <see cref="TextEditorModel.LineEndList"/>[lineIndex - 1],
    /// and <see cref="TextEditorModel.LineEndList"/>[lineIndex]
    /// in the form of the type <see cref="LineInformation"/>.
    /// </summary>
    /// <remarks>
    /// When 'lineIndex' == 0, then a "made-up" line ending named <see cref="LineEnd.StartOfFile"/> will be used
    /// in place of indexing at '<see cref="TextEditorModel.LineEndList"/>[-1]'
    /// </remarks>
    public LineInformation GetLineInformation(int lineIndex)
    {
        AssertLineIndex(lineIndex);

        LineEnd GetLineEndLower(int lineIndex)
        {
            // Large index? Then set the index to the last index.
            lineIndex = Math.Min(lineIndex, LineEndList.Count - 1);

            // Small index? Then return StartOfFile.
            if (lineIndex <= 0)
                return new(0, 0, LineEndKind.StartOfFile);

            // In-range index? Then return the previous line's line ending.
            return LineEndList[lineIndex - 1];
        }

        LineEnd GetLineEndUpper(int lineIndex)
        {
            // Large index? Then set the index to the last index.
            lineIndex = Math.Min(lineIndex, LineEndList.Count - 1);

            // Small index? Then return the first LineEnd
            if (lineIndex <= 0)
                return LineEndList[0];

            // In-range index? Then return the LineEnd at that index.
            return LineEndList[lineIndex];
        }
        
        var lineEndLower = GetLineEndLower(lineIndex);
        var lineEndUpper = GetLineEndUpper(lineIndex);

        return new LineInformation(
            lineIndex,
            lineEndLower.Position_EndExclusiveIndex,
            lineEndUpper.Position_EndExclusiveIndex,
            lineEndLower,
            lineEndUpper);
    }

    public LineInformation GetLineInformationFromPositionIndex(int positionIndex)
    {
        AssertPositionIndex(positionIndex);

        int GetLineIndexFromPositionIndex()
        {
            // StartOfFile
            if (LineEndList[0].Position_EndExclusiveIndex > positionIndex)
                return 0;

            // EndOfFile
            if (LineEndList[^1].Position_EndExclusiveIndex <= positionIndex)
                return LineEndList.Count - 1;

            // In-between
            for (var i = 1; i < LineEndList.Count; i++)
            {
                var lineEndTuple = LineEndList[i];

                if (lineEndTuple.Position_EndExclusiveIndex > positionIndex)
                    return i;
            }

            // Fallback return StartOfFile
            return 0;
        }

        return GetLineInformation(GetLineIndexFromPositionIndex());
    }

    /// <summary>
    /// <see cref="moveBackwards"/> is to mean earlier in the document
    /// (lower column index or lower row index depending on position) 
    /// </summary>
    /// <returns>Will return -1 if no valid result was found.</returns>
    public int GetColumnIndexOfCharacterWithDifferingKind(
        int lineIndex,
        int columnIndex,
        bool moveBackwards)
    {
        var iterateBy = moveBackwards
            ? -1
            : 1;

        var lineStartPositionIndex = GetLineInformation(lineIndex).Position_StartInclusiveIndex;

        if (lineIndex > LineEndList.Count - 1)
            return -1;

        var lastPositionIndexOnLine = LineEndList[lineIndex].Position_EndExclusiveIndex - 1;
        var positionIndex = GetPositionIndex(lineIndex, columnIndex);

        if (moveBackwards)
        {
            if (positionIndex <= lineStartPositionIndex)
                return -1;

            positionIndex -= 1;
        }

        if (positionIndex < 0 || positionIndex >= RichCharacterList.Length)
            return -1;

        var startCharacterKind = CharacterKindHelper.CharToCharacterKind(RichCharacterList[positionIndex].Value);

        while (true)
        {
            if (positionIndex >= RichCharacterList.Length ||
                positionIndex > lastPositionIndexOnLine ||
                positionIndex < lineStartPositionIndex)
            {
                return -1;
            }

            var currentCharacterKind = CharacterKindHelper.CharToCharacterKind(RichCharacterList[positionIndex].Value);

            if (currentCharacterKind != startCharacterKind)
                break;

            positionIndex += iterateBy;
        }

        if (moveBackwards)
            positionIndex += 1;

        return positionIndex - lineStartPositionIndex;
    }

    public bool CanUndoEdit()
    {
        return EditBlockIndex > 0;
    }

    public bool CanRedoEdit()
    {
        return EditBlockIndex < EditBlockList.Count - 1;
    }

    public CharacterKind GetCharacterKind(int positionIndex)
    {
        AssertPositionIndex(positionIndex);

        if (positionIndex == CharCount)
            return CharacterKind.Bad;

        return CharacterKindHelper.CharToCharacterKind(RichCharacterList[positionIndex].Value);
    }

    /// <summary>
    /// This method and <see cref="ReadNextWordOrDefault(TextEditorModel, int, int)"/>
    /// are separate because of 'Ctrl + Space' bring up autocomplete when at a period.
    /// </summary>
    public string? ReadPreviousWordOrDefault(
        int lineIndex,
        int columnIndex,
        bool isRecursiveCall = false)
    {
        var wordPositionIndexEndExclusive = GetPositionIndex(lineIndex, columnIndex);
        var wordCharacterKind = GetCharacterKind(wordPositionIndexEndExclusive - 1);

        if (wordCharacterKind == CharacterKind.Punctuation && !isRecursiveCall)
        {
            // If previous previous word is a punctuation character, then perhaps
            // the user hit { 'Ctrl' + 'Space' } to trigger the autocomplete
            // and was at a MemberAccessToken (or a period '.')
            //
            // So, read the word previous to the punctuation.

            var anotherAttemptColumnIndex = columnIndex - 1;

            if (anotherAttemptColumnIndex >= 0)
                return ReadPreviousWordOrDefault(lineIndex, anotherAttemptColumnIndex, true);
        }

        if (wordCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexStartInclusive = GetColumnIndexOfCharacterWithDifferingKind(
                lineIndex,
                columnIndex,
                true);

            if (wordColumnIndexStartInclusive == -1)
                wordColumnIndexStartInclusive = 0;

            var wordLength = columnIndex - wordColumnIndexStartInclusive;
            var wordPositionIndexStartInclusive = wordPositionIndexEndExclusive - wordLength;

            return GetString(wordPositionIndexStartInclusive, wordLength);
        }

        return null;
    }

    /// <summary>
    /// This method and <see cref="ReadPreviousWordOrDefault(TextEditorModel, int, int, bool)"/>
    /// are separate because of 'Ctrl + Space' bring up autocomplete when at a period.
    /// </summary>
    public string? ReadNextWordOrDefault(int lineIndex, int columnIndex)
    {
        var wordPositionIndexStartInclusive = GetPositionIndex(lineIndex, columnIndex);
        var wordCharacterKind = GetCharacterKind(wordPositionIndexStartInclusive);

        if (wordCharacterKind == CharacterKind.LetterOrDigit)
        {
            var wordColumnIndexEndExclusive = GetColumnIndexOfCharacterWithDifferingKind(
                lineIndex,
                columnIndex,
                false);

            if (wordColumnIndexEndExclusive == -1)
                wordColumnIndexEndExclusive = GetLineLength(lineIndex);

            var wordLength = wordColumnIndexEndExclusive - columnIndex;

            return GetString(wordPositionIndexStartInclusive, wordLength);
        }

        return null;
    }

    /// <summary>
    /// This method returns the text to the left of the cursor in most cases.
    /// The method name is as such because of right to left written texts.<br/><br/>
    /// One uses this method most often to measure the position of the cursor when rendering the
    /// UI for a font-family which is proportional (i.e. not monospace).
    /// </summary>
    public string GetTextOffsettingCursor(TextEditorCursor textEditorCursor)
    {
        var cursorPositionIndex = GetPositionIndex(textEditorCursor);
        var lineStartPositionIndexInclusive = GetLineInformation(textEditorCursor.LineIndex).Position_StartInclusiveIndex;

        return GetString(lineStartPositionIndexInclusive, cursorPositionIndex - lineStartPositionIndexInclusive);
    }

    public string GetLineText(int lineIndex)
    {
        var lineStartPositionIndexInclusive = GetLineInformation(lineIndex).Position_StartInclusiveIndex;
        var lengthOfLine = GetLineLength(lineIndex, true);

        return GetString(lineStartPositionIndexInclusive, lengthOfLine);
    }

    public void AssertColumnIndex(LineInformation line, int columnIndex)
    {
        if (columnIndex < 0)
            throw new LuthetusTextEditorException($"'{nameof(columnIndex)}:{columnIndex}' < 0");
        
        if (columnIndex > line.LastValidColumnIndex)
            throw new LuthetusTextEditorException($"'{nameof(columnIndex)}:{columnIndex}' > {nameof(line)}.{nameof(line.LastValidColumnIndex)}:{line.LastValidColumnIndex}");
    }
    
    public void AssertLineIndex(int lineIndex)
    {
        if (lineIndex < 0)
            throw new LuthetusTextEditorException($"'{nameof(lineIndex)}:{lineIndex}' < 0");
        
        if (lineIndex >= LineCount)
            throw new LuthetusTextEditorException($"'{nameof(lineIndex)}:{lineIndex}' >= {nameof(LineCount)}:{LineCount}");
    }

    public void AssertPositionIndex(int positionIndex)
    {
        if (positionIndex < 0)
            throw new LuthetusTextEditorException($"'{nameof(positionIndex)}:{positionIndex}' < 0");
        
        // NOTE: DocumentLength is a valid position for the cursor to be at.
        if (positionIndex > CharCount)
            throw new LuthetusTextEditorException($"'{nameof(positionIndex)}:{positionIndex}' > {nameof(CharCount)}:{CharCount}");
    }
    
    public void AssertCount(int count)
    {
        if (count < 0)
            throw new LuthetusTextEditorException($"'{nameof(count)}:{count}' < 0");
    }
    #endregion
    
    #region TextEditorModelPartitions
    public void __Insert(int globalPositionIndex, RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition partition = PartitionList[i];

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
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.Insert(relativePositionIndex, richCharacter);

        PartitionListSetItem(
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
        RichCharacter richCharacter)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition partition = PartitionList[i];

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
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
        var outPartition = inPartition.SetItem(relativePositionIndex, richCharacter);

        PartitionListSetItem(
            indexOfPartitionWithAvailableSpace,
            outPartition);
    }

    public void __SetDecorationByte(
        int globalPositionIndex,
        byte decorationByte)
    {
        int indexOfPartitionWithAvailableSpace = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition partition = PartitionList[i];

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
            throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
        var targetRichCharacter = inPartition.RichCharacterList[relativePositionIndex];
        
        inPartition.RichCharacterList[relativePositionIndex] = new(
        	targetRichCharacter.Value,
        	decorationByte);
        _partitionListChanged = true;
    }

    public void __RemoveAt(int globalPositionIndex)
    {
        if (globalPositionIndex >= CharCount)
            return;

        int indexOfPartitionWithContent = -1;
        int relativePositionIndex = -1;
        var runningCount = 0;

        for (int i = 0; i < PartitionList.Count; i++)
        {
            TextEditorPartition partition = PartitionList[i];

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
            throw new LuthetusTextEditorException("if (indexOfPartitionWithContent == -1)");

        if (relativePositionIndex == -1)
            throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

        var inPartition = PartitionList[indexOfPartitionWithContent];
        var outPartition = inPartition.RemoveAt(relativePositionIndex);

        PartitionListSetItem(
            indexOfPartitionWithContent,
            outPartition);
    }

    private void __InsertNewPartition(int partitionIndex)
    {
        PartitionListInsert(partitionIndex, new TextEditorPartition(new List<RichCharacter>()));
    }

    private void __SplitIntoTwoPartitions(int partitionIndex)
    {
        var originalPartition = PartitionList[partitionIndex];

        var firstUnevenSplit = PartitionSize / 2 + PartitionSize % 2;
        var secondUnevenSplit = PartitionSize / 2;

        // Validate multi-byte characters go on same partition (i.e.: '\r\n')
        {
            // firstUnevenSplit is a count so -1 to make it an index
            if (originalPartition.RichCharacterList[firstUnevenSplit - 1].Value == '\r')
            {
                if (originalPartition.RichCharacterList[(firstUnevenSplit - 1) + 1].Value == '\n')
                {
                    firstUnevenSplit += 1;
                    secondUnevenSplit -= 1;
                }
            }

            // TODO: If the partition to split ends in '\r' and the cause for the split
            //       is to create space in order to insert a '\n',
            //       |
            //       Then this works out as a "happy accident" of sorts.
            //       This is not ideal, it should be more concrete than "oops it worked".
            //       |
            //       The reason it works is because a split won't check if the next partition
            //       has space (? source needed) and will always move the '\r' to the new partition,
            //       then return to the insert and put the '\n' immediately after.

            // One of the reasons for not having a multi-byte character span multiple partitions,
            // is that if a partition has capacity 4,096 but a count of 2,048,
            // one cannot insert between the bytes of a multi-byte character
            // so the first partition with only half its capacity used, would be unable to be used
            // any further than 2,048 because it would mean writing between its multi-byte character
            // than spans into the next partition.
        }

        // Replace old
        {
            var partition = new TextEditorPartition(originalPartition.RichCharacterList
                .Skip(0)
                .Take(firstUnevenSplit)
                .ToList());

            PartitionListSetItem(
                partitionIndex,
                partition);
        }

        // Insert new
        {
            var partition = new TextEditorPartition(originalPartition.RichCharacterList
                .Skip(firstUnevenSplit)
                .Take(secondUnevenSplit)
                .ToList());

            PartitionListInsert(
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
            TextEditorPartition partition;

            for (int i = 0; i < PartitionList.Count; i++)
            {
                partition = PartitionList[i];

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
                throw new LuthetusTextEditorException("if (indexOfPartitionWithAvailableSpace == -1)");

            if (relativePositionIndex == -1)
                throw new LuthetusTextEditorException("if (relativePositionIndex == -1)");

            partition = PartitionList[indexOfPartitionWithAvailableSpace];
            var partitionAvailableSpace = PartitionSize - partition.Count;

            var richCharacterBatchInsertList = new List<RichCharacter> { richCharacterEnumerator.Current };

            while (richCharacterBatchInsertList.Count < partitionAvailableSpace && richCharacterEnumerator.MoveNext())
            {
                richCharacterBatchInsertList.Add(richCharacterEnumerator.Current);
            }

            var inPartition = PartitionList[indexOfPartitionWithAvailableSpace];
            var outPartition = inPartition.InsertRange(relativePositionIndex, richCharacterBatchInsertList);

            PartitionListSetItem(
                indexOfPartitionWithAvailableSpace,
                outPartition);

            globalPositionIndex += richCharacterBatchInsertList.Count;
        }
    }

    public void __RemoveRange(int targetGlobalPositionIndex, int targetDeleteCount)
    {
        var foundTargetGlobalPositionIndex = false;
        // The 'searchGlobalPositionIndex' is no longer updated after 'foundTargetGlobalPositionIndex' is true
        // It is just being used to find where the data that should be deleted starts.
        var searchGlobalPositionIndex = 0;
        var runningDeleteCount = 0;

        for (int partitionIndex = 0; partitionIndex < PartitionList.Count; partitionIndex++)
        {
            var partition = PartitionList[partitionIndex];
            var relativePositionIndex = 0;

            if (!foundTargetGlobalPositionIndex)
            {
                // It is '>' specifically, because '0 + partition.Count' is a count, therefore the
                // largest index that could exist in the partition is 1 less than the partition.Count.
                if (searchGlobalPositionIndex + partition.Count > targetGlobalPositionIndex)
                {
                    foundTargetGlobalPositionIndex = true;
                    relativePositionIndex = targetGlobalPositionIndex - searchGlobalPositionIndex;
                }
                else
                {
                    searchGlobalPositionIndex += partition.Count;
                    continue;
                }
            }

            // This section of code is dependent on the condition branch above it having performed
            // a 'continue' if it was entered, but still didn't find the 'targetGlobalPositionIndex'

            var availableDeletes = partition.Count - relativePositionIndex;
            var remainingDeletes = targetDeleteCount - runningDeleteCount;

            var deletes = availableDeletes < remainingDeletes
                ? availableDeletes
                : remainingDeletes;

            // WARNING: The code does not currently alter the _partitionList in any way other than this 'SetItem'...
            //          ...invocation, with regards to this method.
            //          If one adds other alterations to the _partitionList in this method,
            //          check if this logic would break.
            PartitionListSetItem(
                partitionIndex,
                partition.RemoveRange(relativePositionIndex, deletes));

            runningDeleteCount += deletes;

            if (runningDeleteCount >= targetDeleteCount)
                break;
        }
    }

    public void __Add(RichCharacter richCharacter)
    {
        __Insert(CharCount, richCharacter);
    }
    
    public void PartitionListSetItem(int index, TextEditorPartition partition)
    {
    	if (!_partitionListIsShallowCopy)
    	{
    		_partitionListIsShallowCopy = true;
    		PartitionList = new(PartitionList);
    	}
    	
    	PartitionList[index] = partition;
    	_partitionListChanged = true;
    }
    
    public void PartitionListInsert(int index, TextEditorPartition partition)
    {
    	if (!_partitionListIsShallowCopy)
    	{
    		_partitionListIsShallowCopy = true;
    		PartitionList = new(PartitionList);
    	}
    	
    	PartitionList.Insert(index, partition);
    	_partitionListChanged = true;
    }
    #endregion
}
