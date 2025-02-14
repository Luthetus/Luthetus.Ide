using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

public class DotNetSolutionService : IDotNetSolutionService
{
	private DotNetSolutionState _dotNetSolutionState = new();
	
	public event Action? DotNetSolutionStateChanged;
	
	public DotNetSolutionState GetDotNetSolutionState() => _dotNetSolutionState;

    public void ReduceRegisterAction(
    	DotNetSolutionModel argumentDotNetSolutionModel,
    	DotNetSolutionIdeApi dotNetSolutionApi)
    {
    	var inState = GetDotNetSolutionState();
    
        var dotNetSolutionModel = inState.DotNetSolutionModel;

        if (dotNetSolutionModel is not null)
        {
            DotNetSolutionStateChanged?.Invoke();
            return;
        }

        var nextList = inState.DotNetSolutionsList.Add(argumentDotNetSolutionModel);

        _dotNetSolutionState = inState with
        {
            DotNetSolutionsList = nextList
        };
        
        DotNetSolutionStateChanged?.Invoke();
        return;
    }

    public void ReduceDisposeAction(
    	Key<DotNetSolutionModel> dotNetSolutionModelKey,
	    DotNetSolutionIdeApi dotNetSolutionApi)
    {
    	var inState = GetDotNetSolutionState();
    
        var dotNetSolutionModel = inState.DotNetSolutionModel;

        if (dotNetSolutionModel is null)
        {
            DotNetSolutionStateChanged?.Invoke();
        	return;
        }

        var nextList = inState.DotNetSolutionsList.Remove(dotNetSolutionModel);

        _dotNetSolutionState = inState with
        {
            DotNetSolutionsList = nextList
        };
        
        DotNetSolutionStateChanged?.Invoke();
        return;
    }

    public void ReduceWithAction(DotNetSolutionIdeApi.IWithAction withActionInterface)
    {
    	var inState = GetDotNetSolutionState();
    
        var withAction = (DotNetSolutionIdeApi.WithAction)withActionInterface;
        _dotNetSolutionState = withAction.WithFunc.Invoke(inState);
        
        DotNetSolutionStateChanged?.Invoke();
        return;
    }
    
	public Task NotifyDotNetSolutionStateStateHasChanged()
	{
		return Task.CompletedTask;
	}
}
