using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public class WordAutocompleteIndexer : IAutocompleteIndexer
{
    private readonly ConcurrentBag<string> _indexedStringsBag = new();

    public ImmutableArray<string> IndexedStringsBag => _indexedStringsBag.ToImmutableArray();

    public Task IndexTextEditorAsync(TextEditorModel textEditorModel)
    {
        return Task.CompletedTask;
    }

    public Task IndexWordAsync(string word)
    {
        if (!_indexedStringsBag.Contains(word))
            _indexedStringsBag.Add(word);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // The word autocomplete indexer does not make use of the Dispose() method.
    }
}