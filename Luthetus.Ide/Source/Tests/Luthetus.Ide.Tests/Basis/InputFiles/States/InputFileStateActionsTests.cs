using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.Tests.Basis.InputFiles.States;

public class InputFileStateActionsTests
{
    [Fact]
    public void SetSelectedTreeViewModelAction()
    {
        //public record (TreeViewAbsolutePath? SelectedTreeViewModel);
    }
    
    [Fact]
    public void SetSelectedInputFilePatternAction()
    {
        //public record (InputFilePattern InputFilePattern);
    }
    
    [Fact]
    public void SetSearchQueryAction()
    {
        //public record (string SearchQuery);
    }
    
    [Fact]
    public void RefreshCurrentSelectionAction()
    {
        //public record (IBackgroundTaskService BackgroundTaskService);
    }
    
    [Fact]
    public void MoveBackwardsInHistoryAction()
    {
        //public record ;
    }

    [Fact]
    public void MoveForwardsInHistoryAction()
    {
        //public record ;
    }

    [Fact]
    public void SetOpenedTreeViewModelAction()
    {
        //public record (
        //    TreeViewAbsolutePath TreeViewModel,
        //    ILuthetusIdeComponentRenderers IdeComponentRenderers,
        //    ILuthetusCommonComponentRenderers CommonComponentRenderers,
        //    IFileSystemProvider FileSystemProvider,
        //    IEnvironmentProvider EnvironmentProvider);
    }

    [Fact]
    public void OpenParentDirectoryAction()
    {
        //public record (
        //    ILuthetusIdeComponentRenderers IdeComponentRenderers,
        //    ILuthetusCommonComponentRenderers CommonComponentRenderers,
        //    IFileSystemProvider FileSystemProvider,
        //    IEnvironmentProvider EnvironmentProvider,
        //    IBackgroundTaskService BackgroundTaskService);
    }

    [Fact]
    public void StartInputFileStateFormAction()
    {
        //public record (
        //    string Message,
        //    Func<IAbsolutePath?, Task> OnAfterSubmitFunc,
        //    Func<IAbsolutePath?, Task<bool>> SelectionIsValidFunc,
        //    ImmutableArray<InputFilePattern> InputFilePatterns);
    }
}