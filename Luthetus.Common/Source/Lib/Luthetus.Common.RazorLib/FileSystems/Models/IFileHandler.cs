namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IFileHandler
{
    public Task<bool> ExistsAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default);

    public Task DeleteAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default);

    public Task CopyAsync(
        string sourceAbsolutePathString,
        string destinationAbsolutePathString,
        CancellationToken cancellationToken = default);

    public Task MoveAsync(
        string sourceAbsolutePathString,
        string destinationAbsolutePathString,
        CancellationToken cancellationToken = default);

    public Task<DateTime> GetLastWriteTimeAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default);

    public Task<string> ReadAllTextAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default);

    public Task WriteAllTextAsync(
        string absolutePathString,
        string contents,
        CancellationToken cancellationToken = default);
}