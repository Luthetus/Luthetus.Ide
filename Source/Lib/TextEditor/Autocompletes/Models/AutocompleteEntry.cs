namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public class AutocompleteEntry
{
    public AutocompleteEntry(
        string displayName,
        AutocompleteEntryKind autocompleteEntryKind,
        Action? sideEffectAction)
    {
        DisplayName = displayName;
        AutocompleteEntryKind = autocompleteEntryKind;
        SideEffectAction = sideEffectAction;
    }

    public string DisplayName { get; }
    public AutocompleteEntryKind AutocompleteEntryKind { get; }
    public Action? SideEffectAction { get; }
}
