namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public class AutocompleteEntry
{
    public AutocompleteEntry(
        string displayName,
        AutocompleteEntryKind autocompleteEntryKind,
        Func<Task>? sideEffectFunc)
    {
        DisplayName = displayName;
        AutocompleteEntryKind = autocompleteEntryKind;
        SideEffectFunc = sideEffectFunc;
    }

    public string DisplayName { get; }
    public AutocompleteEntryKind AutocompleteEntryKind { get; }
    public Func<Task>? SideEffectFunc { get; }
}
