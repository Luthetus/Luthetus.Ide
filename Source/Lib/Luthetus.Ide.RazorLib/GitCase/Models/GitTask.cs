namespace Luthetus.Ide.RazorLib.GitCase.Models;

public record GitTask(
    Guid Id,
    string DisplayName,
    object Action,
    CancellationToken CancellationToken);