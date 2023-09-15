using Fluxor;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.InputFileCase.States;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.TreeView.Models.TreeViewClasses;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase.States;

[FeatureState]
public partial record FolderExplorerState(
    IAbsolutePath? AbsolutePath,
    bool IsLoadingFolderExplorer)
{
    public static readonly TreeViewStateKey TreeViewFolderExplorerContentStateKey = TreeViewStateKey.NewKey();

    private FolderExplorerState() : this(
        default,
        false)
    {

    }

    public static void ShowInputFile(FolderExplorerSync folderExplorerSync)
    {
        folderExplorerSync.Dispatcher.Dispatch(new InputFileState.RequestInputFileStateFormAction(
            "Folder Explorer",
            afp =>
            {
                if (afp is not null)
                {
                    folderExplorerSync.Dispatcher.Dispatch(new SetFolderExplorerAction(
                        folderExplorerSync,
                        afp));
                }

                return Task.CompletedTask;
            },
            afp =>
            {
                if (afp is null || !afp.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", afp => afp.IsDirectory)
            }.ToImmutableArray()));
    }
}