The issue is: The code for dragging a editor tab to a panel tab is not working.

In order to understand the issue I want to ask myself: "Why does dragging
a editor tab into a dialog, then back, work?

It works because the 'TextEditorTabViewModel.cs' is presuming that it was constructed
from within a 'TextEditorGroupDisplay.razor.cs'.

The previous point is weak.

The real reason it works, is because a dialog does not need to know anything about
the other dialogs.

Whereas a panel tab, must know when it is or isn't the active tab of the panel group.

Furthermore, a dialog does not need to understand who its 'Parent' is.

All the dialog does is render the inner component.

What "concepts" can I list, that makeup this problem as a whole?

// All the following bullet list entries ended with the text: "ViewModel",
// so I'm going to omit that text.
-----------------------------------
- Polymorphic
- Tab
- Draggable
- Dropzone
- Dropzone

There are more concepts than just the "ViewModel"(s) though.

A Tab can be rendered by both a:
- PanelGroup
- TextEditorGroup

What name can be given to the previous bulleted list?
Are these items containers?

If I want to render a 'TextEditorViewModelDisplay.razor' in a 'TextEditorGroup',
what would I do, ignoring the existence of 'TextEditorTabViewModel' as it currently is.

public class TextEditorTabViewModel
{
	
}

--------------

Tangent: the issue is, the tabs can be rendered by various 'containers', and yet,
the 'TextEditorTabViewModel', only understands the 'TextEditorGroup' container.

How can I make 'TextEditorTabViewModel' understand what a 'PanelGroup' container is?

----------------------

Also think about, which came first the chicken or the egg.

The 'TextEditorTabViewModel' can be rendered in a 'PanelGroup' only by
knowing the 'PanelGroup' class.

Can a tab be rendered by a container, of which, the tab when being written the container
source code did not exist.

Yes?

Why is the code for the 'tab' so tied to the 'TextEditorTabViewModel'?

The issue is that, 'TextEditorTabViewModel' needs to know who the container is.

'TextEditorTabViewModel' is too tied to a 'TextEditorGroup'.

The 'TextEditorGroup' is not always available.

As a dialog, tab of a panel group, or a notification even, the 'TextEditorGroup' would be null.

--------------------

Panels don't know what a 'text editor' is. The project references go the opposite direction.

Instead of thinking about taking a text editor tab, and converting it to a tab on a panel,
how do I simply make a text editor tab on a panel from the get go (without this
conversion logic).

Instead of having 'ITabViewModel', contain all necessary information to render it as the active tab,
the 'containers' are looking up the actual state, by using a key which is on the tab.

I think if do as was done with the dialog, in the tabs, then this will work.

The container cannot be expected to understand what it is rendering.
The most obvious showcase of this would be that I want to render
a 'text editor' in a 'panel', yet 'panel'(s) are in a different project,
that don't have a reference to the 'text editor' code.

Okay, I think the 'container' needs to be known by the view model.
And then, the extent of what a view model knows, is limited by whether
the 'container' understand too what the 'view model' is.

Why is 'Panel.razor' used as the name for the component which renders
a 'PanelGroup'?

Tabs know too much, the container should do most of what currently is done
within 'tab'.