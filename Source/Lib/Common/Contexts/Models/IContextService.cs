using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public interface IContextService
{
	public event Action? ContextStateChanged;
    
    public ContextState GetContextState();
    
    public ContextRecord GetContextRecord(Key<ContextRecord> contextKey);
    
    public ContextSwitchState GetContextSwitchState();
    
    public void SetFocusedContextHeirarchy(ContextHeirarchy focusedContextHeirarchy);
    public void ToggleSelectInspectedContextHeirarchy();
    public void IsSelectingInspectableContextHeirarchy(bool value);
    public void SetInspectedContextHeirarchy(ContextHeirarchy? inspectedContextHeirarchy);
    public void AddInspectableContext(InspectableContext inspectableContext);
    public void SetContextKeymap(Key<ContextRecord> contextKey, IKeymap keymap);
    
    public void RegisterContextSwitchGroup(ContextSwitchGroup contextSwitchGroup);
}
