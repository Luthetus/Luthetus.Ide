namespace Luthetus.Common.RazorLib.FileSystems.Models;

public struct FileSystemDrive : IFileSystemDrive
{
    public FileSystemDrive(
        string driveNameAsIdentifier,
        IEnvironmentProvider environmentProvider)
    {
        DriveNameAsIdentifier = driveNameAsIdentifier;
        EnvironmentProvider = environmentProvider;
    }
    
    public string DriveNameAsIdentifier { get; }
    public string DriveNameAsPath => $"{DriveNameAsIdentifier}:";
    public IEnvironmentProvider EnvironmentProvider { get; }
}