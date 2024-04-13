namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IFileSystemProvider
{
    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}