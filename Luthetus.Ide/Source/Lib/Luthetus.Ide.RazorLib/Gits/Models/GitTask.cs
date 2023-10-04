namespace Luthetus.Ide.RazorLib.Gits.Models;

public record GitTask(
    Guid Id,
    string DisplayName,
    CancellationToken CancellationToken);