using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.Local;

public class LocalFileHandler : IFileHandler
{
    public Task<bool> ExistsAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            File.Exists(absoluteFilePathString));
    }

    public Task DeleteAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        File.Delete(absoluteFilePathString);

        return Task.CompletedTask;
    }

    public Task CopyAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        File.Copy(
            sourceAbsoluteFilePathString,
            destinationAbsoluteFilePathString);

        return Task.CompletedTask;
    }

    public Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        File.Move(
            sourceAbsoluteFilePathString,
            destinationAbsoluteFilePathString);

        return Task.CompletedTask;
    }

    public Task<DateTime> GetLastWriteTimeAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            File.GetLastWriteTime(
                absoluteFilePathString));
    }

    public async Task<string> ReadAllTextAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        return await File.ReadAllTextAsync(
            absoluteFilePathString,
            cancellationToken);
    }

    public async Task WriteAllTextAsync(
        string absoluteFilePathString,
        string contents,
        CancellationToken cancellationToken = default)
    {
        await File.WriteAllTextAsync(
            absoluteFilePathString,
            contents,
            cancellationToken);
    }
}