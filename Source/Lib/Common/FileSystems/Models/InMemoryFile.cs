namespace Luthetus.Common.RazorLib.FileSystems.Models;

public record struct InMemoryFile(
    string Data,
    IAbsolutePath AbsolutePath,
    DateTime LastModifiedDateTime,
    bool IsDirectory);