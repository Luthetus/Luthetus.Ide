namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public class AutocompleteEntryTests
{
    public AutocompleteEntryTests(string displayName, AutocompleteEntryKind autocompleteEntryKind)
    {
        DisplayName = displayName;
        AutocompleteEntryKind = autocompleteEntryKind;
    }

    public string DisplayName { get; }
    public AutocompleteEntryKind AutocompleteEntryKind { get; }
}
