using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;

namespace Luthetus.Common.RazorLib.Dialogs.States;

public partial record DialogState
{
    public record RegisterAction(IPolymorphicDialog Entry);
    public record DisposeAction(Key<IPolymorphicUiRecord> PolymorphicUiKey);
    public record SetIsMaximizedAction(Key<IPolymorphicUiRecord> PolymorphicUiKey, bool IsMaximized);
    public record SetActiveDialogKeyAction(Key<IPolymorphicUiRecord> PolymorphicUiKey);
}