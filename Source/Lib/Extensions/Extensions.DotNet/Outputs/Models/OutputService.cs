using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Models;

public class OutputService : IOutputService
{
	private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
		
	private readonly Throttle _throttleCreateTreeView = new Throttle(TimeSpan.FromMilliseconds(333));
	
	public OutputService(DotNetBackgroundTaskApi dotNetBackgroundTaskApi)
	{
		_dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
    }
    
    private OutputState _outputState = new();
    
    public event Action? OutputStateChanged;
    
    public OutputState GetOutputState() => _outputState;

    public void ReduceStateHasChangedAction(Guid dotNetRunParseResultId)
    {
    	var inState = GetOutputState();
    
        _outputState = inState with
        {
        	DotNetRunParseResultId = dotNetRunParseResultId
        };
        
        OutputStateChanged?.Invoke();
        return;
    }
    
	public Task HandleConstructTreeViewEffect()
	{
		_throttleCreateTreeView.Run(async _ => await _dotNetBackgroundTaskApi.Output.Task_ConstructTreeView());
        return Task.CompletedTask;
	}
}
