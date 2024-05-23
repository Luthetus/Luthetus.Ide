namespace Luthetus.TextEditor.Tests.Adhoc.Rewrite;

public class TextEditorWorkInsertion
{
	public TextEditorWorkKind TextEditorWorkKind => TextEditorWorkKind.Insertion;

	/// <summary>
	/// The resource uri of the model which is to be worked upon.
	/// </summary>
	public ResourceUri ResourceUri { get; }

	/// <summary>
	/// This property is optional, and can be Key<TextEditorViewModel>.Empty,
	/// if one does not make use of it.
	///
	/// The key of the view model which is to be worked upon.
	/// </summary>
	public Key<TextEditorViewModel> ViewModelKey { get; }

	/// <summary>
	/// Track where the content should be inserted.
	/// </summary>
	Key<TextEditorCursor> CursorKey;
	
	/// <summary>
	/// The content to insert.
	/// </summary>
	StringBuilder Content;

	public Task Invoke(ITextEditorEditContext editContext)
	{
		var modelModifier = editContext.GetModelModifier(ResourceUri);

/*
Case: a user hits { Ctrl + . } to bring up a quick actions menu.
They choose the menu-option to convert a for loop to a foreach loop.

If a user were permitted to continue editing, meanwhile
async code to determine the resulting foreach loop was occurring.

How would the quick action create a cursor at time of the user
picking the menu-option, and then keep the cursor
in the correct position given that the user might perform
insertions/deletions in the meanwhile that the foreach loop is calculated.

This description is hypothetical, and it might not be a realistic scenario.
Perhaps, the quick actions menu locks the user out of being able to do anything
until the quick action is finished?

===============================================================================

This case brings up the issue of a user's UI cursor, versus a cursor that
is managed purely programmatically and the user never sees it.

Given the previous case, if one were to lock the user out of being able to do
anything until the quick action is finished.

We would have to decide if we want to use the user's UI cursor,
while making deletions and insertions, or if we want to create our own
cursor for use purely in the quick action logic.

In the case of this quick action scenario, the user likely expects
their cursor not to move.

If we use the user's UI cursor for the calculation, then we'd have
to reset, the cursor-modifier back to its original position
after the modifications are performed.

The easiest solution is to lock the user out while the quick action is
being performed. And to furthermore, make a copy of their cursor
so that it doesn't have to be brought back to its original position.

How do you bring an "external" cursor into the ITextEditorEditContext?

Perhaps I have two consecutive insertions, but they are
not done in reference to a user's UI cursor.

Well, I cannot get the cursor by using the ViewModelKey because
that'd give me the user's UI cursor.

Perhaps I can provide in addition to the Key, an Func<Key, TextEditorCursor>.
This would allow one to check the editContext using their Key for the cursor.
If they do not find said cursor, then they invoke the Func, using their key.
This Func then returns the active, immutable version of the cursor.
Then, one can register that immutable cursor within the editContext to get
a CursorModifier for it.
*/

		return Task.CompletedTask;
	}
}
