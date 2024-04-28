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
}