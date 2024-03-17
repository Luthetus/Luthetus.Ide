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

