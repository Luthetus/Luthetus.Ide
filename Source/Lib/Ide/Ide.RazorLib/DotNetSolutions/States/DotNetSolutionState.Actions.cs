using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.States;

public partial record DotNetSolutionState
{
    public record RegisterAction(DotNetSolutionModel DotNetSolutionModel, LuthetusIdeBackgroundTaskApi IdeBackgroundTaskServiceApi);
    public record DisposeAction(Key<DotNetSolutionModel> DotNetSolutionModelKey, LuthetusIdeBackgroundTaskApi IdeBackgroundTaskServiceApi);
    
	public record StateHasChanged;
}