using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="AbsolutePath"/>
/// </summary>
public class AbsolutePathTests
{
    /// <summary>
    /// <see cref="AbsolutePath(string, bool, IEnvironmentProvider)"/>
    /// <br/>----<br/>
    /// <see cref="AbsolutePath.ParentDirectory"/>
    /// <see cref="AbsolutePath.ExactInput"/>
    /// <see cref="AbsolutePath.PathType"/>
    /// <see cref="AbsolutePath.IsDirectory"/>
    /// <see cref="AbsolutePath.EnvironmentProvider"/>
    /// <see cref="AbsolutePath.AncestorDirectoryList"/>
    /// <see cref="AbsolutePath.NameNoExtension"/>
    /// <see cref="AbsolutePath.ExtensionNoPeriod"/>
    /// <see cref="AbsolutePath.RootDrive"/>
    /// <see cref="AbsolutePath.Value"/>
    /// <see cref="AbsolutePath.NameWithExtension"/>
    /// <see cref="AbsolutePath.IsRootDirectory"/>
    /// </summary>
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
                var dirAbsolutePath = new AbsolutePath(dirPath, isDirectory, environmentProvider);

                if (dirAbsolutePath.ParentDirectory is null)
                    throw new Exception();

                Assert.Equal("/", dirAbsolutePath.ParentDirectory.Value);
                Assert.Equal(dirPath, dirAbsolutePath.ExactInput);
                Assert.Equal(PathType.AbsolutePath, dirAbsolutePath.PathType);
                Assert.Equal(isDirectory, dirAbsolutePath.IsDirectory);
                Assert.Equal(environmentProvider, dirAbsolutePath.EnvironmentProvider);
                Assert.Single(dirAbsolutePath.AncestorDirectoryList);
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
                var fileAbsolutePath = new AbsolutePath(filePath, isDirectory, environmentProvider);

                if (fileAbsolutePath.ParentDirectory is null)
                    throw new Exception();

                Assert.Equal($@"/{parentDirectoryName}/", fileAbsolutePath.ParentDirectory.Value);
                Assert.Equal(filePath, fileAbsolutePath.ExactInput);
                Assert.Equal(PathType.AbsolutePath, fileAbsolutePath.PathType);
                Assert.Equal(isDirectory, fileAbsolutePath.IsDirectory);
                Assert.Equal(environmentProvider, fileAbsolutePath.EnvironmentProvider);
                Assert.Equal(2, fileAbsolutePath.AncestorDirectoryList.Count);
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
}