using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;

namespace Luthetus.Ide.Tests.Basis.DotNetSolutions.States;

public class DotNetSolutionSyncEnqueuesTests
{
    public void Website_AddExistingProjectToSolution(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        string projectTemplateShortName,
        string cSharpProjectName,
        IAbsolutePath cSharpProjectAbsolutePath,
        IEnvironmentProvider environmentProvider)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Add Existing-Project To Solution",
            async () => await Website_AddExistingProjectToSolutionAsync(
                dotNetSolutionModelKey,
                projectTemplateShortName,
                cSharpProjectName,
                cSharpProjectAbsolutePath,
                environmentProvider));
    }

    public void SetDotNetSolution(IAbsolutePath inSolutionAbsolutePath)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set .NET Solution",
            async () => await SetDotNetSolutionAsync(inSolutionAbsolutePath));
    }

    public void SetDotNetSolutionTreeView(Key<DotNetSolutionModel> dotNetSolutionModelKey)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set .NET Solution TreeView",
            async () => await SetDotNetSolutionTreeViewAsync(dotNetSolutionModelKey));
    }
}