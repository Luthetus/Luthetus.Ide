using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

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
