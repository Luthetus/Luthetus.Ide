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
    /// <see cref="RelativePath.PathType"/>
    /// <see cref="RelativePath.IsDirectory"/>
    /// <see cref="RelativePath.EnvironmentProvider"/>
    /// <see cref="RelativePath.AncestorDirectoryBag"/>
    /// <see cref="RelativePath.NameNoExtension"/>
    /// <see cref="RelativePath.ExtensionNoPeriod"/>
    /// <see cref="RelativePath.UpDirDirectiveCount"/>
    /// <see cref="RelativePath.ExactInput"/>
    /// <see cref="RelativePath.Value"/>
    /// <see cref="RelativePath.NameWithExtension"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        {
            var relativePathString = "../Math/addition.txt";
            var startingAbsolutePathString = "/Homework/Biology/nervousSystem.txt";
            var expectedEndingAbsolutePathString = "/Homework/Math/addition.txt";

            var relativePath = new RelativePath(relativePathString, false, environmentProvider);
            var startingAbsolutePath = new AbsolutePath(startingAbsolutePathString, false, environmentProvider);

            Assert.Equal(PathType.RelativePath, relativePath.PathType);
            Assert.False(relativePath.IsDirectory);
            Assert.Equal(environmentProvider, relativePath.EnvironmentProvider);
            Assert.Equal("addition", relativePath.NameNoExtension);
            Assert.Equal("txt", relativePath.ExtensionNoPeriod);
            Assert.Equal(1, relativePath.UpDirDirectiveCount);
            Assert.Equal(relativePathString, relativePath.ExactInput);
            Assert.Equal(relativePathString, relativePath.Value);
            Assert.Equal("addition.txt", relativePath.NameWithExtension);

            {
                // Assert.Equal(, relativePath.AncestorDirectoryBag); // How to handle testing this?
                Assert.True(false); // See AncestorDirectoryBag. How to handle this?
            }
        }

        {
            var relativePathString = "./skeletalSystem.txt";
            var startingAbsolutePathString = "/Homework/Biology/nervousSystem.txt";
            var expectedEndingAbsolutePathString = "/Homework/Biology/skeletalSystem.txt";

            var relativePath = new RelativePath(relativePathString, false, environmentProvider);
            var startingAbsolutePath = new AbsolutePath(startingAbsolutePathString, false, environmentProvider);

            Assert.Equal(PathType.RelativePath, relativePath.PathType);
            Assert.False(relativePath.IsDirectory);
            Assert.Equal(environmentProvider, relativePath.EnvironmentProvider);
            Assert.Equal("addition", relativePath.NameNoExtension);
            Assert.Equal("txt", relativePath.ExtensionNoPeriod);
            Assert.Equal(1, relativePath.UpDirDirectiveCount);
            Assert.Equal(relativePathString, relativePath.ExactInput);
            Assert.Equal(relativePathString, relativePath.Value);
            Assert.Equal("addition.txt", relativePath.NameWithExtension);

            {
                // Assert.Equal(, relativePath.AncestorDirectoryBag); // How to handle testing this?
                Assert.True(false); // See AncestorDirectoryBag. How to handle this?
            }
        }

        {
            var relativePathString = "../";
            var startingAbsolutePathString = "/Homework/Biology/nervousSystem.txt";
            var expectedEndingAbsolutePathString = "/Homework/";

            var relativePath = new RelativePath(relativePathString, false, environmentProvider);
            var startingAbsolutePath = new AbsolutePath(startingAbsolutePathString, false, environmentProvider);

            Assert.Equal(PathType.RelativePath, relativePath.PathType);
            Assert.False(relativePath.IsDirectory);
            Assert.Equal(environmentProvider, relativePath.EnvironmentProvider);
            Assert.Equal("addition", relativePath.NameNoExtension);
            Assert.Equal("txt", relativePath.ExtensionNoPeriod);
            Assert.Equal(1, relativePath.UpDirDirectiveCount);
            Assert.Equal(relativePathString, relativePath.ExactInput);
            Assert.Equal(relativePathString, relativePath.Value);
            Assert.Equal("addition.txt", relativePath.NameWithExtension);

            {
                // Assert.Equal(, relativePath.AncestorDirectoryBag); // How to handle testing this?
                Assert.True(false); // See AncestorDirectoryBag. How to handle this?
            }
        }

        {
            var relativePathString = "../";
            var startingAbsolutePathString = "/Homework/Biology/";
            var expectedEndingAbsolutePathString = "/Homework/";

            var relativePath = new RelativePath(relativePathString, false, environmentProvider);
            var startingAbsolutePath = new AbsolutePath(startingAbsolutePathString, false, environmentProvider);

            Assert.Equal(PathType.RelativePath, relativePath.PathType);
            Assert.False(relativePath.IsDirectory);
            Assert.Equal(environmentProvider, relativePath.EnvironmentProvider);
            Assert.Equal("addition", relativePath.NameNoExtension);
            Assert.Equal("txt", relativePath.ExtensionNoPeriod);
            Assert.Equal(1, relativePath.UpDirDirectiveCount);
            Assert.Equal(relativePathString, relativePath.ExactInput);
            Assert.Equal(relativePathString, relativePath.Value);
            Assert.Equal("addition.txt", relativePath.NameWithExtension);

            {
                // Assert.Equal(, relativePath.AncestorDirectoryBag); // How to handle testing this?
                Assert.True(false); // See AncestorDirectoryBag. How to handle this?
            }
        }

    }
}