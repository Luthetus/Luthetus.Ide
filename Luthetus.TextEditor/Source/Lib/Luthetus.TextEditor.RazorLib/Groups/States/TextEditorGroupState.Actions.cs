using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Groups.States;

public partial class TextEditorGroupState
{
    public record RegisterAction(TextEditorGroup Group);
    public record DisposeAction(Key<TextEditorGroup> GroupKey);

    public record AddViewModelToGroupAction(
        Key<TextEditorGroup> GroupKey,
        Key<TextEditorViewModel> ViewModelKey);
    
    public record RemoveViewModelFromGroupAction(
        Key<TextEditorGroup> GroupKey,
        Key<TextEditorViewModel> ViewModelKey);
    
    public record SetActiveViewModelOfGroupAction(
        Key<TextEditorGroup> GroupKey,
        Key<TextEditorViewModel> ViewModelKey);
}