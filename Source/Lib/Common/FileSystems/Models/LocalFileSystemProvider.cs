using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class LocalFileSystemProvider : IFileSystemProvider
{
    public LocalFileSystemProvider(LuthetusCommonApi commonApi)
    {
        File = new LocalFileHandler(commonApi.EnvironmentProviderApi, commonApi.ComponentRendererApi, commonApi.NotificationApi);
        Directory = new LocalDirectoryHandler(commonApi.EnvironmentProviderApi, commonApi.ComponentRendererApi, commonApi.NotificationApi);
    }

    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
}