namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

// TODO: Add a context parameter to narrow results?
public interface IAutocompleteService
{
    /// <summary>
    /// <see cref="GetAutocompleteOptions"/> passes
    /// in as a parameter the contiguous letters or digits
    /// that are to the left of the cursor.
    /// <br/><br/>
    /// If the cursor is in the middle of a word, only
    /// the letters or digits to the left of the cursor
    /// are provided.
    /// <br/><br/>
    /// If the text immediately to the left of the cursor is
    /// not a letter or digit, then <see cref="string.Empty"/> is returned.
    /// <br/><br/>
    /// If the cursor is at the start of the document
    /// (rowIndex 0 and columnIndex 0),
    /// then <see cref="string.Empty"/> is returned.
    /// </summary>
    public List<string> GetAutocompleteOptions(string word);
}
