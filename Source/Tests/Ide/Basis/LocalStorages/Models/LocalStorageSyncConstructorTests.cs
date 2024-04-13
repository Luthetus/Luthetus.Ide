using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.LocalStorages.Models;

namespace Luthetus.Ide.Tests.Basis.LocalStorages.Models;

/// <summary>
/// <see cref="LocalStorageSync"/>
/// </summary>
public partial class LocalStorageSyncConstructorTests
{
    /// <summary>
    /// <see cref="LocalStorageSync(IJSRuntime, IBackgroundTaskService, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="LocalStorageSync.BackgroundTaskService"/>
    /// <see cref="LocalStorageSync.Dispatcher"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }
}