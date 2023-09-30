using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Common.RazorLib.KeyCase.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial record DotNetSolutionState
{
    public record RegisterAction(DotNetSolutionModel DotNetSolutionModel, DotNetSolutionSync Sync);
    public record DisposeAction(Key<DotNetSolutionModel> DotNetSolutionModelKey, DotNetSolutionSync Sync);
}