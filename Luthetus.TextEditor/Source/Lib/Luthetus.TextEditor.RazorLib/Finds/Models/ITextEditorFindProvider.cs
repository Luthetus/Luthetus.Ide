using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.Finds.Models;

public interface ITextEditorFindProvider
{
    public Key<ITextEditorFindProvider> FindProviderKey { get; }
    public Type IconComponentRendererType { get; }
    public string DisplayName { get; }

    public Task SearchAsync(string searchQuery, CancellationToken cancellationToken = default);
}