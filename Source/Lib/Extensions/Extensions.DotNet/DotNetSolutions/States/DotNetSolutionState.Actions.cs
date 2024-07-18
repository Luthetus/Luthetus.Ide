using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.States;

public partial record DotNetSolutionState
{
    public record RegisterAction(
    	DotNetSolutionModel DotNetSolutionModel,
    	DotNetSolutionIdeApi DotNetSolutionApi);
    	
    public record DisposeAction(
	    Key<DotNetSolutionModel> DotNetSolutionModelKey,
	    DotNetSolutionIdeApi DotNetSolutionApi);
    
	public record StateHasChanged;
}