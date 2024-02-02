using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;

public class TextEditorKeymapDefault : Keymap, ITextEditorKeymap
{
    public TextEditorKeymapDefault()
        : base(
            new Key<Keymap>(Guid.Parse("4aaca759-c2c7-4e6f-9d9f-f3d17172df16")),
            "Default")
    {
        AddDefaultCtrlModifiedKeymap();
        AddDefaultAltModifiedKeymap();
        AddDefaultHasSelectionLayerModifiedKeymap();
        AddDefaultMiscKeymap();
    }

    public Key<KeymapLayer> GetLayer(bool hasSelection)
    {
        return hasSelection
            ? TextEditorKeymapDefaultFacts.HasSelectionLayer.Key
            : TextEditorKeymapDefaultFacts.DefaultLayer.Key;
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
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Cut);

        Map.Add(new KeymapArgument("KeyC")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Copy);

        Map.Add(new KeymapArgument("KeyV")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.PasteCommand);

        Map.Add(new KeymapArgument("KeyS")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Save);

        Map.Add(new KeymapArgument("KeyA")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.SelectAll);

        Map.Add(new KeymapArgument("KeyZ")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Undo);

        Map.Add(new KeymapArgument("KeyY")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Redo);

        Map.Add(new KeymapArgument("KeyD")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Duplicate);

        Map.Add(new KeymapArgument("BracketRight")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(false));

        Map.Add(new KeymapArgument("BracketRight")
        {
            CtrlKey = true,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(true));

        Map.Add(new KeymapArgument("KeyF")
        {
            CtrlKey = true,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowFindAllDialog);

        Map.Add(new KeymapArgument("ArrowDown")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollLineDown);

        Map.Add(new KeymapArgument("ArrowUp")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollLineUp);

        Map.Add(new KeymapArgument("PageDown")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.CursorMovePageBottom);

        Map.Add(new KeymapArgument("PageUp")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.CursorMovePageTop);

        Map.Add(new KeymapArgument(KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.NewLineAbove);
        
        Map.Add(new KeymapArgument(KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE)
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        
        Map.Add(new KeymapArgument("Comma")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        
        Map.Add(new KeymapArgument("Slash")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowTooltipByCursorPosition);
        
        Map.Add(new KeymapArgument("KeyF")
        {
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
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
            LayerKey = TextEditorKeymapDefaultFacts.HasSelectionLayer.Key
        }, TextEditorCommandDefaultFacts.IndentMore);

        Map.Add(new KeymapArgument(KeyboardKeyFacts.WhitespaceCodes.TAB_CODE)
        {
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.HasSelectionLayer.Key
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
                LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
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
                LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
            }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        }
    }

    private void AddDefaultMiscKeymap()
    {
        Map.Add(new KeymapArgument("F12")
        {
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToDefinition);

        Map.Add(new KeymapArgument(KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.NewLineBelow);

        Map.Add(new KeymapArgument("PageDown")
        {
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollPageDown);

        Map.Add(new KeymapArgument("PageUp")
        {
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollPageUp);
    }
}
