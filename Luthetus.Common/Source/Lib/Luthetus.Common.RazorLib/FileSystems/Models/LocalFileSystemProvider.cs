namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class LocalFileSystemProvider : IFileSystemProvider
{
    public LocalFileSystemProvider()
    {
        File = new LocalFileHandler();
        Directory = new LocalDirectoryHandler();
    }

    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}