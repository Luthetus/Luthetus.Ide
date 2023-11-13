using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;
using static Luthetus.Common.Tests.Basis.FileSystems.FileSystemsTestsHelper;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="RelativePath"/>
/// </summary>
public class RelativePathTests
{
    /// <summary>
    /// <see cref="RelativePath(string, bool, IEnvironmentProvider)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.PathType"/>
    /// </summary>
    [Fact]
    public void PathType()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.IsDirectory"/>
    /// </summary>
    [Fact]
    public void IsDirectory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.EnvironmentProvider"/>
    /// </summary>
    [Fact]
    public void EnvironmentProvider()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.AncestorDirectoryBag"/>
    /// </summary>
    [Fact]
    public void AncestorDirectoryBag()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.NameNoExtension"/>
    /// </summary>
    [Fact]
    public void NameNoExtension()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.ExtensionNoPeriod"/>
    /// </summary>
    [Fact]
    public void ExtensionNoPeriod()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.UpDirDirectiveCount"/>
    /// </summary>
    [Fact]
    public void UpDirDirectiveCount()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.ExactInput"/>
    /// </summary>
    [Fact]
    public void ExactInput()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.Value"/>
    /// </summary>
    [Fact]
    public void FormattedInput()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.NameWithExtension"/>
    /// </summary>
    [Fact]
    public void NameWithExtension()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="RelativePath.UsedDirectorySeparatorChar"/>
    /// </summary>
    [Fact]
    public void UsedDirectorySeparatorChar()
    {
        throw new NotImplementedException();
    }
}