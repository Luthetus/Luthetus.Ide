using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.States;

public partial record DialogState
{
    public record RegisterAction(IDialogViewModel Dialog);
    public record DisposeAction(Key<IDialogViewModel> DialogKey);
    public record SetIsMaximizedAction(Key<IDialogViewModel> DialogKey, bool IsMaximized);
    public record SetActiveDialogKeyAction(Key<IDialogViewModel> DialogKey);
}