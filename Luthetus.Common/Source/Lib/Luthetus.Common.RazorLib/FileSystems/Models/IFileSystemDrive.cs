namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IFileSystemDrive
{
    public string DriveNameAsIdentifier { get; }
    public string DriveNameAsPath { get; }
}