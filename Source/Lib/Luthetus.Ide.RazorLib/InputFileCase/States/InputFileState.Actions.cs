using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.InputFileCase.States;

public partial record InputFileState
{
    public record RequestInputFileStateFormAction(
        InputFileSync Sync,
        string Message,
        Func<IAbsolutePath?, Task> OnAfterSubmitFunc,
        Func<IAbsolutePath?, Task<bool>> SelectionIsValidFunc,
        ImmutableArray<InputFilePattern> InputFilePatterns);

    public record SetSelectedTreeViewModelAction(
        TreeViewAbsolutePath? SelectedTreeViewModel);

    public record SetOpenedTreeViewModelAction(
        TreeViewAbsolutePath TreeViewModel,
        ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers,
        IFileSystemProvider FileSystemProvider,
        IEnvironmentProvider EnvironmentProvider);

    public record SetSelectedInputFilePatternAction(
        InputFilePattern InputFilePattern);

    public record SetSearchQueryAction(
        string SearchQuery);

    public record MoveBackwardsInHistoryAction;

    public record MoveForwardsInHistoryAction;

    public record OpenParentDirectoryAction(
        ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers,
        IFileSystemProvider FileSystemProvider,
        IEnvironmentProvider EnvironmentProvider,
        IBackgroundTaskService BackgroundTaskService);

    public record RefreshCurrentSelectionAction(
        IBackgroundTaskService BackgroundTaskService);

    public record StartInputFileStateFormAction(
        RequestInputFileStateFormAction RequestInputFileStateFormAction);
}