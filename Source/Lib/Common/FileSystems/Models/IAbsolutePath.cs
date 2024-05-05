namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IAbsolutePath : IPath
{
    public IFileSystemDrive? RootDrive { get; }
    public bool IsRootDirectory { get; }

    /// <summary>
    /// One might prefer <see cref="IEnvironmentProvider.AbsolutePathFactory(string, bool)"/> instead
    /// of this method.<br/><br/>
    /// 
    /// This version adds an extra function invocation for no reason. To use
    /// <see cref="IEnvironmentProvider.AbsolutePathFactory(string, bool)"/> may be a negligible
    /// optimization however.<br/><br/>
    /// 
    /// Keep this method here, it provides more clear documentation on how to create an instance
    /// of <see cref="IAbsolutePath"/>. Having to invoke a method on the <see cref="IEnvironmentProvider"/>
    /// is a bit hard to find.
    /// </summary>
    public static IAbsolutePath Factory(string path, bool isDirectory, IEnvironmentProvider environmentProvider)
    {
        return environmentProvider.AbsolutePathFactory(path, isDirectory);
    }
}
