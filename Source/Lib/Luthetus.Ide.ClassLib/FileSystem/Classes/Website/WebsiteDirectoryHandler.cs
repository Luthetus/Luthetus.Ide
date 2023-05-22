using System.Net.Http.Json;
using Luthetus.Common.RazorLib.Store.AccountCase;
using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.Website;

public class WebsiteDirectoryHandler : IDirectoryHandler
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<AccountState> _accountStateWrap;
    private readonly HttpClient _httpClient;

    public WebsiteDirectoryHandler(
        IEnvironmentProvider environmentProvider,
        IState<AccountState> accountStateWrap,
        HttpClient httpClient)
    {
        _environmentProvider = environmentProvider;
        _accountStateWrap = accountStateWrap;
        _httpClient = httpClient;
    }

    public async Task CreateDirectoryAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(CreateDirectoryAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);

        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryCreateDirectory?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken);
    }

    public async Task DeleteAsync(
        string absoluteFilePathString,
        bool recursive,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(DeleteAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);

        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryDelete?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}&" +
            $"recursive={recursive}",
            cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(ExistsAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);

        var response = await _httpClient.GetFromJsonAsync<bool>(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryExists?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken);

        return response;
    }

    public async Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(MoveAsync));

        throw new NotImplementedException();
    }

    public async Task<string[]> GetDirectoriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetDirectoriesAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);

        var requestUri = "https://hunter-freeman-dev-api.azurewebsites.net/" +
                         "FileSystem/" +
                         "DirectoryGetDirectories?" +
                         $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}";

        var response = await _httpClient.GetFromJsonAsync<string[]>(
                requestUri,
                cancellationToken);

        return response ?? new[] { string.Empty };
    }

    public async Task<string[]> GetFilesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetFilesAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);

        var response = await _httpClient.GetFromJsonAsync<string[]>(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryGetFiles?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken);

        return response ?? new[] { string.Empty };
    }

    public async Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(EnumerateFileSystemEntriesAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            true);

        var response = await _httpClient.GetFromJsonAsync<string[]>(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "DirectoryEnumerateFileSystemEntries?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken);

        return response ?? new[] { string.Empty };
    }
}