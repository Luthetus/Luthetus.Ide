using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.InputFileCase;

public partial record InputFileRegistry
{
    public record RequestInputFileStateFormAction(
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
        ILuthetusCommonBackgroundTaskService LuthetusCommonBackgroundTaskService);

    public record RefreshCurrentSelectionAction(
        ILuthetusCommonBackgroundTaskService LuthetusCommonBackgroundTaskService);

    public record StartInputFileStateFormAction(
        RequestInputFileStateFormAction RequestInputFileStateFormAction);
}