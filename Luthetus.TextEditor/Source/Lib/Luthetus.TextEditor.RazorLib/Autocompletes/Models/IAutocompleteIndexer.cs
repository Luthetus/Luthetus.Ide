using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Autocompletes.Models;

public interface IAutocompleteIndexer : IDisposable
{
    public ImmutableArray<string> IndexedStringsBag { get; }

    public Task IndexTextEditorAsync(TextEditorModel textEditorModel);
    public Task IndexWordAsync(string word);
}