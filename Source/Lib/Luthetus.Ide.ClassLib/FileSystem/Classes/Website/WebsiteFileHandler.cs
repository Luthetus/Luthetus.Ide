using Fluxor;
using Luthetus.Common.RazorLib.Store.AccountCase;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.Website;

public class WebsiteFileHandler : IFileHandler
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<AccountState> _accountStateWrap;
    private readonly HttpClient _httpClient;

    public WebsiteFileHandler(
        IEnvironmentProvider environmentProvider,
        IState<AccountState> accountStateWrap,
        HttpClient httpClient)
    {
        _environmentProvider = environmentProvider;
        _accountStateWrap = accountStateWrap;
        _httpClient = httpClient;
    }

    public async Task<bool> ExistsAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(ExistsAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            false);

        var response = await _httpClient.GetFromJsonAsync<bool>(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "FileExists?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken);

        return response;
    }

    public async Task DeleteAsync(string absoluteFilePathString, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(DeleteAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            false);

        await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "FileDelete?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken);
    }

    public async Task CopyAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(CopyAsync));

        throw new NotImplementedException();
    }

    public async Task MoveAsync(
        string sourceAbsoluteFilePathString,
        string destinationAbsoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(MoveAsync));

        throw new NotImplementedException();
    }

    public async Task<DateTime> GetLastWriteTimeAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(GetLastWriteTimeAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            false);

        var response = await _httpClient.GetFromJsonAsync<DateTime>(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "FileGetLastWriteTime?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken);

        return response;
    }

    public async Task<string> ReadAllTextAsync(
        string absoluteFilePathString,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(ReadAllTextAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            false);

        var response = await _httpClient.GetAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "FileReadAllText?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}",
            cancellationToken);

        return await response.Content
            .ReadAsStringAsync(cancellationToken);
    }

    public async Task WriteAllTextAsync(
        string absoluteFilePathString,
        string contents,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(nameof(WriteAllTextAsync));

        absoluteFilePathString = PathFormatter.FormatAbsoluteFilePathString(
            absoluteFilePathString,
            AccountState.DIRECTORY_SEPARATOR_CHAR,
            false);

        // if (contents.Length > FileSystemState.MAXIMUM_CHARACTER_COUNT_OF_CONTENT)
        // {
        //     contents = new string(contents
        //         .Take(FileSystemState.MAXIMUM_CHARACTER_COUNT_OF_CONTENT)
        //         .ToArray());
        // }
        // else if (string.IsNullOrWhiteSpace(contents))
        // {
        //     contents = "Sample Text";
        // }

        contents = Uri.EscapeDataString(contents);

        await _httpClient.PostAsync(
            "https://hunter-freeman-dev-api.azurewebsites.net/" +
            "FileSystem/" +
            "FileWriteAllText?" +
            $"absoluteFilePathString={Uri.EscapeDataString(absoluteFilePathString)}&" +
            $"contents={contents}",
            null,
            cancellationToken);
    }
}