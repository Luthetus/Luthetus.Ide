using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.CSharpProjectForms.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.RazorLib.Websites;

public class WebsiteDotNetCliHelper
{
    public static async Task StartNewCSharpProjectCommand(
        CSharpProjectFormViewModelImmutable immutableView,
        IEnvironmentProvider environmentProvider,
        IFileSystemProvider fileSystemProvider,
        DotNetSolutionSync dotNetSolutionSync,
        IDispatcher dispatcher,
        DialogRecord dialogRecord,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers)
    {
        var directoryContainingProject = environmentProvider
            .JoinPaths(immutableView.ParentDirectoryNameValue, immutableView.CSharpProjectNameValue) +
            environmentProvider.DirectorySeparatorChar;

        await fileSystemProvider.Directory.CreateDirectoryAsync(directoryContainingProject);

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
            environmentProvider);

        var cSharpAbsolutePath = new AbsolutePath(
            cSharpProjectAbsolutePathString,
            false,
            environmentProvider);

        dotNetSolutionSync.Website_AddExistingProjectToSolution(
            immutableView.DotNetSolutionModel.Key,
            immutableView.ProjectTemplateShortNameValue,
            immutableView.CSharpProjectNameValue,
            cSharpAbsolutePath,
            environmentProvider);

        // Close Dialog
        dispatcher.Dispatch(new DialogState.DisposeAction(dialogRecord.Key));
        NotificationHelper.DispatchInformative("Website .sln template was used", "No terminal available", luthetusCommonComponentRenderers, dispatcher);
    }
}
