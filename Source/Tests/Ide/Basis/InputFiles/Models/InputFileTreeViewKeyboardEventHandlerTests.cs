using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.Tests.Basis.InputFiles.Models;

/// <summary>
/// <see cref="InputFileTreeViewKeyboardEventHandler"/>
/// </summary>
public class InputFileTreeViewKeyboardEventHandlerTests
{
    /// <summary>
    /// <see cref="InputFileTreeViewKeyboardEventHandler(ITreeViewService, IState{InputFileState}, IDispatcher, ILuthetusIdeComponentRenderers, ILuthetusCommonComponentRenderers, IFileSystemProvider, IEnvironmentProvider, Func{IAbsolutePath, Task}, Func{Task}, Func{List{ValueTuple{Key{TreeViewContainer}, TreeViewAbsolutePath}}}, IBackgroundTaskService)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="InputFileTreeViewKeyboardEventHandler.OnKeyDownAsync(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public void OnKeyDownAsync()
    {
        throw new NotImplementedException();
    }
}