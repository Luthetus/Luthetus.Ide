using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.Local;

public class LocalDirectoryHandler : IDirectoryHandler
{
    public Task CreateDirectoryAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(absoluteFilePathString);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        string absoluteFilePathString,
        bool recursive,
        CancellationToken cancellationToken = default)
    {
        Directory.Delete(absoluteFilePathString, recursive);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Directory.Exists(
                absoluteFilePathString));
    }

    public Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Directory.Move(
            sourceAbsoluteFilePathString,
            destinationAbsoluteFilePathString);

        return Task.CompletedTask;
    }

    public Task<string[]> GetDirectoriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Directory.GetDirectories(
                absoluteFilePathString));
    }

    public Task<string[]> GetFilesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Directory.GetFiles(
                absoluteFilePathString));
    }

    public Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Directory.EnumerateFileSystemEntries(
                absoluteFilePathString));
    }
}