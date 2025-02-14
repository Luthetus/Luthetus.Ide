using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

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