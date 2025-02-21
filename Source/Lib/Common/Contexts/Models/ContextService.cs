using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.ListExtensions;

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
    
    public void SetFocusedContextHeirarchy(ContextHeirarchy focusedContextHeirarchy)
    {
    	lock (_stateModificationLock)
    	{
    		var inState = GetContextState();
    	
	        _contextState = inState with
	        {
	            FocusedContextHeirarchy = focusedContextHeirarchy
	        };

			goto finalize;
    	}

		finalize:
        ContextStateChanged?.Invoke();
    }
    
    public void ToggleSelectInspectedContextHeirarchy()
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetContextState();
	    	
	        var outIsSelectingInspectionTarget = !inState.IsSelectingInspectionTarget;
	
	        var outInspectableContextList = inState.InspectableContextList;
	        var outInspectedContextHeirarchy = inState.InspectedContextHeirarchy;
	
	        if (!outIsSelectingInspectionTarget)
	        {
	            outInspectableContextList = new List<InspectableContext>();
	            outInspectedContextHeirarchy = null;
	        }
	
	        _contextState = inState with
	        {
	            IsSelectingInspectionTarget = !inState.IsSelectingInspectionTarget,
	            InspectableContextList = outInspectableContextList,
	            InspectedContextHeirarchy = outInspectedContextHeirarchy,
	        };
	        
	        goto finalize;
	    }

        finalize:
        ContextStateChanged?.Invoke();
    }

    public void IsSelectingInspectableContextHeirarchy(bool value)
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

                goto finalize;
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

				goto finalize;
	        }
	    }

        finalize:
        ContextStateChanged?.Invoke();
    }
    
    public void SetInspectedContextHeirarchy(ContextHeirarchy? inspectedContextHeirarchy)
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

			goto finalize;
        }

        finalize:
        ContextStateChanged?.Invoke();
    }
    
    public void AddInspectableContext(InspectableContext inspectableContext)
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
	        
	        goto finalize;
	    }

        finalize:
        ContextStateChanged?.Invoke();
    }
    
    public void SetContextKeymap(Key<ContextRecord> contextKey, IKeymap keymap)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetContextState();
	    	
	        var inContextRecord = inState.AllContextsList.FirstOrDefault(
	            x => x.ContextKey == contextKey);
	
	        if (inContextRecord != default)
	        {
	            _contextState = inState;
				goto finalize;
            }
	            
	        var index = inState.AllContextsList.FindIndex(x => x.ContextKey == inContextRecord.ContextKey);
	        if (index == -1)
	        {
	        	_contextState = inState;
                goto finalize;
            }
            
            var outAllContextsList = new List<ContextRecord>(inState.AllContextsList);
	
	        outAllContextsList[index] = inContextRecord with
	        {
	            Keymap = keymap
	        };
	
	        _contextState = inState with { AllContextsList = outAllContextsList };
            goto finalize;
        }

        finalize:
        ContextStateChanged?.Invoke();
    }
	
    public void RegisterContextSwitchGroup(ContextSwitchGroup contextSwitchGroup)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetContextSwitchState();
	    
	    	if (inState.ContextSwitchGroupList.Any(x =>
	    			x.Key == contextSwitchGroup.Key))
	    	{
                goto finalize;
            }
	    
	    	var outContextSwitchGroupList = new List<ContextSwitchGroup>(inState.ContextSwitchGroupList);
	    	outContextSwitchGroupList.Add(contextSwitchGroup);
	    
	        _contextSwitchState = inState with
	        {
	            ContextSwitchGroupList = outContextSwitchGroupList
	        };
	        
	        goto finalize;
	    }

        finalize:
        ContextStateChanged?.Invoke();
    }
}
