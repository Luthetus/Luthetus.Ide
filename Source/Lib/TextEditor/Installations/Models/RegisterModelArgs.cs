using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public class RegisterModelArgs
{
    public RegisterModelArgs(
        ResourceUri resourceUri,
        IServiceProvider serviceProvider)
    {
        ResourceUri = resourceUri;
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// The unique identifier for the <see cref="TextEditorModel"/> which is
    /// providing the underlying data to be rendered by this view model.
    /// </summary>
    public ResourceUri ResourceUri { get; }
    public IServiceProvider ServiceProvider { get; }
}
