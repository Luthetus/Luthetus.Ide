namespace Luthetus.Common.RazorLib.FileSystems.Models;

public record InMemoryFileTests(
    string Data,
    IAbsolutePath AbsolutePath,
    DateTime LastModifiedDateTime);