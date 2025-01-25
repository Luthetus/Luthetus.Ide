namespace Luthetus.Common.RazorLib.FileSystems.Models;

public struct FileSystemDrive
{
	private string? _driveNameAsPath;

    public FileSystemDrive(string driveNameAsIdentifier)
    {
        DriveNameAsIdentifier = driveNameAsIdentifier;
    }
    
    public string? DriveNameAsIdentifier { get; }
    public string? DriveNameAsPath => _driveNameAsPath ??= $"{DriveNameAsIdentifier}:";
}