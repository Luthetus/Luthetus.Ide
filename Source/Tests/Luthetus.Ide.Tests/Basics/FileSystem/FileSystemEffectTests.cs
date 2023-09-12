using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Xunit;

namespace Luthetus.Ide.Tests.Basics.FileSystem;

public class FileSystemEffectTests : LuthetusFileSystemTestingBase
{
    [Fact]
    public void SaveFile()
    {
        var content = "abc123";

        var absolutePath = new AbsolutePath(
            @"C:\Users\hunte\Desktop\TestLuthetus\apple.txt",
            true,
            EnvironmentProvider);

        var saveFileAction = new ClassLib.Store.FileSystemCase.FileSystemRegistry.SaveFileAction(
            absolutePath,
            content,
            writtenDateTime => { });

        Dispatcher.Dispatch(saveFileAction);
    }
}