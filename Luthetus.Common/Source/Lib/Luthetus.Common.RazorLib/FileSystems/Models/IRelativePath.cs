namespace Luthetus.Common.RazorLib.FileSystems.Models;

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
    /// Given an <see cref="IRelativePath"/> then <see cref="AncestorDirectoryBag"/> refers to the "../Todos/" in "../Todos/myFile.cs".
    /// In this example there is 1 ancestor directory.<br/><br/>
    /// 
    /// Don't pull <see cref="AncestorDirectoryBag"/> up to <see cref="IPath"/>. If done, one loses the
    /// <see cref="IRelativePath"/> typing
    /// </summary>
    public List<IRelativePath> AncestorDirectoryBag { get; }
}