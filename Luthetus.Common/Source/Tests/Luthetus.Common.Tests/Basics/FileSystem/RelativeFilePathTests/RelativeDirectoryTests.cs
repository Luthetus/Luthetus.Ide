using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.Basics.FileSystem.RelativePathTests;

public class RelativeDirectoryTests : PathTestsBase
{
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_CURRENT_WITH_ENDING_DIRECTORY_SEPARATOR_USING_FORWARD_SLASH()
    {
        var absolutePath = new AbsolutePath("./Aaa/", true, CommonHelper.EnvironmentProvider);
    }
    
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_CURRENT_MISSING_ENDING_DIRECTORY_SEPARATOR_USING_FORWARD_SLASH()
    {
        var absolutePath = new AbsolutePath("./Aaa", true, CommonHelper.EnvironmentProvider);
    }
    
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_CURRENT_WITH_ENDING_DIRECTORY_SEPARATOR_USING_BACK_SLASH()
    {
        var absolutePath = new AbsolutePath(".\\Aaa\\", true, CommonHelper.EnvironmentProvider);
    }
    
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_CURRENT_MISSING_ENDING_DIRECTORY_SEPARATOR_USING_BACK_SLASH()
    {
        var absolutePath = new AbsolutePath(".\\Aaa", true, CommonHelper.EnvironmentProvider);
    }

    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_ONE_UP_DIR_DIRECTIVES_WITH_ENDING_DIRECTORY_SEPARATOR_USING_FORWARD_SLASH()
    {
        var absolutePath = new AbsolutePath("../Todos/", true, CommonHelper.EnvironmentProvider);
    }
    
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_ONE_UP_DIR_DIRECTIVES_MISSING_ENDING_DIRECTORY_SEPARATOR_USING_FORWARD_SLASH()
    {
        var absolutePath = new AbsolutePath("../Todos", true, CommonHelper.EnvironmentProvider);
    }
    
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_ONE_UP_DIR_DIRECTIVES_WITH_ENDING_DIRECTORY_SEPARATOR_USING_BACK_SLASH()
    {
        var absolutePath = new AbsolutePath("..\\Todos\\", true, CommonHelper.EnvironmentProvider);
    }
    
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_ONE_UP_DIR_DIRECTIVES_MISSING_ENDING_DIRECTORY_SEPARATOR_USING_BACK_SLASH()
    {
        var absolutePath = new AbsolutePath("..\\Todos", true, CommonHelper.EnvironmentProvider);
    }
    
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_THREE_UP_DIR_DIRECTIVES_WITH_ENDING_DIRECTORY_SEPARATOR_USING_FORWARD_SLASH()
    {
        var absolutePath = new AbsolutePath("../../../Homework/Math/", true, CommonHelper.EnvironmentProvider);
    }

    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_THREE_UP_DIR_DIRECTIVES_MISSING_ENDING_DIRECTORY_SEPARATOR_USING_FORWARD_SLASH()
    {
        var absolutePath = new AbsolutePath("../../../Homework/Math", true, CommonHelper.EnvironmentProvider);
    }
    
    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_THREE_UP_DIR_DIRECTIVES_WITH_ENDING_DIRECTORY_SEPARATOR_USING_BACK_SLASH()
    {
        var absolutePath = new AbsolutePath("..\\..\\..\\Homework\\Math\\", true, CommonHelper.EnvironmentProvider);
    }

    [Fact]
    public void SHOULD_RESOLVE_DIRECTORY_FROM_THREE_UP_DIR_DIRECTIVES_MISSING_ENDING_DIRECTORY_SEPARATOR_USING_BACK_SLASH()
    {
        var absolutePath = new AbsolutePath("..\\..\\..\\Homework\\Math", true, CommonHelper.EnvironmentProvider);
    }
}
