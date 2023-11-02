using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.Tests;

namespace Luthetus.Common.Tests.UserStories.FileSystem.AbsolutePathTests;

public class AbsoluteDirectoryTests : CommonTestingBase
{
    [Fact]
    public void ROOT_DIR()
    {
        var absolutePath = new AbsolutePath("/", true, CommonHelper.EnvironmentProvider);

        var zzz = absolutePath.FormattedInput;
    }
}
