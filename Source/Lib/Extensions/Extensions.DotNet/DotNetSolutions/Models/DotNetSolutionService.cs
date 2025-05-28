using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

public class DotNetSolutionService : IDotNetSolutionService
{
	private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;

	private DotNetSolutionState _dotNetSolutionState = new();
	
	public event Action? DotNetSolutionStateChanged;
	
	public DotNetSolutionState GetDotNetSolutionState() => _dotNetSolutionState;
	
	public DotNetSolutionService(DotNetBackgroundTaskApi dotNetBackgroundTaskApi)
	{
		_dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
	}

    public void ReduceRegisterAction(DotNetSolutionModel argumentDotNetSolutionModel)
    {
    	var inState = GetDotNetSolutionState();
    
        var dotNetSolutionModel = inState.DotNetSolutionModel;

        if (dotNetSolutionModel is not null)
        {
            DotNetSolutionStateChanged?.Invoke();
            return;
        }

        var nextList = new List<DotNetSolutionModel>(inState.DotNetSolutionsList);
        nextList.Add(argumentDotNetSolutionModel);

        _dotNetSolutionState = inState with
        {
            DotNetSolutionsList = nextList
        };
        
        DotNetSolutionStateChanged?.Invoke();
        return;
    }

    public void ReduceDisposeAction(Key<DotNetSolutionModel> dotNetSolutionModelKey)
    {
    	var inState = GetDotNetSolutionState();
    
        var dotNetSolutionModel = inState.DotNetSolutionModel;

        if (dotNetSolutionModel is null)
        {
            DotNetSolutionStateChanged?.Invoke();
        	return;
        }

        var nextList = new List<DotNetSolutionModel>(inState.DotNetSolutionsList);
        nextList.Remove(dotNetSolutionModel);

        _dotNetSolutionState = inState with
        {
            DotNetSolutionsList = nextList
        };
        
        DotNetSolutionStateChanged?.Invoke();
        return;
    }

    public void ReduceWithAction(DotNetBackgroundTaskApi.IWithAction withActionInterface)
    {
    	var inState = GetDotNetSolutionState();
    
        var withAction = (DotNetBackgroundTaskApi.WithAction)withActionInterface;
        _dotNetSolutionState = withAction.WithFunc.Invoke(inState);
        
        DotNetSolutionStateChanged?.Invoke();
        return;
    }
    
	public Task NotifyDotNetSolutionStateStateHasChanged()
	{
		return Task.CompletedTask;
	}
}
