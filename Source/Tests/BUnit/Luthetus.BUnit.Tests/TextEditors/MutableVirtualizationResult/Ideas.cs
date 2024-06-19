namespace Luthetus.BUnit.Tests.TextEditors.MutableVirtualizationResult;

/*
Virtualization is part of the text editor UI.
This way, only the text the user can fit in their view (this description is simple and inaccurate), is given to the UI
in order to be displayed.

The code that goes into calculating the virtualization result, is seemingly CPU intensive enough,
to warrant that insertion of text by a user should not re-calculate the virtualization result.

A further detail here, moving the scroll position should continue resulting in the re-calculation
of the virtualization result, this isn't seen as a problem. (albeit the current method of re-calculation
could be optimized).

A headache inducing thought is dealing with "random access insertion" so to speak.
If it is presumed a user through the UI and by typing on their keyboard can only edit their virtualization result
    (because anywhere the cursor goes the scroll position will move to, thus making it the new virtualization result).
Then how does one handle refactoring, which inserts text above the current virtualization result.
	Because, this would then move the scrollbar, but in a way that could push the existing virtualization result
		out of bounds, and result in re-calculation of the virtualization being necessary.

In short, it seems "easy enough" to predict what the virtualization result will be after the user types
the letter 'a' within the bounds of an existing virtualization result.

But, edits outside of the virtualization result might be a bit more complicated.

Therefore, it is recommended that the "within bounds of an existing virtualization result"
case is handled firstly.

And that any edit outside of an existing virtualization result would continue to re-trigger the
virtualization calculation for now.

A concern with this idea is that, if incorrectly implemented there could be 'desync' between
what the user sees in the text editor, and the actual text value behind the scenes.

It is vital that there never be any 'desync'.

Considering that there exists code to track the 'EditList'.

Perhaps one could track the starting EditIndex, and the ending EditIndex,
when finalizing an IEditContext.

Then, if the 'EditList' entries are seen to be in bounds of the
'virtualization result' then just insert directly to the 'virtualization result' those edits too.

Else, one could re-calculate the entire virtualization result.

Holding down any key on the keyboard, which inserts a character, demonstrates the current issue with
re-calculating the virtualization result on every insertion.

In a small enough file, there isn't much performance issues.
But, after about 10,000 characters it seems to be getting exponentially performing worse and worse
for every character that is within a file.

If I write a unit test which provides the initial virtualization result, then I can insert text within the bounds
of said virtualization result with this idea being described.

Afterwards, I can assert that the "modified virtualization result" equals what would have
been calculated had I re-invoked the calculate virtualization result method.

======================================================

Deletion of text might bring downstream unloaded text into view.
*/

/// <summary>TODO: Delete the file: "Ideas.cs"</summary>
public class Ideas
{
	
}
