namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IAbsolutePath : IPath
{
    public IFileSystemDrive? RootDrive { get; }
    public bool IsRootDirectory { get; }
}
