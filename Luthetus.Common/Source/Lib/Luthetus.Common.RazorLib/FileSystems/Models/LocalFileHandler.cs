namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class LocalFileHandler : IFileHandler
{
    public Task<bool> ExistsAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            File.Exists(absolutePathString));
    }

    public Task DeleteAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        File.Delete(absolutePathString);

        return Task.CompletedTask;
    }

    public Task CopyAsync(
        string sourceAbsolutePathString,
        string destinationAbsolutePathString,
        CancellationToken cancellationToken = default)
    {
        File.Copy(
            sourceAbsolutePathString,
            destinationAbsolutePathString);

        return Task.CompletedTask;
    }

    public Task MoveAsync(
        string sourceAbsolutePathString,
        string destinationAbsolutePathString,
        CancellationToken cancellationToken = default)
    {
        File.Move(
            sourceAbsolutePathString,
            destinationAbsolutePathString);

        return Task.CompletedTask;
    }

    public Task<DateTime> GetLastWriteTimeAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            File.GetLastWriteTime(
                absolutePathString));
    }

    public async Task<string> ReadAllTextAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        return await File.ReadAllTextAsync(
            absolutePathString,
            cancellationToken);
    }

    public async Task WriteAllTextAsync(
        string absolutePathString,
        string contents,
        CancellationToken cancellationToken = default)
    {
        await File.WriteAllTextAsync(
            absolutePathString,
            contents,
            cancellationToken);
    }
}