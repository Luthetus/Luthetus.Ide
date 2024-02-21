using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

/// <summary>
/// <see cref="TerminalSession"/>
/// </summary>
public class TerminalSessionTests
{
    /// <summary>
    /// <see cref="TerminalSession(string?, IDispatcher, IBackgroundTaskService, ILuthetusCommonComponentRenderers)"/>
    /// <br/>----<br/>
    /// <see cref="TerminalSession.TerminalSessionKey"/>
    /// <see cref="TerminalSession.WorkingDirectoryAbsolutePathString"/>
    /// <see cref="TerminalSession.ActiveTerminalCommand"/>
    /// <see cref="TerminalSession.HasExecutingProcess"/>
    /// <see cref="TerminalSession.TerminalCommandsHistory"/>
    /// <see cref="TerminalSession.ResourceUri"/>
    /// <see cref="TerminalSession.TextEditorViewModelKey"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TerminalSession.ReadStandardOut()"/>
    /// </summary>
    [Fact]
    public void ReadStandardOut_NO_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TerminalSession.ReadStandardOut(Key{TerminalCommand})"/>
    /// </summary>
    [Fact]
    public void ReadStandardOut_WITH_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TerminalSession.GetStandardOut()"/>
    /// </summary>
    [Fact]
    public void GetStandardOut_NO_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TerminalSession.GetStandardOut(Key{TerminalCommand})"/>
    /// </summary>
    [Fact]
    public void GetStandardOut_WITH_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TerminalSession.EnqueueCommandAsync(TerminalCommand)"/>
    /// </summary>
    [Fact]
    public void EnqueueCommandAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TerminalSession.ClearStandardOut()"/>
    /// </summary>
    [Fact]
    public void ClearStandardOut_NO_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TerminalSession.ClearStandardOut(Key{TerminalCommand})"/>
    /// </summary>
    [Fact]
    public void ClearStandardOut_WITH_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TerminalSession.KillProcess()"/>
    /// </summary>
    [Fact]
    public void KillProcess()
    {
        throw new NotImplementedException();
    }
}