using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.UserStories.FileSystem.AbsolutePathTests;

public class AbsoluteDirectoryTests : PathTestsBase
{
    [Fact]
    public void ROOT_DIR()
    {
        var absolutePath = new AbsolutePath("/", true, EnvironmentProvider);

        var zzz = absolutePath.FormattedInput;
    }
}
