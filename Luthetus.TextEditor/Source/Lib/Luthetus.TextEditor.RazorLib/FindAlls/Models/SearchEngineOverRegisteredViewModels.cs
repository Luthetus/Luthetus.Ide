using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public class SearchEngineOverRegisteredViewModels : ITextEditorSearchEngine
{
    public Key<ITextEditorSearchEngine> Key { get; } =
        new Key<ITextEditorSearchEngine>(Guid.Parse("8f82c804-7813-44ea-869a-f77574f2f945"));

    public Type IconComponentRendererType { get; } = typeof(IconCopy);
    public string DisplayName { get; } = "Registered ViewModels";

    public async Task SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        await Task.Delay(5_000).ConfigureAwait(false);
    }
}