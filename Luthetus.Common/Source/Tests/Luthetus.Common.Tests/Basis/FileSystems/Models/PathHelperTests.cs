using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="PathHelper"/>
/// </summary>
public class PathHelperTests
{
    /*
     * PathHelper.GetAbsoluteFromAbsoluteAndRelative needs to be broken down into many tests.
     * 
     * (directoryAbsolutePath, directoryRelativePath) => (directoryRelativePath)
     * (directoryAbsolutePath, directoryRelativePath) => (fileRelativePath)
     * (directoryAbsolutePath, fileRelativePath) => (directoryRelativePath)
     * (directoryAbsolutePath, fileRelativePath) => (fileRelativePath)
     * (fileAbsolutePath, directoryRelativePath) => (directoryRelativePath)
     * (fileAbsolutePath, directoryRelativePath) => (fileRelativePath)
     * (fileAbsolutePath, fileRelativePath) => (fileRelativePath)
     * (fileAbsolutePath, fileRelativePath) => (directoryRelativePath)
     *
     * -------------------------------------------------------------------------
     * 
     * (d, d) => (d)
     * (d, d) => (f)
     * (d, f) => (d)
     * (d, f) => (f)
     * (f, d) => (d)
     * (f, d) => (f)
     * (f, f) => (f)
     * (f, f) => (d)
     *
     * -------------------------------------------------------------------------
     * 
     * Explanation of thought:
     * 
     * Take in a directory absolute path, and a directory relative path.
     * Then have the result be a directory absolute path.
     * (d, d) => (d)
     * 
     * Take in a directory absolute path, and a directory relative path.
     * Then have the result be a file absolute path.
     * (d, d) => (f)
     * 
     * Take in a directory absolute path, and a file relative path.
     * Then have the result be a directory absolute path.
     * (d, f) => (d)
     *
     * -------------------------------------------------------------------------
     * 
     * More ideas:
     * 
     * I don't believe it the case that a relative path is accurrate, nor that an
     * absolute path is accurate.
     * 
     * -Root
     *     -Homework
     *         -Math
     *             -addition.txt
     *             -subtraction.txt
     *         -Biology
     *             -nervousSystem.txt
     *             -skeletalSystem.txt
     *
     * =============================================
     *
     * -Root
     *     -Homework
     *         -Math <---------------------------------
     *             -addition.txt                       ^ 
     *             -subtraction.txt                    |
     *         -Biology ------------------------------>
     *             -nervousSystem.txt
     *             -skeletalSystem.txt  
     *             
     * This goes from:
     *     "/Homework/Biology/"
     *         to
     *     "/Homework/Math/"
     *     
     * The relative path would be "../Math/"
     *
     * There are two main situations I'm thinking about:
     *     -"Biology" is directory and all is as expected
     *     -"Biology" is a file and....?
     *     
     * The relative path states that the resulting absolute path
     * should be a directory.
     * 
     * What is to be done in the case that the resulting absolute path does
     * not match what the relative path has as its 'IsDirectory' property?
     */

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
    /// <see cref="PathHelper.GetAbsoluteFromAbsoluteAndRelative(IAbsolutePath, string, IEnvironmentProvider)"/>
    /// ---------------------------------------------
    /// -Root
    ///     -Homework
    ///         -Math
    ///             -addition.txt
    ///             -subtraction.txt
    ///         -Biology
    ///             -nervousSystem.txt
    ///             -skeletalSystem.txt
    /// </summary>
    [Fact]
    public void GetAbsoluteFromAbsoluteAndRelative_Aaa()
    {
        FileSystemsTestsHelper.InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        // UpDir directive performs differently when starting on a file vs a directory
        {
            // If one starts on a directory, then the immediate parent is the answer.
            //
            // BUT if one starts on a file, the file's parent is the upper directory.
            // Instead it would be one ancestor further.
        }

        // File to file with UpDir directives.
        {
            var start = "/Homework/Biology/nervousSystem.txt";
            var relativePath = "../Math/addition.txt";

            var output = "/Homework/Math/addition.txt";
        }

        // File to file NOT-USING any UpDir directives.
        {
            var start = "/Homework/Biology/nervousSystem.txt";
            var relativePath = "./skeletalSystem.txt";

            var output = "/Homework/Biology/skeletalSystem.txt";
        }

        // A single UpDir directive from a file.
        {
            var start = "/Homework/Biology/nervousSystem.txt";
            var relativePath = "../";

            var output = "/Homework/";
        }

        // A single UpDir directive from a directory.
        {
            var start = "/Homework/Biology/";
            var relativePath = "../";

            var output = "/Homework/";
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
