namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IAbsolutePath : IPath
{
    /// <summary>
    /// Only non null when specifying a drive to mount
    ///
    /// Example: "C:\\" gets stored as "C"
    ///
    /// To get the guaranteed non null root directory
    /// use GetRootDirectory
    /// </summary>
    public IFileSystemDrive? RootDrive { get; }
    /// <summary>
    /// Returns either System.IO.Path.DirectorySeparatorChar
    ///
    /// OR
    ///
    /// Returns $"{RootDrive}:{System.IO.Path.DirectorySeparatorChar}"
    /// </summary>
    public IAbsolutePath? ParentDirectory { get; }
    public bool IsRootDirectory { get; }
    /// <summary>
    /// Don't pull <see cref="AncestorDirectoryBag"/> up to <see cref="IPath"/>. If done, one loses the
    /// <see cref="IAbsolutePath"/> typing
    /// </summary>
    public List<IAbsolutePath> AncestorDirectoryBag { get; }
}
