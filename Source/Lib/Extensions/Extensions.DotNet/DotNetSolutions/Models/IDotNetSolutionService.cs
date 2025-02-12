using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

public interface IDotNetSolutionService
{
	public event Action? DotNetSolutionStateChanged;
	
	public DotNetSolutionState GetDotNetSolutionState();

    public void ReduceRegisterAction(
    	DotNetSolutionModel dotNetSolutionModel,
    	DotNetSolutionIdeApi dotNetSolutionApi);

    public void ReduceDisposeAction(
    	Key<DotNetSolutionModel> dotNetSolutionModelKey,
	    DotNetSolutionIdeApi dotNetSolutionApi);

    public void ReduceWithAction(DotNetSolutionIdeApi.IWithAction withActionInterface);
    
	public Task NotifyDotNetSolutionStateStateHasChanged();
}
