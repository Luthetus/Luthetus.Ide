2024-06-01
==========

Goal: optimize the EditBlock.cs logic

Issue:
	- Currently the EditBlock.cs takes a snapshot of the text editor's contents in its entirety.
	- Is it possible to instead track the only the changes to a text editor,
	  	and then have an understanding of how one would undo/redo those edits.
	- One such way of this is to introduce edit primitives that are the only way to
      	edit a document.
	- Then, each edit primitive could have a well defined do/undo
	- I want to try and type out what the editing will look like
	  	by doing a pseudo code example in my head and typing it out.

Pseudo code example:

(Some notes about naming):
	- Perhaps begin using the word 'document' to describe edits?

public enum DocumentEditKind
{
	Insert,
	Remove,
}

var textEditor = new TextEditor("");
textEditor.EditList ==
{
	// Empty
};

// textEditor.Insert(int positionIndex, string content);

textEditor.Insert(0, "Hello");
textEditor.EditList ==
{
	InsertEdit
	{
		PositionIndex = 0,
		Content = "Hello",

		// If the next insertion is at ContinuationPositionIndex then
		// an undo should undo both insertions.
		//
		// If the next insertion is NOT at ContinuationPositionIndex then
		// an undo should NOT undo both insertions.
		//
		ContinuationPositionIndex => PositionIndex + Content.Length
	},
};

textEditor.Insert(0, "Abc");
textEditor.EditList ==
{
	InsertEdit
	{
		PositionIndex = 0,
		Content = "Hello",
		ContinuationPositionIndex => PositionIndex + Content.Length
	},
	InsertEdit
	{
		PositionIndex = 0,
		Content = "Abc"
	},
};

public struct InsertEdit : ITextEditorEdit
{
	public InsertEdit(int positionIndex, string content)
	{
		PositionIndex = positionIndex;
		Content = content;
	}

	public int PositionIndex { get; }
	public string Content { get; }

	// If the next insertion is at ContinuationPositionIndex then
	// an undo should undo both insertions.
	//
	// If the next insertion is NOT at ContinuationPositionIndex then
	// an undo should NOT undo both insertions.
	//
	public int ContinuationPositionIndex => PositionIndex + Content.Length

	public EditKind EditKind => EditKind.Insert;
}

public struct RemoveEdit : ITextEditorEdit
{
	public RemoveEdit(int positionIndex, int length)
	{
		PositionIndex = positionIndex;
		Length = length;
	}

	public int PositionIndex { get; }
	public int Length { get; }

	// This type's ContinuationPositionIndex would be more complicated than the InsertEdit.
	// One can only insert the text in one direction.
	//
	// But, a mix of 'Backspace' and 'Remove' keypresses,
	// woud it be right to have an undo, combine the both of them?
	//
	public int ContinuationPositionIndex => PositionIndex + Content.Length;

	public EditKind EditKind => EditKind.Remove;
}

public void ToUndoEdit(ITextEditorEdit edit)
{
	switch (edit.EditKind)
	{
		case EditKind.Insert:
			return new RemoveEdit();
			break;
		case EditKind.Remove:
			return new InsertEdit();
			break;
		default:
			throw new NotImplementedException();
	}
}

public void Undo()
{
	var undoEdit = EditList.MostRecentlyPerformedEdit.ToUndoEdit();
}


// Given that a List<T> has the methods, 'Add(...)' and 'Remove(...)',
// should I stop using the naming of 'Delete' but instead also use the wording 'Remove'?
// (this change has been made)

// If I want to better replicate the cursor,
// is it necessary to split the 'RemoveEdit' into 'BackspaceEdit' and 'DeleteEdit'?
//
// My thought is no, but I'm uncertain. It seems that conversion of a BackspaceEdit
// to a DeleteEdit, that a 'do' would put you at the same positionIndex at the end,
// provided you convert them.
//
// Also I'm wondering if an 'undo' would share the same property as the 'do'.
// Perhaps the 'undo' ends up losing data about the cursor by converting from
// 'BackspaceEdit' to 'DeleteEdit' or vice versa.
//
// Yes, the 'undo' will no longer position the cursor correctly if I
// convert a 'BackspaceEdit' into a 'DeleteEdit'.
//
// If I want only 'RemoveEdit' and for it to represent both 'backspace' key presses
// and 'delete' key presses correctly (in regards to cursor position when undoing).
//
// Then, I could add another property to the 'RemoveEdit'.
// This property would then be on every instance that gets created.
//
// I believe the 'ITextEditorEdit' type is a rapidly constructed type.
// It won't just be a few of these. The user is constantly creating new edit instances.
// So perhaps the separation of 'RemoveEdit' into 'BackspaceEdit' and 'DeleteEdit',
// as to avoid the extra property would be of use?
//
// In reference to "The user is constantly creating new edit instances".
// I want to have the type be a 'struct' in order to avoid large amounts
// of garbage collection.

// It might be good to have not only an EditList.
// But to group each entry be an 'EditGroup'.
//
// This allows one to have edits which don't just undo the entirety
// of a consecutive sequence of edits; while maintaining
// a large backlog of edits.
//
// For example, if two consecutive InsertEdits are performed, but they are not
// contiguous edits. One may not want to perform and undo, then have both edits
// be undone.
//
// Yet, if one is permitted to have 10 edits in the EditList,
// one perhaps wouldn't want those 2 edits to take 2 slots of the edit list.

// This comment is sort of a side note.
//
// If I have a text editor consisting of 5 partitions.
// When I make an edit to the 5th partition,
// I could perhaps re-use the first 4 partitions?
//
// Well, now that I think of it.
// Why do I even 'string.Join(partitionList)' in the first place?
//
// What I'm referring to is, after every edit, I join the string content
// of every partition into a list.
//
// This operation is likely defeating the purpose of the partitions in the first place.
// Since an edit to 1 partition requires them all to be 'interacted with' again.
//
// A change to 1 partition preferably would limit any calculation to that particular partition.
//
// I recall the issue. The TextEditorTextSpan needed the total text.
//
// But, I could still join partition 'n' with 'n+1' and then cache the result.
// 