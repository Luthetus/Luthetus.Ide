using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.Local;

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