using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public class WordAutocompleteIndexer : IAutocompleteIndexer
{
    private readonly ConcurrentBag<string> _indexedStringsList = new();

    public ImmutableArray<string> IndexedStringsList => _indexedStringsList.ToImmutableArray();

    public Task IndexTextEditorAsync(TextEditorModel textEditorModel)
    {
        return Task.CompletedTask;
    }

    public Task IndexWordAsync(string word)
    {
        if (!_indexedStringsList.Contains(word))
            _indexedStringsList.Add(word);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // The word autocomplete indexer does not make use of the Dispose() method.
    }
}