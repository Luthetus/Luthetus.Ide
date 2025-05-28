namespace Luthetus.Extensions.DotNet.Outputs.Models;

public interface IOutputService
{
    public event Action? OutputStateChanged;
    
    public OutputState GetOutputState();

    public void ReduceStateHasChangedAction(Guid dotNetRunParseResultId);
    
	public Task HandleConstructTreeViewEffect();
	
	public ValueTask Do_ConstructTreeView();
}
