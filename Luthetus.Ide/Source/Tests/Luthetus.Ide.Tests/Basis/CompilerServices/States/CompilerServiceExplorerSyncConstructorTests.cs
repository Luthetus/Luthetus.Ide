using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.CompilerServices.States;

namespace Luthetus.Ide.Tests.Basis.CompilerServices.States;

/// <summary>
/// <see cref="CompilerServiceExplorerSync"/>
/// </summary>
public class CompilerServiceExplorerSyncConstructorTests
{
    /// <summary>
    /// <see cref="CompilerServiceExplorerSync(ILuthetusIdeComponentRenderers, ILuthetusCommonComponentRenderers, ITreeViewService, IState{CompilerServiceExplorerState}, IDecorationMapperRegistry, ICompilerServiceRegistry, IBackgroundTaskService, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="CompilerServiceExplorerSync.BackgroundTaskService"/>
    /// <see cref="CompilerServiceExplorerSync.Dispatcher"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }
}