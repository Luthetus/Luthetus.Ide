namespace Luthetus.Ide.RazorLib.GitCase;

public record GitTask(
    Guid Id,
    string DisplayName,
    object Action,
    CancellationToken CancellationToken);