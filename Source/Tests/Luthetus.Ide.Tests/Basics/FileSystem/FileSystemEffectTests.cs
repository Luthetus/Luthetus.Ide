namespace Luthetus.Ide.Tests.Basics.FileSystem;

public class FileSystemEffectTests : LuthetusFileSystemTestingBase
{
    [Fact]
    public void SaveFile()
    {
        var content = "abc123";

        var absoluteFilePath = new AbsoluteFilePath(
            @"C:\Users\hunte\Desktop\TestLuthetus\apple.txt",
            true,
            EnvironmentProvider);

        var saveFileAction = new FileSystemState.SaveFileAction(
            absoluteFilePath,
            content,
            writtenDateTime => { });

        Dispatcher.Dispatch(saveFileAction);
    }
}