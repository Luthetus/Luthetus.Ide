using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.FileSystem.Classes.Website;

public class WebsiteFileSystemProvider : IFileSystemProvider
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<AccountState> _accountStateWrap;
    private readonly HttpClient _httpClient;

    public WebsiteFileSystemProvider(
        IEnvironmentProvider environmentProvider,
        IState<AccountState> accountStateWrap,
        HttpClient httpClient)
    {
        _environmentProvider = environmentProvider;
        _accountStateWrap = accountStateWrap;
        _httpClient = httpClient;

        File = new WebsiteFileHandler(
            environmentProvider,
            accountStateWrap,
            httpClient);

        Directory = new WebsiteDirectoryHandler(
            environmentProvider,
            accountStateWrap,
            httpClient);
    }

    public IFileHandler File { get; set; }
    public IDirectoryHandler Directory { get; set; }
}