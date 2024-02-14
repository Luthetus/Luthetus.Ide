using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public class SearchEngineOverRenderLinkedViewModels : ITextEditorSearchEngine
{
    public Key<ITextEditorSearchEngine> Key { get; } =
        new Key<ITextEditorSearchEngine>(Guid.Parse("9bdad472-04eb-488b-88cc-e1b6e3686399"));

    public Type IconComponentRendererType { get; } = typeof(IconArrowDown);
    public string DisplayName { get; } = "Rendered ViewModels";

    public async Task SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        await Task.Delay(3_000).ConfigureAwait(false);
    }
}