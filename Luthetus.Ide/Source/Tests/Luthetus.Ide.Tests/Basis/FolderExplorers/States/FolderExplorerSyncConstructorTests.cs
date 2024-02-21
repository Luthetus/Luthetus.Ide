using Fluxor;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.FolderExplorers.States;

namespace Luthetus.Ide.Tests.Basis.FolderExplorers.States;

/// <summary>
/// <see cref="FolderExplorerSync"/>
/// </summary>
public class FolderExplorerSyncConstructorTests
{
    /// <summary>
    /// <see cref="FolderExplorerSync(IFileSystemProvider, IEnvironmentProvider, ILuthetusIdeComponentRenderers, ILuthetusCommonComponentRenderers, ITreeViewService, InputFileSync, IBackgroundTaskService, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="FolderExplorerSync.BackgroundTaskService"/>
    /// <see cref="FolderExplorerSync.Dispatcher"/>
    /// <see cref="FolderExplorerSync.InputFileSync"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }
}
