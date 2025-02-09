using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public interface IContextService
{
	public event Action? ContextStateChanged;
    
    public ContextState GetContextState();
    
    public void ReduceSetFocusedContextHeirarchyAction(ContextHeirarchy focusedContextHeirarchy);
    public void ReduceToggleSelectInspectedContextHeirarchyAction();
    public void ReduceIsSelectingInspectableContextHeirarchyAction(bool value);
    public void ReduceSetInspectedContextHeirarchyAction(ContextHeirarchy? inspectedContextHeirarchy);
    public void ReduceAddInspectableContextAction(InspectableContext inspectableContext);
    public void ReduceSetContextKeymapAction(Key<ContextRecord> contextKey, IKeymap keymap);
}
