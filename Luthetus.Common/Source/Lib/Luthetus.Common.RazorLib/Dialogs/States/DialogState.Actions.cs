using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.States;

public partial record DialogState
{
    public record RegisterAction(IDialog Dialog);
    public record DisposeAction(Key<IDynamicViewModel> DynamicViewModelKey);
    public record SetIsMaximizedAction(Key<IDynamicViewModel> DynamicViewModelKey, bool IsMaximized);
    public record SetActiveDialogKeyAction(Key<IDynamicViewModel> DynamicViewModelKey);
}