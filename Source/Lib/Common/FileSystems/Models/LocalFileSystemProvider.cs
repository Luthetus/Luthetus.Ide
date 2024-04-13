using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class LocalFileSystemProvider : IFileSystemProvider
{
    public LocalFileSystemProvider(
        IEnvironmentProvider environmentProvider,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        IDispatcher dispatcher)
    {
        File = new LocalFileHandler(environmentProvider, commonComponentRenderers, dispatcher);
        Directory = new LocalDirectoryHandler(environmentProvider, commonComponentRenderers, dispatcher);
    }

    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}