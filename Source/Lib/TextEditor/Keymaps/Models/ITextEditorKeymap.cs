using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Events.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models;

/// <summary>
/// Are you not just writing the name of the keymap?
/// (or some unique identifier).
/// into local storage?
/// </summary>
public interface ITextEditorKeymap
{
	public string DisplayName { get; }

	public Key<KeymapLayer> GetLayer(bool hasSelection);

    public string GetCursorCssClassString();

    public string GetCursorCssStyleString(
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions);
	
	public ValueTask HandleEvent(
    	TextEditorComponentData componentData,
	    Key<TextEditorViewModel> viewModelKey,
	    KeyboardEventArgs keyboardEventArgs);
}