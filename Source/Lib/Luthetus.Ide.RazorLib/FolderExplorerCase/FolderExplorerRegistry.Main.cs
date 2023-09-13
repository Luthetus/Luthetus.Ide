using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib.InputFileCase;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase;

[FeatureState]
public partial record FolderExplorerRegistry(
    IAbsolutePath? AbsolutePath,
    bool IsLoadingFolderExplorer)
{
    public static readonly TreeViewStateKey TreeViewFolderExplorerContentStateKey = TreeViewStateKey.NewKey();

    private FolderExplorerRegistry() : this(
        default,
        false)
    {

    }

    public static void ShowInputFile(IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new InputFileRegistry.RequestInputFileStateFormAction(
            "Folder Explorer",
            afp =>
            {
                if (afp is not null)
                    dispatcher.Dispatch(new SetFolderExplorerAction(afp));

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