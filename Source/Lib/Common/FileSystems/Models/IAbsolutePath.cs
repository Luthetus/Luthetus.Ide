namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IAbsolutePath : IPath
{
    public IFileSystemDrive? RootDrive { get; }
    public bool IsRootDirectory { get; }
    
    /// <summary>
    /// The previous implementation of this type was holding a property:
    /// ````public List<AncestorDirectory> AncestorDirectoryList { get; } = new();
    ///
    /// Just for it to rarely ever be used, and for it be creating many object instances for no reason.
    ///
    /// In order to keep the very small amount of code that was using this property happy,
    /// this method will calculate it "on demand".
    ///
    /// Returning/assigning a new AbsolutePath('s) '_ancestorDirectoryList is a bit of an oddity though.
    /// The reason for this is that the lexing was being done in the constructor.
    ///
    /// So, an optional parameter to the constructor was added such that a List<AncestorDirectory> could be
    /// provided, and then if so it would be filled with the ancestor directories.
    ///
    /// I have a "poor mans" count of the amount of AncestorDirectory instances prior to the change,
    /// so I'll have to see if this is any bit effective.
    /// </summary>
    public List<string> GetAncestorDirectoryList();

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
