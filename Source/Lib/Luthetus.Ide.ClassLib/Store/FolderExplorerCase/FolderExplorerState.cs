using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Store.FolderExplorerCase;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.FolderExplorerCase;

[FeatureState]
public record FolderExplorerState(IAbsoluteFilePath? AbsoluteFilePath)
{
    public FolderExplorerState() : this(default(IAbsoluteFilePath))
    {

    }

    public static Task ShowInputFileAsync(IDispatcher dispatcher)
    {
        dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "FolderExplorer",
                afp =>
                {
                    dispatcher.Dispatch(
                        new SetFolderExplorerStateAction(afp));

                    return Task.CompletedTask;
                },
                afp =>
                {
                    if (afp is null ||
                        !afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(true);
                },
                new[]
                {
                    new InputFilePattern(
                        "Directory",
                        afp => afp.IsDirectory)
                }.ToImmutableArray()));

        return Task.CompletedTask;
    }
}