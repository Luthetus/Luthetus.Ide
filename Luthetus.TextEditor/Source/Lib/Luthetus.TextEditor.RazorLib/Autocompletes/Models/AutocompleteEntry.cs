namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public class AutocompleteEntry
{
    public AutocompleteEntry(string displayName, AutocompleteEntryKind autocompleteEntryKind)
    {
        DisplayName = displayName;
        AutocompleteEntryKind = autocompleteEntryKind;
    }

    public string DisplayName { get; }
    public AutocompleteEntryKind AutocompleteEntryKind { get; }
}
