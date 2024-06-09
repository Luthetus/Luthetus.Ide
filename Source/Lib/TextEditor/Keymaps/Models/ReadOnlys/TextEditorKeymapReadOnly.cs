using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.ReadOnlys;

public class TextEditorKeymapReadOnly : Keymap, ITextEditorKeymap
{
	private readonly Func<Key<TextEditorViewModel>> _getViewModelKeyFunc;

	public TextEditorKeymapReadOnly(Func<Key<TextEditorViewModel>> getViewModelKeyFunc)
        : base(new Key<Keymap>(Guid.Parse("326d1b3b-4d8a-43be-a377-330375593d0d")),
               "ReadOnly")
    {
		_getViewModelKeyFunc = getViewModelKeyFunc;
        
        AddDefaultCtrlModifiedKeymap();
        AddDefaultAltModifiedKeymap();
        AddDefaultHasSelectionLayerModifiedKeymap();
        AddDefaultMiscKeymap();
	}

    public Key<KeymapLayer> GetLayer(bool hasSelection)
    {
        return hasSelection
            ? TextEditorKeymapReadOnlyFacts.HasSelectionLayer.Key
            : TextEditorKeymapReadOnlyFacts.DefaultLayer.Key;
    }

    public string GetCursorCssClassString()
    {
        return TextCursorKindFacts.BeamCssClassString;
    }

    public string GetCursorCssStyleString(
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions)
    {
        return string.Empty;
    }

    private void AddDefaultCtrlModifiedKeymap()
    {
        AddDefaultCtrlAltModifiedKeymap();

        Map.Add(new KeymapArgument("KeyX")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Cut);

        Map.Add(new KeymapArgument("KeyC")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Copy);

        Map.Add(new KeymapArgument("KeyV")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.PasteCommand);

        Map.Add(new KeymapArgument("KeyS")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Save);

        Map.Add(new KeymapArgument("KeyA")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.SelectAll);

        Map.Add(new KeymapArgument("KeyZ")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Undo);

        Map.Add(new KeymapArgument("KeyY")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Redo);

        Map.Add(new KeymapArgument("KeyD")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Duplicate);

        Map.Add(new KeymapArgument("BracketRight")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(false));

        Map.Add(new KeymapArgument("BracketRight")
        {
            CtrlKey = true,
            ShiftKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(true));

        Map.Add(new KeymapArgument("KeyF")
        {
            CtrlKey = true,
            ShiftKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowFindAllDialog);

        Map.Add(new KeymapArgument("ArrowDown")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollLineDown);

        Map.Add(new KeymapArgument("ArrowUp")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollLineUp);

        Map.Add(new KeymapArgument("PageDown")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.CursorMovePageBottom);

        Map.Add(new KeymapArgument("PageUp")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.CursorMovePageTop);

        Map.Add(new KeymapArgument(KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.NewLineAbove);
        
        Map.Add(new KeymapArgument(KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE)
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        
        Map.Add(new KeymapArgument("Comma")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        
        Map.Add(new KeymapArgument("Slash")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowTooltipByCursorPosition);
        
        Map.Add(new KeymapArgument("KeyF")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowFindOverlay);
    }

    private void AddDefaultAltModifiedKeymap()
    {
        return;
    }

    private void AddDefaultHasSelectionLayerModifiedKeymap()
    {
        Map.Add(new KeymapArgument(KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
        {
            LayerKey = TextEditorKeymapReadOnlyFacts.HasSelectionLayer.Key
        }, TextEditorCommandDefaultFacts.IndentMore);

        Map.Add(new KeymapArgument(KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
        {
            ShiftKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.HasSelectionLayer.Key
        }, TextEditorCommandDefaultFacts.IndentLess);
    }

    private void AddDefaultCtrlAltModifiedKeymap()
    {
        // 65 to 90 provides capital letters (both sides inclusive) (ASCII)
        for (int i = 65; i <= 90; i++)
        {
            var character = (char)i;

            _ = Map.TryAdd(new KeymapArgument($"Key{character}")
            {
                ShiftKey = true,
                CtrlKey = true,
                AltKey = true,
                LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
            }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        }

        // 97 to 122 provides lowercase letters (both sides inclusive) (ASCII)
        for (int i = 97; i <= 122; i++)
        {
            var character = (char)i;

            _ = Map.TryAdd(new KeymapArgument($"Key{char.ToUpperInvariant(character)}")
            {
                CtrlKey = true,
                AltKey = true,
                LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
            }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        }
    }

    private void AddDefaultMiscKeymap()
    {
        Map.Add(new KeymapArgument("F12")
        {
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToDefinition);

        Map.Add(new KeymapArgument(KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            ShiftKey = true,
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.NewLineBelow);

        Map.Add(new KeymapArgument("PageDown")
        {
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollPageDown);

        Map.Add(new KeymapArgument("PageUp")
        {
            LayerKey = TextEditorKeymapReadOnlyFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollPageUp);
    }

	public bool TryMap(KeyboardEventArgs keyboardEventArgs, KeymapArgument keymapArgument, TextEditorComponentData componentData, out CommandNoType? command)
	{
        return Map.TryGetValue(keymapArgument, out command);
	}
}
