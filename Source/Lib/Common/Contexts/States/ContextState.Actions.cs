using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Contexts.States;

public partial record ContextState
{
    public record struct SetFocusedContextHeirarchyAction(ContextHeirarchy FocusedContextHeirarchy);
    public record struct ToggleSelectInspectedContextHeirarchyAction;
    public record struct IsSelectingInspectableContextHeirarchyAction(bool Value);
    public record struct SetInspectedContextHeirarchyAction(ContextHeirarchy? InspectedContextHeirarchy);
    public record struct AddInspectableContextAction(InspectableContext InspectableContext);
    public record struct SetContextKeymapAction(Key<ContextRecord> ContextKey, IKeymap Keymap);
}
