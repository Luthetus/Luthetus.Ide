using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.InputFiles.States;

public partial record InputFileState
{
    public record SetSelectedTreeViewModelAction(TreeViewAbsolutePath? SelectedTreeViewModel);
    public record SetSelectedInputFilePatternAction(InputFilePattern InputFilePattern);
    public record SetSearchQueryAction(string SearchQuery);
    public record RefreshCurrentSelectionAction(IBackgroundTaskService BackgroundTaskService);
    public record MoveBackwardsInHistoryAction;
    public record MoveForwardsInHistoryAction;

    public record SetOpenedTreeViewModelAction(
        TreeViewAbsolutePath TreeViewModel,
        ILuthetusIdeComponentRenderers IdeComponentRenderers,
        ILuthetusCommonComponentRenderers CommonComponentRenderers,
        IFileSystemProvider FileSystemProvider,
        IEnvironmentProvider EnvironmentProvider);

    public record OpenParentDirectoryAction(
        ILuthetusIdeComponentRenderers IdeComponentRenderers,
        ILuthetusCommonComponentRenderers CommonComponentRenderers,
        IFileSystemProvider FileSystemProvider,
        IEnvironmentProvider EnvironmentProvider,
        IBackgroundTaskService BackgroundTaskService);

    public record StartInputFileStateFormAction(
        string Message,
        Func<IAbsolutePath?, Task> OnAfterSubmitFunc,
        Func<IAbsolutePath?, Task<bool>> SelectionIsValidFunc,
        ImmutableArray<InputFilePattern> InputFilePatterns);
}