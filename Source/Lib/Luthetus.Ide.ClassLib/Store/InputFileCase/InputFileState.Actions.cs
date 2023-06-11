using System.Collections.Immutable;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.TreeViewImplementations;

namespace Luthetus.Ide.ClassLib.Store.InputFileCase;

public partial record InputFileState
{
    public record RequestInputFileStateFormAction(string Message, Func<IAbsoluteFilePath?, Task> OnAfterSubmitFunc, Func<IAbsoluteFilePath?, Task<bool>> SelectionIsValidFunc, ImmutableArray<InputFilePattern> InputFilePatterns);
    public record SetSelectedTreeViewModelAction(TreeViewAbsoluteFilePath? SelectedTreeViewModel);
    public record SetOpenedTreeViewModelAction(TreeViewAbsoluteFilePath TreeViewModel, ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers, IFileSystemProvider FileSystemProvider, IEnvironmentProvider EnvironmentProvider);
    public record SetSelectedInputFilePatternAction(InputFilePattern InputFilePattern);
    public record SetSearchQueryAction(string SearchQuery);
    public record MoveBackwardsInHistoryAction;
    public record MoveForwardsInHistoryAction;
    public record OpenParentDirectoryAction(ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers, IFileSystemProvider FileSystemProvider, IEnvironmentProvider EnvironmentProvider, ICommonBackgroundTaskQueue CommonBackgroundTaskQueue);
    public record RefreshCurrentSelectionAction(ICommonBackgroundTaskQueue CommonBackgroundTaskQueue);
    public record StartInputFileStateFormAction(RequestInputFileStateFormAction RequestInputFileStateFormAction);
}