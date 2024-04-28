using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Contexts.States;

public partial record ContextState
{
    public record SetFocusedContextHeirarchyAction(ContextHeirarchy FocusedContextHeirarchy);
    public record ToggleSelectInspectedContextHeirarchyAction;
    public record IsSelectingInspectableContextHeirarchyAction(bool Value);
    public record SetInspectedContextHeirarchyAction(ContextHeirarchy? InspectedContextHeirarchy);
    public record AddInspectableContextAction(InspectableContext InspectableContext);
    public record SetContextKeymapAction(Key<ContextRecord> ContextKey, Keymap Keymap);
}
