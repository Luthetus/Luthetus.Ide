using System.Collections.Immutable;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.States;

public partial record InputFileState
{
    public record SetSelectedTreeViewModelAction(TreeViewAbsolutePath? SelectedTreeViewModel);
    public record SetSelectedInputFilePatternAction(InputFilePattern InputFilePattern);
    public record SetSearchQueryAction(string SearchQuery);
    
    public record RefreshCurrentSelectionAction(IBackgroundTaskService BackgroundTaskService)
    {
        public TreeViewAbsolutePath? CurrentSelection { get; set; }
    }

    public record MoveBackwardsInHistoryAction;
    public record MoveForwardsInHistoryAction;

    public record SetOpenedTreeViewModelAction(
        TreeViewAbsolutePath TreeViewModel,
        IIdeComponentRenderers IdeComponentRenderers,
        ICommonComponentRenderers CommonComponentRenderers,
        IFileSystemProvider FileSystemProvider,
        IEnvironmentProvider EnvironmentProvider);

    public record OpenParentDirectoryAction(
        IIdeComponentRenderers IdeComponentRenderers,
        ICommonComponentRenderers CommonComponentRenderers,
        IFileSystemProvider FileSystemProvider,
        IEnvironmentProvider EnvironmentProvider,
        IBackgroundTaskService BackgroundTaskService)
    {
        public TreeViewAbsolutePath? ParentDirectoryTreeViewModel { get; set; }
    }

    public record StartInputFileStateFormAction(
        string Message,
        Func<IAbsolutePath?, Task> OnAfterSubmitFunc,
        Func<IAbsolutePath?, Task<bool>> SelectionIsValidFunc,
        ImmutableArray<InputFilePattern> InputFilePatterns);
}