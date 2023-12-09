using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.Finds.Models;

public class RegisteredViewModelsFindProvider : ITextEditorFindProvider
{
    public Key<ITextEditorFindProvider> FindProviderKey { get; } =
        new Key<ITextEditorFindProvider>(Guid.Parse("8f82c804-7813-44ea-869a-f77574f2f945"));

    public Type IconComponentRendererType { get; } = typeof(IconCopy);
    public string DisplayName { get; } = "Registered ViewModels";

    public async Task SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        await Task.Delay(5_000);
    }
}