using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests;

public class PathTestsBase
{
    protected readonly InMemoryEnvironmentProvider EnvironmentProvider;
    protected readonly InMemoryFileSystemProvider FileSystemProvider;

    public PathTestsBase()
    {
        EnvironmentProvider = new InMemoryEnvironmentProvider();
        FileSystemProvider = new InMemoryFileSystemProvider(EnvironmentProvider);
    }
}
