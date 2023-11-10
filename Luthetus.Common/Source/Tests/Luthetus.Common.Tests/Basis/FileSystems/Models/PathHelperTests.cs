using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="PathHelper"/>
/// </summary>
public class PathHelperTests
{
    /// <summary>
    /// <see cref="PathHelper.GetAbsoluteFromAbsoluteAndRelative(IAbsolutePath, string, IEnvironmentProvider)"/>
    /// </summary>
    [Fact]
    public void GetAbsoluteFromAbsoluteAndRelative_Directory_Simple()
    {
        FileSystemsTestsHelper.InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        // Directory
        {
            // 'simple' input
            {
                var firstDirName = string.Empty;
                var secondDirName = "school";
                var thirdDirName = "homework";
                var fourthDirName = "math";

                // var workingDirPath = "/school/homework/math/";
                var workingDirPath = $"{firstDirName}/{secondDirName}/{thirdDirName}/{fourthDirName}/";
                var workingDirAbsolutePath = new AbsolutePath(workingDirPath, true, environmentProvider);

                var relativePathString = $@"../";
                var isDirectory = true;

                var actualAbsolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
                    workingDirAbsolutePath,
                    relativePathString,
                    environmentProvider);

                var actualAbsolutePath = new AbsolutePath(actualAbsolutePathString, true, environmentProvider);

                if (actualAbsolutePath.ParentDirectory is null)
                    throw new Exception();

                var expectedPathString = $"/school/homework/";

                Assert.Equal(expectedPathString, actualAbsolutePath.Value);
                Assert.Equal(actualAbsolutePathString, actualAbsolutePath.ExactInput);
                Assert.Equal(isDirectory, actualAbsolutePath.IsDirectory);
                Assert.Equal(environmentProvider, actualAbsolutePath.EnvironmentProvider);
                Assert.Equal(2, actualAbsolutePath.AncestorDirectoryBag.Count);
                Assert.Equal(thirdDirName, actualAbsolutePath.NameNoExtension);

                Assert.Equal(
                    environmentProvider.DirectorySeparatorChar.ToString(),
                    actualAbsolutePath.ExtensionNoPeriod);

                Assert.Null(actualAbsolutePath.RootDrive);
                Assert.Equal(actualAbsolutePathString, actualAbsolutePath.Value);

                Assert.Equal(
                    thirdDirName + environmentProvider.DirectorySeparatorChar,
                    actualAbsolutePath.NameWithExtension);

                Assert.False(actualAbsolutePath.IsRootDirectory);
            }
        }
    }

    /// <summary>
    /// <see cref="PathHelper.GetAbsoluteFromAbsoluteAndRelative(IAbsolutePath, string, IEnvironmentProvider)"/>
    /// </summary>
    [Fact]
    public void GetAbsoluteFromAbsoluteAndRelative_File_Simple()
    {
        FileSystemsTestsHelper.InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        // File 
        {
            // 'simple' input
            {
                var firstDirName = string.Empty;
                var secondDirName = "school";
                var thirdDirName = "homework";
                var fourthDirName = "math";
                var fileName = "addition";
                var fileExtension = "txt";

                // var fileAbsolutePathString = "/school/homework/math/addition.txt";
                var fileAbsolutePathString = 
                    $"{firstDirName}/{secondDirName}/{thirdDirName}/{fourthDirName}/{fileName}.{fileExtension}";

                var fileAbsolutePath = new AbsolutePath(fileAbsolutePathString, true, environmentProvider);

                var relativePathString = $@"../../..";
                var isDirectory = true;

                var actualAbsolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
                    fileAbsolutePath,
                    relativePathString,
                    environmentProvider);

                var actualAbsolutePath = new AbsolutePath(actualAbsolutePathString, true, environmentProvider);

                if (fileAbsolutePath.ParentDirectory is null)
                    throw new Exception();

                var expectedPathString = $"/school/homework/";

                Assert.Equal(expectedPathString, actualAbsolutePath.Value);
                Assert.Equal(actualAbsolutePathString, actualAbsolutePath.ExactInput);
                Assert.Equal(isDirectory, actualAbsolutePath.IsDirectory);
                Assert.Equal(environmentProvider, actualAbsolutePath.EnvironmentProvider);
                Assert.Equal(2, actualAbsolutePath.AncestorDirectoryBag.Count);
                Assert.Equal(firstDirName, actualAbsolutePath.NameNoExtension);

                Assert.Equal(
                    environmentProvider.DirectorySeparatorChar.ToString(),
                    actualAbsolutePath.ExtensionNoPeriod);

                Assert.Null(actualAbsolutePath.RootDrive);
                Assert.Equal(actualAbsolutePathString, actualAbsolutePath.Value);

                Assert.Equal(
                    firstDirName + environmentProvider.DirectorySeparatorChar,
                    actualAbsolutePath.NameWithExtension);

                Assert.False(actualAbsolutePath.IsRootDirectory);
            }
        }

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="PathHelper.GetRelativeFromTwoAbsolutes(IAbsolutePath, IAbsolutePath, IEnvironmentProvider)"/>
    /// </summary>
    [Fact]
    public void GetRelativeFromTwoAbsolutes()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="PathHelper.CalculateNameWithExtension(string, string, bool)"/>
    /// </summary>
    [Fact]
    public void CalculateNameWithExtension()
    {
        throw new NotImplementedException();
    }
}
