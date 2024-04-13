namespace Luthetus.Common.RazorLib.FileSystems.Models;

/// <summary>
/// A side comment: Windows allow both '\\' and '/' as file delimiters.
/// Be sure to check both System.IO.Path.DirectorySeparatorChar and
/// System.IO.Path.AltDirectorySeparatorChar
/// </summary>
public interface IPath
{
    public PathType PathType { get; }
    public bool IsDirectory { get; }
    public string NameNoExtension { get; }
    /// <summary>
    /// Given: { "somePath.txt", NOT_DIRECTORY }<br/>
    /// Then: <see cref="NameWithExtension"/> is "somePath.txt"
    /// <br/><br/>
    /// Given: { "somePath.txt", IS_DIRECTORY }<br/>
    /// Then: <see cref="NameWithExtension"/> is "somePath.txt/"
    /// <br/><br/>
    /// Given: { "somePath", NOT_DIRECTORY }<br/>
    /// Then: <see cref="NameWithExtension"/> is "somePath"
    /// <br/><br/>
    /// Given: { "somePath", IS_DIRECTORY }<br/>
    /// Then: <see cref="NameWithExtension"/> is "somePath/"
    /// <br/><br/>
    /// Given: { "somePath/", NOT_DIRECTORY }<br/>
    /// Then: throw new Exception('File names cannot contain directory separators');
    /// <br/><br/>
    /// Given: { "somePath/", IS_DIRECTORY }<br/>
    /// Then: <see cref="NameWithExtension"/> is "somePath/"
    /// </summary>
    public string NameWithExtension { get; }
    public string ExtensionNoPeriod { get; }
    /// <summary>
    /// Given: "/Repos\\BlazorCrudApp\\"<br/>
    /// Then: <see cref="ExactInput"/> is "/Repos\\BlazorCrudApp\\"
    /// <br/><br/>
    /// Given: "\\Repos/BlazorCrudApp/"<br/>
    /// Then: <see cref="ExactInput"/> is "\\Repos/BlazorCrudApp/"
    /// <br/><br/>
    /// Overall reasoning: <see cref="ExactInput"/> is to return the user
    ///                    input exactly as they gave it originally.
    /// </summary>
    public string? ExactInput { get; }
    /// <summary>
    /// <see cref="Value"/> is the path as a string, but with all
    /// directory separating characters having been standardized
    /// to be <see cref="IEnvironmentProvider.DirectorySeparatorChar"/>.
    /// </summary>
    public string Value { get; }
    public AncestorDirectory? ParentDirectory { get; }
    public List<AncestorDirectory> AncestorDirectoryList { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }
}
