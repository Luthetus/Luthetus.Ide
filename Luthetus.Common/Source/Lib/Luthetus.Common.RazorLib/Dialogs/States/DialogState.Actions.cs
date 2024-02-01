using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.States;

public partial record DialogState
{
    public record RegisterAction(DialogRecord Entry);
    public record DisposeAction(Key<DialogRecord> Key);
    public record SetIsMaximizedAction(Key<DialogRecord> Key, bool IsMaximized);
    public record SetActiveDialogKeyAction(Key<DialogRecord> DialogKey);
}