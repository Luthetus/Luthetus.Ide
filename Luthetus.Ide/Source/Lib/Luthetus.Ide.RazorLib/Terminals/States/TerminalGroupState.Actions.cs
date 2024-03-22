using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record TerminalGroupState
{
    public record SetActiveTerminalSessionAction(Key<TerminalSession> TerminalSessionKey);
}
