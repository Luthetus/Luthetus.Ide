using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

/// <summary>
/// <see cref="Terminal"/>
/// </summary>
public class TerminalTests
{
    /// <summary>
    /// <see cref="Terminal(string?, IDispatcher, IBackgroundTaskService, ILuthetusCommonComponentRenderers)"/>
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
    /// <see cref="Terminal.ReadStandardOut()"/>
    /// </summary>
    [Fact]
    public void ReadStandardOut_NO_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="Terminal.ReadStandardOut(Key{TerminalCommand})"/>
    /// </summary>
    [Fact]
    public void ReadStandardOut_WITH_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="Terminal.GetStandardOut()"/>
    /// </summary>
    [Fact]
    public void GetStandardOut_NO_ARGS()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="Terminal.GetStandardOut(Key{TerminalCommand})"/>
    /// </summary>
    [Fact]
    public void GetStandardOut_WITH_ARGS()
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