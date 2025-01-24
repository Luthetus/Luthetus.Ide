namespace Luthetus.Common.RazorLib.FileSystems.Models;

/// <summary>
/// Given an <see cref="IRelativePath"/> then <see cref="AncestorDirectoryList"/> refers to the "../Todos/" in "../Todos/myFile.cs".
/// In this example there is 1 ancestor directory.<br/><br/>
/// </summary>
public interface IRelativePath : IPath
{
    /// <summary>
    /// <see cref="UpDirDirectiveCount"/> refers to the ".." in "../Todos/myFile.cs".
    /// In this example the count is 1.
    /// <br/><br/>
    /// Given "./Folder/file.txt". The count is 0.
    /// <br/><br/>
    /// Given "../../../Homework/math.txt". The count is 3.
    /// </summary>
    public int UpDirDirectiveCount { get; }

	/// <summary>
    /// The IAbsolutePath version was optimized,
    /// the IRelativePath still needs to be optimized.
    /// </summary>
    public List<(string NameWithExtension, string Path)> GetAncestorDirectoryList();

    /// <summary>
    /// One might prefer <see cref="IEnvironmentProvider.RelativePathFactory(string, bool)"/> instead
    /// of this method.<br/><br/>
    /// 
    /// This version adds an extra function invocation for no reason. To use
    /// <see cref="IEnvironmentProvider.RelativePathFactory(string, bool)"/> may be a negligible
    /// optimization however.<br/><br/>
    /// 
    /// Keep this method here, it provides more clear documentation on how to create an instance
    /// of <see cref="IRelativePath"/>. Having to invoke a method on the <see cref="IEnvironmentProvider"/>
    /// is a bit hard to find.
    /// </summary>
    public static IRelativePath Factory(string path, bool isDirectory, IEnvironmentProvider environmentProvider)
    {
        return environmentProvider.RelativePathFactory(path, isDirectory);
    }
}