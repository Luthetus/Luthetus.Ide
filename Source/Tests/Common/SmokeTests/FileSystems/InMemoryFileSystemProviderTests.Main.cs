using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.FileSystems.Models;
using static Luthetus.Common.RazorLib.FileSystems.Models.InMemoryFileSystemProvider;
using static Luthetus.Common.Tests.SmokeTests.FileSystems.FileSystemsTestsHelper;

namespace Luthetus.Common.Tests.SmokeTests.FileSystems;

public partial class InMemoryFileSystemProviderTests
{
    [Fact]
    public void Constructor()
    {
        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.IsType<InMemoryFileSystemProvider>(fileSystemProvider);
        Assert.IsType<InMemoryDirectoryHandler>(fileSystemProvider.Directory);
        Assert.IsType<InMemoryFileHandler>(fileSystemProvider.File);

        Assert.Contains(fileSystemProvider.Files, x => x.AbsolutePath.Value == WellKnownPaths.Directories.Biology);
        Assert.Contains(fileSystemProvider.Files, x => x.AbsolutePath.Value == WellKnownPaths.Files.NervousSystemTxt);
    }
}
