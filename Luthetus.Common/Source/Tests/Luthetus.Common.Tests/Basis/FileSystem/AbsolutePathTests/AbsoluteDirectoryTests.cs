using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.Tests;

namespace Luthetus.Common.Tests.Basics.FileSystem.AbsolutePathTests;

public class AbsoluteDirectoryTests : CommonTestingBase
{
    [Fact]
    public void Directory_WITH_ForwardSlash()
    {
        var pathString = "/Aaa/Bbb/Ccc/";

        var absolutePath = new AbsolutePath(pathString, true, CommonHelper.EnvironmentProvider);

        Assert.Equal(pathString, absolutePath.ExactInput);

        // Presumption that '/' is the DirectorySeparatorChar is being made here.
        Assert.Equal(pathString, absolutePath.FormattedInput);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[1];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[2];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);

        Assert.Equal("Ccc", absolutePath.NameNoExtension);
        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
        Assert.Equal("Ccc/", absolutePath.NameWithExtension);
    }

    [Fact]
    public void Directory_WITH_ForwardSlash_AND_MissingEnd()
    {
        var pathString = "/Aaa/Bbb/Ccc";

        var absolutePath = new AbsolutePath("/Aaa/Bbb/Ccc", true, CommonHelper.EnvironmentProvider);

        Assert.Equal(pathString, absolutePath.ExactInput);

        // Presumption that '/' is the DirectorySeparatorChar is being made here.
        Assert.Equal("/Aaa/Bbb/Ccc/", absolutePath.FormattedInput);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[1];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[2];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);
        
        Assert.Equal("Ccc", absolutePath.NameNoExtension);
        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
        Assert.Equal("Ccc/", absolutePath.NameWithExtension);
    }
    
    [Fact]
    public void Directory_WITH_BackSlash()
    {
        var pathString = "\\Aaa\\Bbb\\Ccc\\";

        var absolutePath = new AbsolutePath(pathString, true, CommonHelper.EnvironmentProvider);

        Assert.Equal(pathString, absolutePath.ExactInput);

        // Presumption that '/' is the DirectorySeparatorChar is being made here.
        Assert.Equal("/Aaa/Bbb/Ccc/", absolutePath.FormattedInput);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[1];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[2];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);

        Assert.Equal("Ccc", absolutePath.NameNoExtension);
        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
        Assert.Equal("Ccc/", absolutePath.NameWithExtension);
    }

    [Fact]
    public void Directory_WITH_BackSlash_AND_MissingEnd()
    {
        var pathString = "\\Aaa\\Bbb\\Ccc";

        var absolutePath = new AbsolutePath(pathString, true, CommonHelper.EnvironmentProvider);

        Assert.Equal(pathString, absolutePath.ExactInput);

        // Presumption that '/' is the DirectorySeparatorChar is being made here.
        Assert.Equal("/Aaa/Bbb/Ccc/", absolutePath.FormattedInput);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[1];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[2];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);

        Assert.Equal("Ccc", absolutePath.NameNoExtension);
        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
        Assert.Equal("Ccc/", absolutePath.NameWithExtension);
    }

    [Fact]
    public void Directory_FROM_Drive_WITH_ForwardSlash()
    {
        var pathString = "C:/Aaa/Bbb/Ccc/";

        var absolutePath = new AbsolutePath(pathString, true, CommonHelper.EnvironmentProvider);

        Assert.Equal(pathString, absolutePath.ExactInput);

        // Presumption that '/' is the DirectorySeparatorChar is being made here.
        Assert.Equal("C:/Aaa/Bbb/Ccc/", absolutePath.FormattedInput);

        Assert.NotNull(absolutePath.RootDrive);
        Assert.Equal("C", absolutePath.RootDrive!.DriveNameAsIdentifier);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[1];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[2];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);

        Assert.Equal("Ccc", absolutePath.NameNoExtension);
        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
        Assert.Equal("Ccc/", absolutePath.NameWithExtension);
    }

    [Fact]
    public void Directory_FROM_Drive_WITH_ForwardSlash_AND_MissingEnd()
    {
        var pathString = "C:/Aaa/Bbb/Ccc";

        var absolutePath = new AbsolutePath(pathString, true, CommonHelper.EnvironmentProvider);

        Assert.Equal(pathString, absolutePath.ExactInput);

        // Presumption that '/' is the DirectorySeparatorChar is being made here.
        Assert.Equal("C:/Aaa/Bbb/Ccc/", absolutePath.FormattedInput);

        Assert.NotNull(absolutePath.RootDrive);
        Assert.Equal("C", absolutePath.RootDrive!.DriveNameAsIdentifier);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[1];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[2];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);

        Assert.Equal("Ccc", absolutePath.NameNoExtension);
        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
        Assert.Equal("Ccc/", absolutePath.NameWithExtension);
    }

    [Fact]
    public void Directory_FROM_Drive_WITH_BackSlash()
    {
        var pathString = "C:\\Aaa\\Bbb\\Ccc\\";

        var absolutePath = new AbsolutePath(pathString, true, CommonHelper.EnvironmentProvider);

        Assert.Equal(pathString, absolutePath.ExactInput);

        // Presumption that '/' is the DirectorySeparatorChar is being made here.
        Assert.Equal("C:/Aaa/Bbb/Ccc/", absolutePath.FormattedInput);

        Assert.NotNull(absolutePath.RootDrive);
        Assert.Equal("C", absolutePath.RootDrive!.DriveNameAsIdentifier);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[1];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[2];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);

        Assert.Equal("Ccc", absolutePath.NameNoExtension);
        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
        Assert.Equal("Ccc/", absolutePath.NameWithExtension);
    }

    [Fact]
    public void Directory_FROM_WITH_BackSlash_Drive_AND_MissingEnd()
    {
        var pathString = "C:\\Aaa\\Bbb\\Ccc";

        var absolutePath = new AbsolutePath(pathString, true, CommonHelper.EnvironmentProvider);

        Assert.Equal(pathString, absolutePath.ExactInput);

        // Presumption that '/' is the DirectorySeparatorChar is being made here.
        Assert.Equal("C:/Aaa/Bbb/Ccc/", absolutePath.FormattedInput);

        Assert.NotNull(absolutePath.RootDrive);
        Assert.Equal("C", absolutePath.RootDrive!.DriveNameAsIdentifier);

        Assert.Equal(3, absolutePath.AncestorDirectoryBag.Count);

        var ancestorIndexZero = absolutePath.AncestorDirectoryBag[0];
        Assert.Equal(string.Empty, ancestorIndexZero.NameNoExtension);
        Assert.Equal("/", ancestorIndexZero.ExtensionNoPeriod);
        Assert.Equal("/", ancestorIndexZero.NameWithExtension);

        var ancestorIndexOne = absolutePath.AncestorDirectoryBag[1];
        Assert.Equal("Aaa", ancestorIndexOne.NameNoExtension);
        Assert.Equal("/", ancestorIndexOne.ExtensionNoPeriod);
        Assert.Equal("Aaa/", ancestorIndexOne.NameWithExtension);

        var ancestorIndexTwo = absolutePath.AncestorDirectoryBag[2];
        Assert.Equal("Bbb", ancestorIndexTwo.NameNoExtension);
        Assert.Equal("/", ancestorIndexTwo.ExtensionNoPeriod);
        Assert.Equal("Bbb/", ancestorIndexTwo.NameWithExtension);

        Assert.Equal("Ccc", absolutePath.NameNoExtension);
        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
        Assert.Equal("Ccc/", absolutePath.NameWithExtension);
    }
}
