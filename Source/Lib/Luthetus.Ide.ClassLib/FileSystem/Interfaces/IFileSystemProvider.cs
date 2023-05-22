namespace Luthetus.Ide.ClassLib.FileSystem.Interfaces;

public interface IFileSystemProvider
{
    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}