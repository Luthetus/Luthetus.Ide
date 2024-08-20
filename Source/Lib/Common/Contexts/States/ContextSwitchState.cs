using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;

namespace Luthetus.Common.RazorLib.Contexts.States;

[FeatureState]
public partial record ContextSwitchState(ImmutableList<ContextSwitchGroup> ContextSwitchGroupList)
{
	public ContextSwitchState() : this(ImmutableList<ContextSwitchGroup>.Empty)
	{
	}

	public record RegisterContextSwitchGroupAction(ContextSwitchGroup ContextSwitchGroup);
	
	public class Reducer
    {
        [ReducerMethod]
        public static ContextSwitchState ReduceRegisterContextSwitchGroupAction(
            ContextSwitchState inState,
            RegisterContextSwitchGroupAction registerContextSwitchGroupAction)
        {
        	if (inState.ContextSwitchGroupList.Any(x =>
        			x.Key == registerContextSwitchGroupAction.ContextSwitchGroup.Key))
        	{
        		return inState;
        	}
        
        	var outContextSwitchGroupList = inState.ContextSwitchGroupList.Add(
        		registerContextSwitchGroupAction.ContextSwitchGroup);
        
            return inState with
            {
                ContextSwitchGroupList = outContextSwitchGroupList
            };
        }
    }
}
