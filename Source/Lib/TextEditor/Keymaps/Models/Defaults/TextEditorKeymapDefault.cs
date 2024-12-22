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
        TryRegister(new KeymapArgs()
        {
            Key = "x",
            Code = "KeyX",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Cut);

        TryRegister(new KeymapArgs()
        {
            Key = "c",
            Code = "KeyC",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Copy);

        TryRegister(new KeymapArgs()
        {
            Key = "v",
            Code = "KeyV",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.PasteCommand);

        TryRegister(new KeymapArgs()
        {
            Key = "s",
            Code = "KeyS",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.TriggerSave);

        TryRegister(new KeymapArgs()
        {
            Key = "a",
            Code = "KeyA",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.SelectAll);

        TryRegister(new KeymapArgs()
        {
            Key = "z",
            Code = "KeyZ",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Undo);

        TryRegister(new KeymapArgs()
        {
            Key = "y",
            Code = "KeyY",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Redo);

        TryRegister(new KeymapArgs()
        {
            Key = "d",
            Code = "KeyD",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.Duplicate);

        TryRegister(new KeymapArgs()
        {
            Key = "]",
            Code = "BracketRight",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(false));

        TryRegister(new KeymapArgs()
        {
            Key = "}",
            Code = "BracketRight",
            CtrlKey = true,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(true));
        
        TryRegister(new KeymapArgs()
        {
            Key = "Tab",
            Code = "Tab",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);
        
        TryRegister(new KeymapArgs()
        {
            Key = "Tab",
            Code = "Tab",
            CtrlKey = true,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DoNothingDiscard);

        TryRegister(new KeymapArgs()
        {
            Key = "ArrowDown",
            Code = "ArrowDown",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollLineDown);

        TryRegister(new KeymapArgs()
        {
            Key = "ArrowUp",
            Code = "ArrowUp",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollLineUp);

        TryRegister(new KeymapArgs()
        {
            Key = "PageDown",
            Code = "PageDown",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.CursorMovePageBottom);

        TryRegister(new KeymapArgs()
        {
            Key = "PageUp",
            Code = "PageUp",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.CursorMovePageTop);

        TryRegister(new KeymapArgs()
        {
            Key = "Enter",
            Code = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.NewLineAbove);
        
        TryRegister(new KeymapArgs()
        {
            Key = "/",
            Code = "Slash",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowTooltipByCursorPosition);
        
        TryRegister(new KeymapArgs()
        {
            Key = "f",
            Code = "KeyF",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowFindOverlay);
        
        TryRegister(new KeymapArgs()
        {
            Key = "F",
            Code = "KeyF",
            CtrlKey = true,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.PopulateSearchFindAll);
        
        TryRegister(new KeymapArgs()
        {
            Key = "r",
            Code = "KeyR",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.RefreshSyntaxHighlighting);
        
        TryRegister(new KeymapArgs()
        {
            Key = " ",
            Code = "Space",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ShowAutocompleteMenu);
        
        TryRegister(new KeymapArgs()
        {
            Key = ".",
            Code = "Period",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.QuickActionsSlashRefactor);
        
        TryRegister(new KeymapArgs()
        {
            Key = "j",
            Code = "KeyJ",
            CtrlKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.DEBUG_BreakLineEndings);
    }

    private void AddDefaultAltModifiedKeymap()
    {
    	TryRegister(new KeymapArgs()
        {
            Key = "ArrowDown",
            Code = "ArrowDown",
            AltKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.MoveLineDown);
    
        TryRegister(new KeymapArgs()
        {
            Key = "ArrowUp",
            Code = "ArrowUp",
            AltKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.MoveLineUp);
    }

    private void AddDefaultHasSelectionLayerModifiedKeymap()
    {
        TryRegister(new KeymapArgs()
        {
            Key = "Tab",
            Code = KeyboardKeyFacts.WhitespaceCodes.TAB_CODE,
            LayerKey = TextEditorKeymapDefaultFacts.HasSelectionLayer.Key
        }, TextEditorCommandDefaultFacts.IndentMore);

        TryRegister(new KeymapArgs()
        {
            Key = "Tab",
            Code = KeyboardKeyFacts.WhitespaceCodes.TAB_CODE,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.HasSelectionLayer.Key
        }, TextEditorCommandDefaultFacts.IndentLess);
    }

    private void AddDefaultMiscKeymap()
    {
    	TryRegister(new KeymapArgs()
        {
            Key = "F7",
            Code = "F7",
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.RelatedFilesQuickPick);
    
        TryRegister(new KeymapArgs()
        {
            Key = "F12",
            Code = "F12",
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.GoToDefinition);

        TryRegister(new KeymapArgs()
        {
            Key = "Enter",
            Code = KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE,
            ShiftKey = true,
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.NewLineBelow);

        TryRegister(new KeymapArgs()
        {
            Key = "PageDown",
            Code = "PageDown",
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollPageDown);

        TryRegister(new KeymapArgs()
        {
            Key = "PageUp",
            Code = "PageUp",
            LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key
        }, TextEditorCommandDefaultFacts.ScrollPageUp);
    }

	public bool TryMap(KeymapArgs keymapArgument, TextEditorComponentData componentData, out CommandNoType? command)
	{
        return MapFirstOrDefault(keymapArgument, out command);
    }
}
