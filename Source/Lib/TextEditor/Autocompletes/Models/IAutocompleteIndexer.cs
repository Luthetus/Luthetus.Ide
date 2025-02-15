using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Concurrent;

namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public interface IAutocompleteIndexer : IDisposable
{
    public ConcurrentBag<string> IndexedStringsList { get; }

    public Task IndexTextEditorAsync(TextEditorModel textEditorModel);
    public Task IndexWordAsync(string word);
}