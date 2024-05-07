using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.States;

public partial class DotNetSolutionSync
{
    public Task Website_AddExistingProjectToSolution(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        string projectTemplateShortName,
        string cSharpProjectName,
        IAbsolutePath cSharpProjectAbsolutePath,
        IEnvironmentProvider environmentProvider)
    {
        return BackgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Add Existing-Project To Solution",
            async () => await Website_AddExistingProjectToSolutionAsync(
                dotNetSolutionModelKey,
                projectTemplateShortName,
                cSharpProjectName,
                cSharpProjectAbsolutePath,
                environmentProvider));
    }

    public Task SetDotNetSolution(IAbsolutePath inSolutionAbsolutePath)
    {
        return BackgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set .NET Solution",
            async () => await SetDotNetSolutionAsync(inSolutionAbsolutePath));
    }

    public Task SetDotNetSolutionTreeView(Key<DotNetSolutionModel> dotNetSolutionModelKey)
    {
        return BackgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set .NET Solution TreeView",
            async () => await SetDotNetSolutionTreeViewAsync(dotNetSolutionModelKey));
    }
}