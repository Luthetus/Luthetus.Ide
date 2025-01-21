/*using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;
using static Luthetus.Common.Tests.Basis.FileSystems.FileSystemsTestsHelper;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="PathHelper"/>
/// </summary>
public class PathHelperTests
{
    /// <summary>
    /// <see cref="PathHelper.GetAbsoluteFromAbsoluteAndRelative(IAbsolutePath, string, IEnvironmentProvider)"/>
    /// ---------------------------------------------<br/>
    /// Root<br/>
    /// ∙└───Homework<br/>
    /// ∙∙∙∙∙∙├───Math<br/>
    /// ∙∙∙∙∙∙│∙∙∙∙├───addition.txt<br/>
    /// ∙∙∙∙∙∙│∙∙∙∙└───subtraction.txt<br/>
    /// ∙∙∙∙∙∙│<br/>
    /// ∙∙∙∙∙∙└───Biology<br/>
    /// ∙∙∙∙∙∙∙∙∙∙∙├───nervousSystem.txt<br/>
    /// ∙∙∙∙∙∙∙∙∙∙∙└───skeletalSystem.txt<br/>
    /// </summary>
    [Fact]
    public void GetAbsoluteFromAbsoluteAndRelative()
    {
        InitializeFileSystemsTests(
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
            var startPathString = "/Homework/Biology/nervousSystem.txt";
            var relativePathString = "../Math/addition.txt";
            var expectedOutputPathString = "/Homework/Math/addition.txt";

            var startAbsolutePath = environmentProvider.AbsolutePathFactory(startPathString, false);

            var outputAbsolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
                startAbsolutePath,
                relativePathString,
                environmentProvider);

            Assert.Equal(expectedOutputPathString, outputAbsolutePathString);
        }

        // File to file NOT-USING any UpDir directives.
        {
            var startPathString = "/Homework/Biology/nervousSystem.txt";
            var relativePathString = "./skeletalSystem.txt";
            var expectedOutputPathString = "/Homework/Biology/skeletalSystem.txt";

            var startAbsolutePath = environmentProvider.AbsolutePathFactory(startPathString, false);

            var outputAbsolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
                startAbsolutePath,
                relativePathString,
                environmentProvider);

            Assert.Equal(expectedOutputPathString, outputAbsolutePathString);
        }

        // A single UpDir directive from a file.
        {
            var startPathString = "/Homework/Biology/nervousSystem.txt";
            var relativePathString = "../";
            var expectedOutputPathString = "/Homework/";

            var startAbsolutePath = environmentProvider.AbsolutePathFactory(startPathString, false);

            var outputAbsolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
                startAbsolutePath,
                relativePathString,
                environmentProvider);

            Assert.Equal(expectedOutputPathString, outputAbsolutePathString);
        }

        // A single UpDir directive from a directory.
        {
            var startPathString = "/Homework/Biology/";
            var relativePathString = "../";
            var expectedOutputPathString = "/Homework/";

            var startAbsolutePath = environmentProvider.AbsolutePathFactory(startPathString, false);

            var outputAbsolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
                startAbsolutePath,
                relativePathString,
                environmentProvider);

            Assert.Equal(expectedOutputPathString, outputAbsolutePathString);
        }

        // BUG: (2024-05-18)
        // =================
        // This occurred while working with the git CLI but is expected to be a common case.
        //
        // Input AbsolutePath: "\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\"
        // Input RelativePath: "BlazorApp4NetCoreDbg/Shared/NavMenu.razor"
        // --------------------------------------------------------------------
        //
        // Expected result: "\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\BlazorApp4NetCoreDbg\Shared\NavMenu.razor"
        // Actual result: "\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg\Shared\NavMenu.razor"
        // --------------------------------------------------------------------
        //
        // This this "unit test" doesn't pass either, but for a different reason related to directory separator characters.
        // So, the original issue seems to be specific to the 'LocalEnvironmentProvider'.
        //{
        //    var startPathString = "\\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\";
        //    var relativePathString = "BlazorApp4NetCoreDbg/Shared/NavMenu.razor";
        //    var expectedOutputPathString = "\\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg\\Shared\\NavMenu.razor";
        //
        //    var startAbsolutePath = environmentProvider.AbsolutePathFactory(startPathString, false);
        //
        //    var outputAbsolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
        //        startAbsolutePath,
        //        relativePathString,
        //        environmentProvider);
        //
        //    // TODO: Delete this code block. I want to normalize the directory separator character
        //    //       to help with debugging this.
        //    {
        //        var outputAbsolutePath = environmentProvider.AbsolutePathFactory(outputAbsolutePathString, false);
        //
        //        // "/Users/hunte/Repos/Demos/BlazorApp4NetCoreDbg/BlazorApp4NetCoreDbg/Shared/NavMenu.razor"
        //        var normalizedOutputPathString = outputAbsolutePath.Value;
        //    }
        //
        //    Assert.Equal(expectedOutputPathString, outputAbsolutePathString);
        //}
    }

    /// <summary>
    /// <see cref="PathHelper.GetRelativeFromTwoAbsolutes(IAbsolutePath, IAbsolutePath, IEnvironmentProvider)"/>
    /// </summary>
    [Fact]
    public void GetRelativeFromTwoAbsolutes()
    {
        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        // File to file with UpDir directives.
        {
            var startAbsolutePath = environmentProvider.AbsolutePathFactory(
                WellKnownPaths.Files.NervousSystemTxt,
                false);

            var endAbsolutePath = environmentProvider.AbsolutePathFactory(
                WellKnownPaths.Files.AdditionTxt,
                false);

            var expectedRelativePathString = "../Math/addition.txt";

            var outputRelativePathString = PathHelper.GetRelativeFromTwoAbsolutes(
                startAbsolutePath,
                endAbsolutePath,
                environmentProvider);

            Assert.Equal(expectedRelativePathString, outputRelativePathString);
        }

        // File to file NOT-USING any UpDir directives.
        {
            var startAbsolutePath = environmentProvider.AbsolutePathFactory(
                WellKnownPaths.Files.NervousSystemTxt,
                false);

            var endAbsolutePath = environmentProvider.AbsolutePathFactory(
                WellKnownPaths.Files.SkeletalSystemTxt,
                false);

            var expectedRelativePathString = "./skeletalSystem.txt";

            var outputRelativePathString = PathHelper.GetRelativeFromTwoAbsolutes(
                startAbsolutePath,
                endAbsolutePath,
                environmentProvider);

            Assert.Equal(expectedRelativePathString, outputRelativePathString);
        }

        // A single UpDir directive from a file.
        {
            var startAbsolutePath = environmentProvider.AbsolutePathFactory(
                WellKnownPaths.Files.NervousSystemTxt,
                false);

            var endAbsolutePath = environmentProvider.AbsolutePathFactory(
                WellKnownPaths.Directories.Homework,
                false);

            var expectedRelativePathString = "../";

            var outputRelativePathString = PathHelper.GetRelativeFromTwoAbsolutes(
                startAbsolutePath,
                endAbsolutePath,
                environmentProvider);

            Assert.Equal(expectedRelativePathString, outputRelativePathString);
        }

        // A single UpDir directive from a directory.
        {
            var startAbsolutePath = environmentProvider.AbsolutePathFactory(
                WellKnownPaths.Directories.Biology,
                false);

            var endAbsolutePath = environmentProvider.AbsolutePathFactory(
                WellKnownPaths.Directories.Homework,
                false);

            var expectedRelativePathString = "../";

            var outputRelativePathString = PathHelper.GetRelativeFromTwoAbsolutes(
                startAbsolutePath,
                endAbsolutePath,
                environmentProvider);

            Assert.Equal(expectedRelativePathString, outputRelativePathString);
        }
    }

    /// <summary>
    /// <see cref="PathHelper.CalculateNameWithExtension(string, string, bool)"/>
    /// </summary>
    [Fact]
    public void CalculateNameWithExtension()
    {
        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.Equal("MyClass.cs", PathHelper.CalculateNameWithExtension(
            "MyClass",
            "cs",
            false));

        Assert.Equal("MyClass", PathHelper.CalculateNameWithExtension(
            "MyClass",
            string.Empty,
            false));

        Assert.Equal("MyClass/", PathHelper.CalculateNameWithExtension(
            "MyClass",
            environmentProvider.DirectorySeparatorChar.ToString(),
            true));
        
        Assert.Equal("MyClass.cs/", PathHelper.CalculateNameWithExtension(
            "MyClass.cs",
            environmentProvider.DirectorySeparatorChar.ToString(),
            true));
    }
}
*/