namespace Luthetus.Ide.ClassLib.FileSystem.Interfaces;

public interface IFileHandler
{
    public Task<bool> ExistsAsync(string absoluteFilePathString, CancellationToken cancellationToken = default);
    public Task DeleteAsync(string absoluteFilePathString, CancellationToken cancellationToken = default);
    public Task CopyAsync(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString, CancellationToken cancellationToken = default);
    public Task MoveAsync(string sourceAbsoluteFilePathString, string destinationAbsoluteFilePathString, CancellationToken cancellationToken = default);
    public Task<DateTime> GetLastWriteTimeAsync(string absoluteFilePathString, CancellationToken cancellationToken = default);
    public Task<string> ReadAllTextAsync(string absoluteFilePathString, CancellationToken cancellationToken = default);
    public Task WriteAllTextAsync(string absoluteFilePathString, string contents, CancellationToken cancellationToken = default);
}