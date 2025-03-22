using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public struct FastParseArgs
{
    public FastParseArgs(
        ResourceUri resourceUri,
        string extensionNoPeriod,
        IServiceProvider serviceProvider)
    {
        ResourceUri = resourceUri;
        ExtensionNoPeriod = extensionNoPeriod;
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// The unique identifier for the <see cref="TextEditorModel"/> which is
    /// providing the underlying data to be rendered by this view model.
    /// </summary>
    public ResourceUri ResourceUri { get; }
    public string ExtensionNoPeriod { get; }
    public IServiceProvider ServiceProvider { get; }
    public bool ShouldBlockUntilBackgroundTaskIsCompleted { get; init; }
}
