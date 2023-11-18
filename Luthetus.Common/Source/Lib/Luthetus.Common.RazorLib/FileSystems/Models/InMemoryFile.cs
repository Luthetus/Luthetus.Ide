namespace Luthetus.Common.RazorLib.FileSystems.Models;

public record InMemoryFile(
    string Data,
    IAbsolutePath AbsolutePath,
    DateTime LastModifiedDateTime,
    bool IsDirectory);