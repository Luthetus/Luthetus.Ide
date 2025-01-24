namespace Luthetus.Common.RazorLib.FileSystems.Models;

public record InMemoryFile(
    string Data,
    AbsolutePath AbsolutePath,
    DateTime LastModifiedDateTime,
    bool IsDirectory);