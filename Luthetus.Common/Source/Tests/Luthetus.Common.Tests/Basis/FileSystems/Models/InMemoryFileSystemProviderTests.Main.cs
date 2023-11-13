using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;
using static Luthetus.Common.RazorLib.FileSystems.Models.InMemoryFileSystemProvider;
using static Luthetus.Common.Tests.Basis.FileSystems.FileSystemsTestsHelper;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="InMemoryFileSystemProvider"/>
/// </summary>
public partial class InMemoryFileSystemProviderTests
{
    /// <summary>
    /// <see cref="InMemoryFileSystemProvider(IEnvironmentProvider)"/>
    /// <br/>----<br/>
    /// <see cref="InMemoryFileSystemProvider.File"/>
    /// <see cref="InMemoryFileSystemProvider.Directory"/>
    /// <see cref="InMemoryFileSystemProvider.Files"/>
    /// </summary>
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