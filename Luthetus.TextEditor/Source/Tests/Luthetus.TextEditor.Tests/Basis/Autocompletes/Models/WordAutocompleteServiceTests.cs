namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public class WordAutocompleteServiceTests : IAutocompleteService
{
    private readonly IAutocompleteIndexer _autocompleteIndexer;

    public WordAutocompleteService(IAutocompleteIndexer autocompleteIndexer)
    {
        _autocompleteIndexer = autocompleteIndexer;
    }

    public List<string> GetAutocompleteOptions(string word)
    {
        var indexedStrings = _autocompleteIndexer.IndexedStringsBag;

        return new List<string>(indexedStrings.Where(x => x.StartsWith(word)).Take(5));
    }
}
