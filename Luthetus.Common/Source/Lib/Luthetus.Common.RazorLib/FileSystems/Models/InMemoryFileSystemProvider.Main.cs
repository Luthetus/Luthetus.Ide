using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public partial class InMemoryFileSystemProvider : IFileSystemProvider
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly List<InMemoryFile> _files = new();
    private readonly SemaphoreSlim _modificationSemaphore = new(1, 1);
    private readonly InMemoryFileHandler _file;
    private readonly InMemoryDirectoryHandler _directory;

    public InMemoryFileSystemProvider(IEnvironmentProvider environmentProvider)
    {
        _environmentProvider = environmentProvider;

        _file = new InMemoryFileHandler(this, _environmentProvider);
        _directory = new InMemoryDirectoryHandler(this, _environmentProvider);

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