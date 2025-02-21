using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;
    
public struct TryShowViewModelArgs
{
    public TryShowViewModelArgs(
        Key<TextEditorViewModel> viewModelKey,
        Key<TextEditorGroup> groupKey,
        bool shouldSetFocusToEditor,
        IServiceProvider serviceProvider)
    {
        ViewModelKey = viewModelKey;
        GroupKey = groupKey;
        ShouldSetFocusToEditor = shouldSetFocusToEditor;
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// The identifier for which view model is to be used.
    /// </summary>
    public Key<TextEditorViewModel> ViewModelKey { get; }
    /// <summary>
    /// If this view model should be rendered as a tab within a group then provide this key.
    /// Otherwise pass in <see cref="Key{T}.Empty"/>
    /// </summary>
    public Key<TextEditorGroup> GroupKey { get; }
    public bool ShouldSetFocusToEditor { get; }
    public IServiceProvider ServiceProvider { get; }
}
