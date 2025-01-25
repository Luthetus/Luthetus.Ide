namespace Luthetus.Common.RazorLib.FileSystems.Models;

/// <summary>
/// Verify that 'Data is not null' to know the constructor was invoked rather than 'default'.
/// 'Data' is not allowed to be null.
/// </summary>
public record struct InMemoryFile(
    string Data,
    AbsolutePath AbsolutePath,
    DateTime LastModifiedDateTime,
    bool IsDirectory);