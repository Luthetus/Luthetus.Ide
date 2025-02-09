using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public class ContextService : IContextService
{
	private ContextState _contextState = new();
    
    public event Action? ContextStateChanged;
    
    public ContextState GetContextState() => _contextState;
    
    public void ReduceSetFocusedContextHeirarchyAction(ContextHeirarchy focusedContextHeirarchy)
    {
    	var inState = GetContextState();
    	
        _contextState = inState with
        {
            FocusedContextHeirarchy = focusedContextHeirarchy
        };
        
        ContextStateChanged?.Invoke();
        return;
    }
    
    public void ReduceToggleSelectInspectedContextHeirarchyAction()
    {
    	var inState = GetContextState();
    	
        var outIsSelectingInspectionTarget = !inState.IsSelectingInspectionTarget;

        var outInspectableContextList = inState.InspectableContextList;
        var outInspectedContextHeirarchy = inState.InspectedContextHeirarchy;

        if (!outIsSelectingInspectionTarget)
        {
            inState.InspectableContextList.Clear();
            outInspectedContextHeirarchy = null;
        }

        _contextState = inState with
        {
            IsSelectingInspectionTarget = !inState.IsSelectingInspectionTarget,
            InspectableContextList = outInspectableContextList,
            InspectedContextHeirarchy = outInspectedContextHeirarchy,
        };
        
        ContextStateChanged?.Invoke();
        return;
    }
    
    public void ReduceIsSelectingInspectableContextHeirarchyAction(bool value)
    {
    	var inState = GetContextState();
    	
        if (value)
        {
            _contextState = inState with
            {
                IsSelectingInspectionTarget = true
            };
            
            ContextStateChanged?.Invoke();
        	return;
        }
        else
        {
        	inState.InspectableContextList.Clear();
        	
            _contextState = inState with
            {
                IsSelectingInspectionTarget = false,
                InspectedContextHeirarchy = null,
            };
            
            ContextStateChanged?.Invoke();
	        return;
        }
    }
    
    public void ReduceSetInspectedContextHeirarchyAction(ContextHeirarchy? inspectedContextHeirarchy)
    {
    	var inState = GetContextState();
    	
    	inState.InspectableContextList.Clear();
    
        _contextState = inState with
        {
            IsSelectingInspectionTarget = false,
            InspectedContextHeirarchy = inspectedContextHeirarchy,
        };
        
        ContextStateChanged?.Invoke();
        return;
    }
    
    public void ReduceAddInspectableContextAction(InspectableContext inspectableContext)
    {
    	var inState = GetContextState();
    	
        inState.InspectableContextList.Add(inspectableContext);

        _contextState = inState with { };
        
        ContextStateChanged?.Invoke();
        return;
    }
    
    public void ReduceSetContextKeymapAction(Key<ContextRecord> contextKey, IKeymap keymap)
    {
    	var inState = GetContextState();
    	
        var inContextRecord = inState.AllContextsList.FirstOrDefault(
            x => x.ContextKey == contextKey);

        if (inContextRecord != default)
        {
            _contextState = inState;
            ContextStateChanged?.Invoke();
        	return;
        }
            
        var index = inState.AllContextsList.FindIndex(x => x.ContextKey == inContextRecord.ContextKey);
        if (index == -1)
        {
        	_contextState = inState;
        	ContextStateChanged?.Invoke();
        	return;
        }

        inState.AllContextsList[index] = inContextRecord with
        {
            Keymap = keymap
        };

        _contextState = inState with { };
        ContextStateChanged?.Invoke();
        return;
    }
}
