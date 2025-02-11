using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public class ContextService : IContextService
{
	private readonly object _stateModificationLock = new();
	
	private ContextState _contextState = new();
	private ContextSwitchState _contextSwitchState = new();
    
    public event Action? ContextStateChanged;
    public event Action? ContextSwitchStateChanged;
    
    public ContextState GetContextState() => _contextState;
    
    public ContextRecord GetContextRecord(Key<ContextRecord> contextKey) =>
    	_contextState.AllContextsList.FirstOrDefault(x => x.ContextKey == contextKey);
    
    public ContextSwitchState GetContextSwitchState() => _contextSwitchState;
    
    public void ReduceSetFocusedContextHeirarchyAction(ContextHeirarchy focusedContextHeirarchy)
    {
    	lock (_stateModificationLock)
    	{
    		var inState = GetContextState();
    	
	        _contextState = inState with
	        {
	            FocusedContextHeirarchy = focusedContextHeirarchy
	        };
	        
	        ContextStateChanged?.Invoke();
	        return;
    	}
    }
    
    public void ReduceToggleSelectInspectedContextHeirarchyAction()
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetContextState();
	    	
	        var outIsSelectingInspectionTarget = !inState.IsSelectingInspectionTarget;
	
	        var outInspectableContextList = inState.InspectableContextList;
	        var outInspectedContextHeirarchy = inState.InspectedContextHeirarchy;
	
	        if (!outIsSelectingInspectionTarget)
	        {
	            outInspectableContextList = new();
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
    }
    
    public void ReduceIsSelectingInspectableContextHeirarchyAction(bool value)
    {
    	lock (_stateModificationLock)
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
	        	var outInspectableContextList = new List<InspectableContext>();
	        	
	            _contextState = inState with
	            {
	                IsSelectingInspectionTarget = false,
	                InspectedContextHeirarchy = null,
	                InspectableContextList = outInspectableContextList,
	            };
	            
	            ContextStateChanged?.Invoke();
		        return;
	        }
	    }
    }
    
    public void ReduceSetInspectedContextHeirarchyAction(ContextHeirarchy? inspectedContextHeirarchy)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetContextState();
	    	
	    	var outInspectableContextList = new List<InspectableContext>();
	    
	        _contextState = inState with
	        {
	            IsSelectingInspectionTarget = false,
	            InspectedContextHeirarchy = inspectedContextHeirarchy,
	            InspectableContextList = outInspectableContextList,
	        };
	        
	        ContextStateChanged?.Invoke();
	        return;
	    }
    }
    
    public void ReduceAddInspectableContextAction(InspectableContext inspectableContext)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetContextState();
	
	    	var outInspectableContextList = new List<InspectableContext>(inState.InspectableContextList);
	        outInspectableContextList.Add(inspectableContext);
	
	        _contextState = inState with
	        {
	        	InspectableContextList = outInspectableContextList
	        };
	        
	        ContextStateChanged?.Invoke();
	        return;
	    }
    }
    
    public void ReduceSetContextKeymapAction(Key<ContextRecord> contextKey, IKeymap keymap)
    {
    	lock (_stateModificationLock)
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
	
    public void ReduceRegisterContextSwitchGroupAction(ContextSwitchGroup contextSwitchGroup)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetContextSwitchState();
	    
	    	if (inState.ContextSwitchGroupList.Any(x =>
	    			x.Key == contextSwitchGroup.Key))
	    	{
	    		ContextSwitchStateChanged?.Invoke();
	        	return;
	    	}
	    
	    	var outContextSwitchGroupList = new List<ContextSwitchGroup>(inState.ContextSwitchGroupList);
	    	outContextSwitchGroupList.Add(contextSwitchGroup);
	    
	        _contextSwitchState = inState with
	        {
	            ContextSwitchGroupList = outContextSwitchGroupList
	        };
	        
	        ContextSwitchStateChanged?.Invoke();
	        return;
	    }
    }
}
