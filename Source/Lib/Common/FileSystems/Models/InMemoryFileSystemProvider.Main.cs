using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public partial class InMemoryFileSystemProvider : IFileSystemProvider
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly List<InMemoryFile> _files = new();
    private readonly SemaphoreSlim _modificationSemaphore = new(1, 1);
    private readonly InMemoryFileHandler _file;
    private readonly InMemoryDirectoryHandler _directory;

    public InMemoryFileSystemProvider(
        IEnvironmentProvider environmentProvider,
        ICommonComponentRenderers commonComponentRenderers,
        IDispatcher dispatcher)
    {
        _environmentProvider = environmentProvider;

        _file = new InMemoryFileHandler(this, _environmentProvider, commonComponentRenderers, dispatcher);
        _directory = new InMemoryDirectoryHandler(this, _environmentProvider, commonComponentRenderers, dispatcher);

        Directory
            .CreateDirectoryAsync(_environmentProvider.RootDirectoryAbsolutePath.Value)
            .Wait();

        Directory
            .CreateDirectoryAsync(_environmentProvider.HomeDirectoryAbsolutePath.Value)
            .Wait();
    }

    public ImmutableArray<InMemoryFile> Files => _files.ToImmutableArray();

    public IFileHandler File => _file;
    public IDirectoryHandler Directory => _directory;
}