namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class LocalDirectoryHandler : IDirectoryHandler
{
    public Task CreateDirectoryAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(absolutePathString);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        string absolutePathString,
        bool recursive,
        CancellationToken cancellationToken = default)
    {
        Directory.Delete(absolutePathString, recursive);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Directory.Exists(
                absolutePathString));
    }

    public Task CopyAsync(
        string sourceAbsolutePathString,
        string destinationAbsolutePathString,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task MoveAsync(
        string sourceAbsolutePathString,
        string destinationAbsolutePathString,
        CancellationToken cancellationToken = default)
    {
        Directory.Move(
            sourceAbsolutePathString,
            destinationAbsolutePathString);

        return Task.CompletedTask;
    }

    public Task<string[]> GetDirectoriesAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Directory.GetDirectories(
                absolutePathString));
    }

    public Task<string[]> GetFilesAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Directory.GetFiles(
                absolutePathString));
    }

    public Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Directory.EnumerateFileSystemEntries(
                absolutePathString));
    }
}