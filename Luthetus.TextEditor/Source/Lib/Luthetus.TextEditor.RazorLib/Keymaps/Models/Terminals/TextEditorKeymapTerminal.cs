using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;
using Luthetus.Common.RazorLib.Commands.Models;
using static Luthetus.TextEditor.RazorLib.TextEditors.Displays.TextEditorViewModelDisplay;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Terminals;

public class TextEditorKeymapTerminal : Keymap, ITextEditorKeymap
{
    public TextEditorKeymapTerminal()
        : base(
            new Key<Keymap>(Guid.Parse("baf160e1-6b43-494b-99db-0e8c7500facb")),
            "Terminal")
    {
    }

    public Key<KeymapLayer> GetLayer(bool hasSelection)
    {
        return hasSelection
            ? TextEditorKeymapTerminalFacts.HasSelectionLayer.Key
            : TextEditorKeymapTerminalFacts.DefaultLayer.Key;
    }

    public string GetCursorCssClassString()
    {
        return TextCursorKindFacts.BlockCssClassString;
    }

    public string GetCursorCssStyleString(
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions)
    {
		var characterWidthInPixels = textEditorViewModel.VirtualizationResult.CharAndRowMeasurements.CharacterWidth;
		var characterWidthInPixelsInvariantCulture = characterWidthInPixels.ToCssValue();
		return $"width: {characterWidthInPixelsInvariantCulture}px;";
	}

	public bool TryMap(
		KeymapArgument keymapArgument,
		TextEditorEvents events,
		out CommandNoType? command)
	{
		var commandDisplayName = "Terminal::InterceptDefaultKeymap";

		command = new TextEditorCommand(
			commandDisplayName, "terminal_intercept-default-keymap", false, true, TextEditKind.None, null,
			interfaceCommandArgs =>
			{
				var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

				commandArgs.TextEditorService.Post(
					nameof(commandDisplayName),
					async editContext =>
					{
						var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
						var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);
						var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
						var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

						if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
							return;

						if (primaryCursorModifier.RowIndex == modelModifier.RowCount - 1)
						{
							var throttleEventOnKeyDown = new ThrottleEventOnKeyDown(
								new TextEditorEvents(events, new TextEditorKeymapDefault()),
								keymapArgument.ToKeyboardEventArgs(),
								commandArgs.ModelResourceUri,
								commandArgs.ViewModelKey);

							await throttleEventOnKeyDown.InvokeWithEditContext(editContext);
						}
					});

				return Task.CompletedTask;
			});

		return true;
	}
}
