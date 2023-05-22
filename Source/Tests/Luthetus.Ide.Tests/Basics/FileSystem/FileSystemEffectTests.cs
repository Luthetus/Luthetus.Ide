using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.Store.FileSystemCase;

namespace Luthetus.Ide.Tests.Basics.FileSystem;

public class FileSystemEffectTests : BlazorStudioFileSystemTestingBase
{
    [Fact]
    public void SaveFile()
    {
        var content = "abc123";

        var absoluteFilePath = new AbsoluteFilePath(
            @"C:\Users\hunte\Desktop\TestBlazorStudio\apple.txt",
            true,
            EnvironmentProvider);

        var saveFileAction = new FileSystemState.SaveFileAction(
            absoluteFilePath,
            content,
            () => { });

        Dispatcher.Dispatch(saveFileAction);
    }
}