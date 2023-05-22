namespace Luthetus.Ide.ClassLib.FileSystem.Interfaces;

/// <summary>
/// TODO: I was not aware of <see cref="DirectoryInfo"/> nor <see cref="FileInfo"/> when I started this project. I will when I find time remedy this mistake and change over to Microsoft's file system classes.
/// </summary>
public interface IAbsoluteFilePath : IFilePath
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
    public string GetRootDirectory { get; }
    public string GetAbsoluteFilePathString();
    public AbsoluteFilePathKind AbsoluteFilePathKind { get; }
    public IAbsoluteFilePath? ParentDirectory { get; }
}