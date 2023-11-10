namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IFileSystemDrive
{
    /// <summary>
    /// Given: "C:/Users"<br/>
    /// Then <see cref="DriveNameAsIdentifier"/> is "C"
    /// </summary>
    public string DriveNameAsIdentifier { get; }
    /// <summary>
    /// Given: "C:/Users"<br/>
    /// Then <see cref="DriveNameAsPath"/> is "C:"
    /// </summary>
    public string DriveNameAsPath { get; }
}