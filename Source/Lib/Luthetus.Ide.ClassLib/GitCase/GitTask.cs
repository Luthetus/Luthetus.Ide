namespace Luthetus.Ide.ClassLib.GitCase;

public record GitTask(
    Guid Id,
    string DisplayName,
    object Action,
    CancellationToken CancellationToken);