using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.FileSystems.Models;
using static Luthetus.Common.Tests.SmokeTests.FileSystems.FileSystemsTestsHelper;

namespace Luthetus.Common.Tests.SmokeTests.FileSystems;

public class RelativePathTests
{
    [Fact]
    public void Constructor()
    {
        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        {
            // This path is silly because it currently is going one UpDir directive too many
            // just to arive at the same ending location.
            //
            // That being said, I need to parse more than one UpDir directive, and
            // am using this for that purpose.
            var relativePathString = "../../Homework/Math/";
            var relativePath = environmentProvider.RelativePathFactory(relativePathString, true);

            Assert.Equal(PathType.RelativePath, relativePath.PathType);
            Assert.True(relativePath.IsDirectory);
            Assert.Equal(environmentProvider, relativePath.EnvironmentProvider);
            Assert.Equal("Math", relativePath.NameNoExtension);
            Assert.Equal("/", relativePath.ExtensionNoPeriod);
            Assert.Equal(2, relativePath.UpDirDirectiveCount);
            Assert.Equal(relativePathString, relativePath.ExactInput);
            Assert.Equal(relativePathString, relativePath.Value);
            Assert.Equal("Math/", relativePath.NameWithExtension);

            var homeworkDirectory = relativePath.GetAncestorDirectoryList()[0];
            var homeworkDirectoryRelativePath = new RelativePath(homeworkDirectory.Path, true, environmentProvider);
            Assert.Equal("Homework", homeworkDirectoryRelativePath.NameNoExtension);
        }

        {
            var relativePathString = "../Math/addition.txt";
            var relativePath = environmentProvider.RelativePathFactory(relativePathString, false);

            Assert.Equal(PathType.RelativePath, relativePath.PathType);
            Assert.False(relativePath.IsDirectory);
            Assert.Equal(environmentProvider, relativePath.EnvironmentProvider);
            Assert.Equal("addition", relativePath.NameNoExtension);
            Assert.Equal("txt", relativePath.ExtensionNoPeriod);
            Assert.Equal(1, relativePath.UpDirDirectiveCount);
            Assert.Equal(relativePathString, relativePath.ExactInput);
            Assert.Equal(relativePathString, relativePath.Value);
            Assert.Equal("addition.txt", relativePath.NameWithExtension);

            var mathDirectory = relativePath.GetAncestorDirectoryList()[0];
            var mathDirectoryRelativePath = new RelativePath(mathDirectory.Path, true, environmentProvider);
            Assert.Equal("Math", mathDirectoryRelativePath.NameNoExtension);
        }

        {
            var relativePathString = "./skeletalSystem.txt";
            var relativePath = environmentProvider.RelativePathFactory(relativePathString, false);

            Assert.Equal(PathType.RelativePath, relativePath.PathType);
            Assert.False(relativePath.IsDirectory);
            Assert.Equal(environmentProvider, relativePath.EnvironmentProvider);
            Assert.Equal("skeletalSystem", relativePath.NameNoExtension);
            Assert.Equal("txt", relativePath.ExtensionNoPeriod);
            Assert.Equal(0, relativePath.UpDirDirectiveCount);
            Assert.Equal(relativePathString, relativePath.ExactInput);
            Assert.Equal(relativePathString, relativePath.Value);
            Assert.Equal("skeletalSystem.txt", relativePath.NameWithExtension);

            Assert.Empty(relativePath.GetAncestorDirectoryList());
        }

        {
            var relativePathString = "../";
            var relativePath = environmentProvider.RelativePathFactory(relativePathString, true);

            Assert.Equal(PathType.RelativePath, relativePath.PathType);
            Assert.True(relativePath.IsDirectory);
            Assert.Equal(environmentProvider, relativePath.EnvironmentProvider);
            Assert.Equal(string.Empty, relativePath.NameNoExtension);
            Assert.Equal("/", relativePath.ExtensionNoPeriod);
            Assert.Equal(1, relativePath.UpDirDirectiveCount);
            Assert.Equal(relativePathString, relativePath.ExactInput);
            Assert.Equal(relativePathString, relativePath.Value);
            Assert.Equal("/", relativePath.NameWithExtension);

            Assert.Empty(relativePath.GetAncestorDirectoryList());
        }

        {
            var relativePathString = "../";
            var relativePath = environmentProvider.RelativePathFactory(relativePathString, true);

            Assert.Equal(PathType.RelativePath, relativePath.PathType);
            Assert.True(relativePath.IsDirectory);
            Assert.Equal(environmentProvider, relativePath.EnvironmentProvider);
            Assert.Equal(string.Empty, relativePath.NameNoExtension);
            Assert.Equal("/", relativePath.ExtensionNoPeriod);
            Assert.Equal(1, relativePath.UpDirDirectiveCount);
            Assert.Equal(relativePathString, relativePath.ExactInput);
            Assert.Equal(relativePathString, relativePath.Value);
            Assert.Equal("/", relativePath.NameWithExtension);

            Assert.Empty(relativePath.GetAncestorDirectoryList());
        }
    }
}
