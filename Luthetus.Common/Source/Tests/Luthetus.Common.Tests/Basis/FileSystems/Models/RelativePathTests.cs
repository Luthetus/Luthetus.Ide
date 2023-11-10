using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;

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
        FileSystemsTestsHelper.InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        //var firstDirName = string.Empty;
        //var secondDirName = "school";
        //var thirdDirName = "homework";
        //var fourthDirName = "math";

        //// var workingDirPath = "/school/homework/math/";
        //var workingDirPath = $"{firstDirName}/{secondDirName}/{thirdDirName}/{fourthDirName}/";
        //var workingDirAbsolutePath = new AbsolutePath(workingDirPath, true, environmentProvider);

        //// Directory
        //{
        //    // 'simple' input
        //    {
        //        var relativePathString = $@"../";
        //        var isDirectory = true;
        //        var relativePath = new RelativePath(relativePathString, isDirectory, environmentProvider);

        //        var actualAbsolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
        //            workingDirAbsolutePath,
        //            relativePathString,
        //            environmentProvider);

        //        var actualAbsolutePath = new AbsolutePath(actualAbsolutePathString, true, environmentProvider);

        //        if (actualAbsolutePath.ParentDirectory is null)
        //            throw new Exception();

        //        var expectedPathString = $"/school/homework/";
        //        var expectedAbsolutePath = new AbsolutePath(expectedPathString, true, environmentProvider);

        //        Assert.Equal(expectedPathString, actualAbsolutePath.Value);
        //        Assert.Null(actualAbsolutePath.ExactInput);
        //        Assert.Equal(isDirectory, actualAbsolutePath.IsDirectory);
        //        Assert.Equal(environmentProvider, actualAbsolutePath.EnvironmentProvider);
        //        Assert.Equal(2, actualAbsolutePath.AncestorDirectoryBag.Count);
        //        Assert.Equal(expectedDirName, actualAbsolutePath.NameNoExtension);

        //        Assert.Equal(
        //            environmentProvider.DirectorySeparatorChar.ToString(),
        //            actualAbsolutePath.ExtensionNoPeriod);

        //        Assert.Null(actualAbsolutePath.RootDrive);
        //        Assert.Equal(actualAbsolutePathString, actualAbsolutePath.Value);

        //        Assert.Equal(
        //            expectedDirName + environmentProvider.DirectorySeparatorChar,
        //            actualAbsolutePath.NameWithExtension);

        //        Assert.False(actualAbsolutePath.IsRootDirectory);
        //    }
        //}
        
        //// File 
        //{
        //    // 'simple' input
        //    {
        //        var fileName = "math";
        //        var fileExtension = "txt";
        //        var parentDirectoryName = "homework";
        //        var filePath = $@"/{parentDirectoryName}/{fileName}.{fileExtension}";
        //        var isDirectory = false;
        //        var fileAbsolutePath = new AbsolutePath(filePath, isDirectory, environmentProvider);

        //        if (fileAbsolutePath.ParentDirectory is null)
        //            throw new Exception();

        //        Assert.Equal($@"/{parentDirectoryName}/", fileAbsolutePath.ParentDirectory.Value);
        //        Assert.Equal(filePath, fileAbsolutePath.ExactInput);
        //        Assert.Equal(PathType.AbsolutePath, fileAbsolutePath.PathType);
        //        Assert.Equal(isDirectory, fileAbsolutePath.IsDirectory);
        //        Assert.Equal(environmentProvider, fileAbsolutePath.EnvironmentProvider);
        //        Assert.Equal(2, fileAbsolutePath.AncestorDirectoryBag.Count);
        //        Assert.Equal(fileName, fileAbsolutePath.NameNoExtension);
        //        Assert.Equal(fileExtension, fileAbsolutePath.ExtensionNoPeriod);
        //        Assert.Null(fileAbsolutePath.RootDrive);
        //        Assert.Equal(filePath, fileAbsolutePath.Value);

        //        Assert.Equal(
        //            fileName + '.' + fileExtension,
        //            fileAbsolutePath.NameWithExtension);

        //        Assert.False(fileAbsolutePath.IsRootDirectory);
        //    }
        //}

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