using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public class WordAutocompleteIndexer : IAutocompleteIndexer
{
    private readonly ITextEditorService _textEditorService;
    private readonly ConcurrentBag<string> _indexedStringsBag = new();

    public WordAutocompleteIndexer(ITextEditorService textEditorService)
    {
        _textEditorService = textEditorService;

        _textEditorService.ModelStateWrap.StateChanged += ModelsCollectionWrapOnStateChanged;
    }

    private void ModelsCollectionWrapOnStateChanged(object? sender, EventArgs e)
    {
        // TODO: When should the indexer re-index or incrementally do so
    }

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
        _textEditorService.ModelStateWrap.StateChanged -= ModelsCollectionWrapOnStateChanged;
    }
}