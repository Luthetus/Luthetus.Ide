namespace Luthetus.Extensions.Git.Models;

public record GitTask(
    Guid Id,
    string DisplayName,
    CancellationToken CancellationToken);