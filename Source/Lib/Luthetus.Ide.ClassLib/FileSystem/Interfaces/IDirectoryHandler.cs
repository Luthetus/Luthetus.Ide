namespace Luthetus.Ide.ClassLib.FileSystem.Interfaces;

public interface IDirectoryHandler
{
    public Task CreateDirectoryAsync(string absoluteFilePathString, CancellationToken cancellationToken = default);
    public Task DeleteAsync(string absoluteFilePathString, bool recursive, CancellationToken cancellationToken = default);
    public Task<bool> ExistsAsync(string absoluteFilePathString, CancellationToken cancellationToken = default);
    public Task MoveAsync(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString, CancellationToken cancellationToken = default);
    public Task<string[]> GetDirectoriesAsync(string absoluteFilePathString, CancellationToken cancellationToken = default);
    public Task<string[]> GetFilesAsync(string absoluteFilePathString, CancellationToken cancellationToken = default);
    public Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(string absoluteFilePathString, CancellationToken cancellationToken = default);
}