using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial class DotNetSolutionSync
{
    public void AddExistingProjectToSolution(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        string localProjectTemplateShortName,
        string localCSharpProjectName,
        IAbsolutePath cSharpProjectAbsolutePath,
        IEnvironmentProvider environmentProvider)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Add Existing-Project To Solution",
            async () => await AddExistingProjectToSolutionAsync(
                dotNetSolutionModelKey,
                localProjectTemplateShortName,
                localCSharpProjectName,
                cSharpProjectAbsolutePath,
                environmentProvider));
    }

    public void SetDotNetSolution(IAbsolutePath inSolutionAbsolutePath)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Set .NET Solution",
            async () => await SetDotNetSolutionAsync(inSolutionAbsolutePath));
    }

    public void SetDotNetSolutionTreeView(Key<DotNetSolutionModel> dotNetSolutionModelKey)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Set .NET Solution TreeView",
            async () => await SetDotNetSolutionTreeViewAsync(dotNetSolutionModelKey));
    }
}