using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models;

public interface ITextEditorKeymap
{
	public Key<KeymapLayer> GetLayer(bool hasSelection);

    public string GetCursorCssClassString();

    public string GetCursorCssStyleString(
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions);

	/// <summary>
	/// TODO: (the following comment was copy and pasted from <see cref="Keymap"/>)...
	///       ...This dictionary typed property is an implementation detail and should be
	///       hidden behind an interface.
	///       |
	///       Further details: An integrated terminal is currently being written.
	///       The keymap for the integrated terminal is intended to capture the
	///       keyboard event args.
	///       ...
	///       This would allow the integrated terminal to then check if the cursor
	///       is on the final line of the text editor or not.
	///       ...
	///       If the cursor is on the final line, then write out the text,
	///       otherwise treat the text editor as readonly because the cursor
	///       is at 'history'
	///       ...
	///       But, with all this said, one cannot 'delay' the insertion of the text,
	///       because the integrated terminal when intercepting the keystroke does not have
	///       access to the keyboard event.
	///       ...
	///       If an interface IKeymap existed, then the methods:
	///         -bool TryRegister(KeyboardEventArgs args, Command command)
	///         -bool TryMap(KeyboardEventArgs args, out Command command)
	///		  Could be made, such that 'TryMap' would have a reference to the 'KeyboardEventArgs'
	/// </summary>
	public bool TryMap(KeyboardEventArgs keyboardEventArgs, KeymapArgs keymapArgument, TextEditorComponentData componentData, out CommandNoType? command);
}