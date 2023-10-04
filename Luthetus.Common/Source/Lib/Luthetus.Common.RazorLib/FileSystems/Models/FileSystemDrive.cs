namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class FileSystemDrive : IFileSystemDrive
{
    public FileSystemDrive(
        string driveNameAsIdentifier,
        IEnvironmentProvider environmentProvider)
    {
        DriveNameAsIdentifier = driveNameAsIdentifier;
        EnvironmentProvider = environmentProvider;
    }

    /// <summary>
    /// Given: "C:/Users"<br/>
    /// Then <see cref="DriveNameAsIdentifier"/> is "C"
    /// </summary>
    public string DriveNameAsIdentifier { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }
    /// <summary>
    /// Given: "C:/Users"<br/>
    /// Then <see cref="DriveNameAsPath"/> is "C:"
    /// </summary>
    public string DriveNameAsPath => $"{DriveNameAsIdentifier}:";
}