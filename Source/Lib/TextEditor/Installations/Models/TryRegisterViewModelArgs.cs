using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public struct TryRegisterViewModelArgs
{
    public TryRegisterViewModelArgs(
        Key<TextEditorViewModel> viewModelKey,
        ResourceUri resourceUri,
        Category category,
        bool shouldSetFocusToEditor,
        IServiceProvider serviceProvider)
    {
        ViewModelKey = viewModelKey;
        ResourceUri = resourceUri;
        Category = category;
        ShouldSetFocusToEditor = shouldSetFocusToEditor;
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// One can use <see cref="Key{T}.NewKey()"/> if they have no preference for the key value.
    /// <br/><br/>
    /// The identifier for the view model which is used by most of the API in
    /// <see cref="TextEditors.Models.TextEditorServices.ITextEditorService"/>.
    /// </summary>
    public Key<TextEditorViewModel> ViewModelKey { get; }
    /// <summary>
    /// The unique identifier for the <see cref="TextEditorModel"/> which is
    /// providing the underlying data to be rendered by this view model.
    /// </summary>
    public ResourceUri ResourceUri { get; }
    /// <summary>
    /// <inheritdoc cref="TextEditors.Models.Category"/>
    /// </summary>
    public Category Category { get; }
    /// <summary>
    /// When this view model next gets rendered should browser focus be set to the text editor.
    /// </summary>
    public bool ShouldSetFocusToEditor { get; }
    public IServiceProvider ServiceProvider { get; }
}
