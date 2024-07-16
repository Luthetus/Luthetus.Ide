using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.RazorLib.CSharpProjects.Models;
using Luthetus.CompilerServices.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.CompilerServices.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.CompilerServices.RazorLib.Websites;

public class WebsiteDotNetCliHelper
{
    public static async Task StartNewCSharpProjectCommand(
        CSharpProjectFormViewModelImmutable immutableView,
        IEnvironmentProvider environmentProvider,
        IFileSystemProvider fileSystemProvider,
        CompilerServicesBackgroundTaskApi compilerServicesBackgroundTaskApi,
        IDispatcher dispatcher,
		IDialog dialogRecord,
        ICommonComponentRenderers commonComponentRenderers)
    {
        var directoryContainingProject = environmentProvider
            .JoinPaths(immutableView.ParentDirectoryNameValue, immutableView.CSharpProjectNameValue) +
            environmentProvider.DirectorySeparatorChar;

        await fileSystemProvider.Directory
            .CreateDirectoryAsync(directoryContainingProject)
            .ConfigureAwait(false);

        var localCSharpProjectNameWithExtension = immutableView.CSharpProjectNameValue +
            '.' +
            ExtensionNoPeriodFacts.C_SHARP_PROJECT;

        var cSharpProjectAbsolutePathString = environmentProvider.JoinPaths(
            directoryContainingProject,
            localCSharpProjectNameWithExtension);

        await WebsiteProjectTemplateFacts.HandleNewCSharpProjectAsync(
                immutableView.ProjectTemplateShortNameValue,
                cSharpProjectAbsolutePathString,
                fileSystemProvider,
                environmentProvider)
            .ConfigureAwait(false);

        var cSharpAbsolutePath = environmentProvider.AbsolutePathFactory(
            cSharpProjectAbsolutePathString,
            false);

        compilerServicesBackgroundTaskApi.DotNetSolution.Website_AddExistingProjectToSolution(
            immutableView.DotNetSolutionModel.Key,
            immutableView.ProjectTemplateShortNameValue,
            immutableView.CSharpProjectNameValue,
            cSharpAbsolutePath);

        // Close Dialog
        dispatcher.Dispatch(new DialogState.DisposeAction(dialogRecord.DynamicViewModelKey));
        NotificationHelper.DispatchInformative("Website .sln template was used", "No terminal available", commonComponentRenderers, dispatcher, TimeSpan.FromSeconds(7));
    }
}
