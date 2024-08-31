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

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;

public class TextEditorKeymapDefault : Keymap, ITextEditorKeymap
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<KeymapArgs, CommandNoType> _map = new();

    public TextEditorKeymapDefault()
        : base(new Key<Keymap>(Guid.Parse("4aaca759-c2c7-4e6f-9d9f-f3d17172df16")),
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

        _map.Add(new KeymapArgs()
        {
            Key = "x",
            Code = "KeyX",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Cut);

        _map.Add(new KeymapArgs()
        {
            Key = "c",
            Code = "KeyC",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Copy);

        _map.Add(new KeymapArgs()
        {
            Key = "v",
            Code = "KeyV",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.PasteCommand);

        _map.Add(new KeymapArgs()
        {
            Key = "s",
            Code = "KeyS",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.TriggerSave);

        _map.Add(new KeymapArgs()
        {
            Key = "a",
            Code = "KeyA",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.SelectAll);

        _map.Add(new KeymapArgs()
        {
            Key = "z",
            Code = "KeyZ",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Undo);

        _map.Add(new KeymapArgs()
        {
            Key = "y",
            Code = "KeyY",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Redo);

        _map.Add(new KeymapArgs()
        {
            Key = "d",
            Code = "KeyD",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Duplicate);

        _map.Add(new KeymapArgs()
        {
            Key = "]",
            Code = "BracketRight",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(false));

        _map.Add(new KeymapArgs()
        {
            Key = "}",
            Code = "BracketRight",
            CtrlKey = true,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(true));
        
        _map.Add(new KeymapArgs()
        {
            Key = "Tab",
            Code = "Tab",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        
        _map.Add(new KeymapArgs()
        {
            Key = "Tab",
            Code = "Tab",
            CtrlKey = true,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        
        _map.Add(new KeymapArgs()
        {
            Key = "p",
            Code = "KeyP",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);

        _map.Add(new KeymapArgs()
        {
            Key = "F",
            Code = "KeyF",
            CtrlKey = true,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowFindAllDialog);

        _map.Add(new KeymapArgs()
        {
            Key = "ArrowDown",
            Code = "ArrowDown",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollLineDown);

        _map.Add(new KeymapArgs()
        {
            Key = "ArrowUp",
            Code = "ArrowUp",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollLineUp);

        _map.Add(new KeymapArgs()
        {
            Key = "PageDown",
            Code = "PageDown",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.CursorMovePageBottom);

        _map.Add(new KeymapArgs()
        {
            Key = "PageUp",
            Code = "PageUp",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.CursorMovePageTop);

        _map.Add(new KeymapArgs()
        {
            Key = "Enter",
            Code = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.NewLineAbove);
        
        _map.Add(new KeymapArgs()
        {
            Key = " ",
            Code = KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE,
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        
        _map.Add(new KeymapArgs()
        {
            Key = ",",
            Code = "Comma",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        
        _map.Add(new KeymapArgs()
        {
            Key = "/",
            Code = "Slash",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowTooltipByCursorPosition);
        
        _map.Add(new KeymapArgs()
        {
            Key = "f",
            Code = "KeyF",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowFindOverlay);
        
        _map.Add(new KeymapArgs()
        {
            Key = "r",
            Code = "KeyR",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.RefreshSyntaxHighlighting);
        
        _map.Add(new KeymapArgs()
        {
            Key = ".",
            Code = "Period",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.QuickActionsSlashRefactor);
        
        _map.Add(new KeymapArgs()
        {
            Key = "j",
            Code = "KeyJ",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DEBUG_BreakLineEndings);
    }

    private void AddDefaultAltModifiedKeymap()
    {
    	_map.Add(new KeymapArgs()
        {
            Key = "ArrowDown",
            Code = "ArrowDown",
            AltKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.MoveLineDown);
    
        _map.Add(new KeymapArgs()
        {
            Key = "ArrowUp",
            Code = "ArrowUp",
            AltKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.MoveLineUp);
    }

    private void AddDefaultHasSelectionLayerModifiedKeymap()
    {
        _map.Add(new KeymapArgs()
        {
            Key = "Tab",
            Code = KeyboardKeyFacts.WhitespaceCodes.TAB_CODE,
            LayerKey = TextEditorKeymapDefaultFacts.HasSelectionLayer.Key
        }, TextEditorCommandDefaultFacts.IndentMore);

        _map.Add(new KeymapArgs()
        {
            Key = "Tab",
            Code = KeyboardKeyFacts.WhitespaceCodes.TAB_CODE,
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

            _ = _map.TryAdd(new KeymapArgs()
            {
                Key = $"{character}",
                Code = $"Key{character}",
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

            _ = _map.TryAdd(new KeymapArgs()
            {
                Key = $"{character}",
                Code = $"Key{char.ToUpperInvariant(character)}",
                CtrlKey = true,
                AltKey = true,
                LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
            }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        }
    }

    private void AddDefaultMiscKeymap()
    {
    	_map.Add(new KeymapArgs()
        {
            Key = "F7",
            Code = "F7",
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.RelatedFilesQuickPick);
    
        _map.Add(new KeymapArgs()
        {
            Key = "F12",
            Code = "F12",
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToDefinition);

        _map.Add(new KeymapArgs()
        {
            Key = "Enter",
            Code = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.NewLineBelow);

        _map.Add(new KeymapArgs()
        {
            Key = "PageDown",
            Code = "PageDown",
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollPageDown);

        _map.Add(new KeymapArgs()
        {
            Key = "PageUp",
            Code = "PageUp",
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollPageUp);
    }

	public bool TryMap(KeyboardEventArgs keyboardEventArgs, KeymapArgs keymapArgument, TextEditorComponentData componentData, out CommandNoType? command)
	{
        return _map.TryGetValue(keymapArgument, out command);
	}
}
