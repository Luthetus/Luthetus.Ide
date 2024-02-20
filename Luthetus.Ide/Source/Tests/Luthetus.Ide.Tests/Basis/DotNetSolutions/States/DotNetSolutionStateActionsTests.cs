using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;

namespace Luthetus.Ide.Tests.Basis.DotNetSolutions.States;

public class DotNetSolutionStateActionsTests
{
    public record RegisterAction(DotNetSolutionModel DotNetSolutionModel, DotNetSolutionSync Sync);
    public record DisposeAction(Key<DotNetSolutionModel> DotNetSolutionModelKey, DotNetSolutionSync Sync);
    
	public record StateHasChanged;
}