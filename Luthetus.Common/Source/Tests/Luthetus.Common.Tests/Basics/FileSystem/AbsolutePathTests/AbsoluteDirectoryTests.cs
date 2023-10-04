using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.Basics.FileSystem.AbsolutePathTests;

public class AbsoluteDirectoryTests : PathTestsBase
{
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_ROOT_WITH_ENDING_DIRECTORY_SEPARATOR_USING_FORWARD_SLASH()
    {
        var absolutePath = new AbsolutePath("/Aaa/Bbb/Ccc/", true, EnvironmentProvider);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);
    }

    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_ROOT_MISSING_ENDING_DIRECTORY_SEPARATOR_USING_FORWARD_SLASH()
    {
        var absolutePath = new AbsolutePath("/Aaa/Bbb/Ccc", true, EnvironmentProvider);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);
    }
    
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_ROOT_WITH_ENDING_DIRECTORY_SEPARATOR_USING_BACK_SLASH()
    {
        var absolutePath = new AbsolutePath("\\Aaa\\Bbb\\Ccc\\", true, EnvironmentProvider);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);
    }

    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_ROOT_MISSING_ENDING_DIRECTORY_SEPARATOR_USING_BACK_SLASH()
    {
        var absolutePath = new AbsolutePath("\\Aaa\\Bbb\\Ccc", true, EnvironmentProvider);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);
    }

    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_DRIVE_WITH_ENDING_DIRECTORY_SEPARATOR_USING_FORWARD_SLASH()
    {
        var absolutePath = new AbsolutePath("C:/Users/hunte/Repos/", true, EnvironmentProvider);

        Assert.NotNull(absolutePath.RootDrive);
        Assert.Equal("C", absolutePath.RootDrive!.DriveNameAsIdentifier);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);
    }

    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_DRIVE_MISSING_ENDING_DIRECTORY_SEPARATOR_USING_FORWARD_SLASH()
    {
        var absolutePath = new AbsolutePath("C:/Users/hunte/Repos", true, EnvironmentProvider);
    }

    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_DRIVE_WITH_ENDING_DIRECTORY_SEPARATOR_USING_BACK_SLASH()
    {
        var absolutePath = new AbsolutePath("C:\\Users\\hunte\\Repos\\", true, EnvironmentProvider);
    }

    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_DRIVE_MISSING_ENDING_DIRECTORY_SEPARATOR_USING_BACK_SLASH()
    {
        var absolutePath = new AbsolutePath("C:\\Users\\hunte\\Repos", true, EnvironmentProvider);
    }
}
