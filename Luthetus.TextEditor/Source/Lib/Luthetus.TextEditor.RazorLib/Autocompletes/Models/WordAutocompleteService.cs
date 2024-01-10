namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public class WordAutocompleteService : IAutocompleteService
{
    private readonly WordAutocompleteIndexer _wordAutocompleteIndexer;

    public WordAutocompleteService(WordAutocompleteIndexer wordAutocompleteIndexer)
    {
        _wordAutocompleteIndexer = wordAutocompleteIndexer;
    }

    public List<string> GetAutocompleteOptions(string word)
    {
        var indexedStrings = _wordAutocompleteIndexer.IndexedStringsList;
        return new List<string>(indexedStrings.Where(x => x.StartsWith(word)).Take(5));
    }
}
