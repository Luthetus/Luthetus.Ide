namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IDirectoryHandler
{
    public Task CreateDirectoryAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default);

    public Task DeleteAsync(
        string absolutePathString,
        bool recursive,
        CancellationToken cancellationToken = default);

    public Task<bool> ExistsAsync(
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

    public Task<string[]> GetDirectoriesAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default);

    public Task<string[]> GetFilesAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default);

    public Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default);
}