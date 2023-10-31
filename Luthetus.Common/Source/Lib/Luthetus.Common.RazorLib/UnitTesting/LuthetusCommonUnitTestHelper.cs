using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests;

public class LuthetusCommonUnitTestHelper
{
    public LuthetusCommonUnitTestHelper()
    {
        EnvironmentProvider = new InMemoryEnvironmentProvider();
        FileSystemProvider = new InMemoryFileSystemProvider(EnvironmentProvider);
    }

    public InMemoryEnvironmentProvider EnvironmentProvider { get; }
    public InMemoryFileSystemProvider FileSystemProvider { get; }
}
