using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Fluxor;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

/// <summary>
/// <see cref="Terminal"/>
/// </summary>
public class TerminalTests
{
    /// <summary>
    /// <see cref="Terminal(string, string?, IDispatcher, IBackgroundTaskService, ITextEditorService, ILuthetusCommonComponentRenderers, ICompilerServiceRegistry)"/>
    /// <br/>----<br/>
    /// <see cref="Terminal.Key"/>
    /// <see cref="Terminal.WorkingDirectoryAbsolutePathString"/>
    /// <see cref="Terminal.ActiveTerminalCommand"/>
    /// <see cref="Terminal.HasExecutingProcess"/>
    /// <see cref="Terminal.TerminalCommandsHistory"/>
    /// <see cref="Terminal.ResourceUri"/>
    /// <see cref="Terminal.TextEditorViewModelKey"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="Terminal.EnqueueCommandAsync(TerminalCommand)"/>
    /// </summary>
    [Fact]
    public void EnqueueCommandAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="Terminal.ClearStandardOut()"/>
    /// </summary>
    [Fact]
    public void ClearStandardOut_NO_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="Terminal.ClearStandardOut(Key{TerminalCommand})"/>
    /// </summary>
    [Fact]
    public void ClearStandardOut_WITH_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="Terminal.KillProcess()"/>
    /// </summary>
    [Fact]
    public void KillProcess()
    {
        throw new NotImplementedException();
    }
}