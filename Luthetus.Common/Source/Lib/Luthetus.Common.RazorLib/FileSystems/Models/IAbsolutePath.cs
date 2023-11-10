namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IAbsolutePath : IPath
{
    public IFileSystemDrive? RootDrive { get; }
    public IAbsolutePath? ParentDirectory { get; }
    public bool IsRootDirectory { get; }
    /// <summary>
    /// Don't pull <see cref="AncestorDirectoryBag"/> up to <see cref="IPath"/>. If done, one loses the
    /// <see cref="IAbsolutePath"/> typing
    /// </summary>
    public List<IAbsolutePath> AncestorDirectoryBag { get; }
}
