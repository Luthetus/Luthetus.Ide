using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

public interface IDotNetSolutionService
{
	public event Action? DotNetSolutionStateChanged;
	
	public DotNetSolutionState GetDotNetSolutionState();

    public void ReduceRegisterAction(DotNetSolutionModel dotNetSolutionModel);

    public void ReduceDisposeAction(Key<DotNetSolutionModel> dotNetSolutionModelKey);

    public void ReduceWithAction(DotNetBackgroundTaskApi.IWithAction withActionInterface);
    
	public Task NotifyDotNetSolutionStateStateHasChanged();
}
