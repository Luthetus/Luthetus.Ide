using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.Basics.FileSystem.RelativePathTests;

public class RelativeDirectoryTests : PathTestsBase
{
    [Fact]
    public void Directory_FROM_Current_WITH_ForwardSlash()
    {
        var absolutePath = new AbsolutePath("./Aaa/", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Directory_FROM_Current_WITH_ForwardSlash_AND_MissingEnd()
    {
        var absolutePath = new AbsolutePath("./Aaa", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Directory_FROM_Current_WITH_BackSlash()
    {
        var absolutePath = new AbsolutePath(".\\Aaa\\", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Directory_FROM_Current_WITH_BackSlash_AND_MissingEnd()
    {
        var absolutePath = new AbsolutePath(".\\Aaa", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }

    [Fact]
    public void Directory_FROM_UpDir_WITH_ForwardSlash()
    {
        var absolutePath = new AbsolutePath("../Todos/", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Directory_FROM_UpDir_WITH_ForwardSlash_AND_MissingEnd()
    {
        var absolutePath = new AbsolutePath("../Todos", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Directory_FROM_UpDir_WITH_BackSlash()
    {
        var absolutePath = new AbsolutePath("..\\Todos\\", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Directory_FROM_UpDir_WITH_BackSlash_AND_MissingEnd()
    {
        var absolutePath = new AbsolutePath("..\\Todos", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Directory_FROM_Three_UpDir_WITH_ForwardSlash()
    {
        var absolutePath = new AbsolutePath("../../../Homework/Math/", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }

    [Fact]
    public void Directory_FROM_Three_UpDir_WITH_ForwardSlash_AND_MissingEnd()
    {
        var absolutePath = new AbsolutePath("../../../Homework/Math", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }
    
    [Fact]
    public void Directory_FROM_Three_UpDir_WITH_BackSlash()
    {
        var absolutePath = new AbsolutePath("..\\..\\..\\Homework\\Math\\", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }

    [Fact]
    public void Directory_FROM_Three_UpDir_WITH_BackSlash_AND_MissingEnd()
    {
        var absolutePath = new AbsolutePath("..\\..\\..\\Homework\\Math", true, CommonHelper.EnvironmentProvider);
        throw new NotImplementedException();
    }
}
