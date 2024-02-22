using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.States.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.Tests.Basis.Terminals.States;

/// <summary>
/// <see cref="TerminalSessionWasModifiedState"/>
/// </summary>
public class TerminalSessionWasModifiedStateMainTests
{
    /// <summary>
    /// <see cref="TerminalSessionWasModifiedState(ImmutableDictionary{Key{TerminalSession}, Key{StateRecord}}, string)"/>
    /// <br/>----<br/>
    /// <see cref="TerminalSessionWasModifiedState.TerminalSessionWasModifiedMap"/>
    /// <see cref="TerminalSessionWasModifiedState.EmptyTextHack"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }
}