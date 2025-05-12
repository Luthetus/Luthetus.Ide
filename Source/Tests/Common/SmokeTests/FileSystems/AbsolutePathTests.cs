using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.SmokeTests.FileSystems;

public class AbsolutePathTests
{
	[Fact]
    public void Constructor()
    {
        FileSystemsTestsHelper.InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        // Directory
        {
            // 'simple' input
            {
                var dirName = "homework";
                var dirPath = $@"/{dirName}/";
                var isDirectory = true;
                var dirAbsolutePath = environmentProvider.AbsolutePathFactory(dirPath, isDirectory);

                if (dirAbsolutePath.ParentDirectory is null)
                    throw new Exception();

                Assert.Equal("/", dirAbsolutePath.ParentDirectory);
                Assert.Equal(dirPath, dirAbsolutePath.ExactInput);
                Assert.Equal(PathType.AbsolutePath, dirAbsolutePath.PathType);
                Assert.Equal(isDirectory, dirAbsolutePath.IsDirectory);
                Assert.Equal(environmentProvider, dirAbsolutePath.EnvironmentProvider);
                Assert.Single(dirAbsolutePath.GetAncestorDirectoryList());
                Assert.Equal(dirName, dirAbsolutePath.NameNoExtension);

                Assert.Equal(
                    environmentProvider.DirectorySeparatorChar.ToString(),
                    dirAbsolutePath.ExtensionNoPeriod);

                Assert.Null(dirAbsolutePath.RootDrive);
                Assert.Equal(dirPath, dirAbsolutePath.Value);

                Assert.Equal(
                    dirName + environmentProvider.DirectorySeparatorChar,
                    dirAbsolutePath.NameWithExtension);

                Assert.False(dirAbsolutePath.IsRootDirectory);
            }
        }
        
        // File 
        {
            // 'simple' input
            {
                var fileName = "math";
                var fileExtension = "txt";
                var parentDirectoryName = "homework";
                var filePath = $@"/{parentDirectoryName}/{fileName}.{fileExtension}";
                var isDirectory = false;
                var fileAbsolutePath = environmentProvider.AbsolutePathFactory(filePath, isDirectory);

                if (fileAbsolutePath.ParentDirectory is null)
                    throw new Exception();

                Assert.Equal($@"/{parentDirectoryName}/", fileAbsolutePath.ParentDirectory);
                Assert.Equal(filePath, fileAbsolutePath.ExactInput);
                Assert.Equal(PathType.AbsolutePath, fileAbsolutePath.PathType);
                Assert.Equal(isDirectory, fileAbsolutePath.IsDirectory);
                Assert.Equal(environmentProvider, fileAbsolutePath.EnvironmentProvider);
                Assert.Equal(2, fileAbsolutePath.GetAncestorDirectoryList().Count);
                Assert.Equal(fileName, fileAbsolutePath.NameNoExtension);
                Assert.Equal(fileExtension, fileAbsolutePath.ExtensionNoPeriod);
                Assert.Null(fileAbsolutePath.RootDrive);
                Assert.Equal(filePath, fileAbsolutePath.Value);

                Assert.Equal(
                    fileName + '.' + fileExtension,
                    fileAbsolutePath.NameWithExtension);

                Assert.False(fileAbsolutePath.IsRootDirectory);
            }
        }
    }

    [Fact]
    public void Aaa()
    {
        FileSystemsTestsHelper.InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        var path = "C:\\Users\\hunte\\Repos";
        var isDirectory = true;
        var dirAbsolutePath = environmentProvider.AbsolutePathFactory(path, isDirectory);
    }
}
