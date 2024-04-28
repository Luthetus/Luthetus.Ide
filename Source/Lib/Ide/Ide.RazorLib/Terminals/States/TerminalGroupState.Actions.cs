using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record TerminalGroupState
{
    public record SetActiveTerminalAction(Key<Terminal> TerminalKey);
}
