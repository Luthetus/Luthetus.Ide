using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class IdePromptOpenSolutionDisplay : ComponentBase
{
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;

    [Parameter, EditorRequired]
    public IAbsolutePath AbsolutePath { get; set; } = null!;

    private async Task OpenSolutionOnClick()
    {
        await DotNetSolutionSync.SetDotNetSolution(AbsolutePath);
    }
}