using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record class TerminalGroupState
{
    public static class Reducer
    {
        [ReducerMethod]
        public static TerminalGroupState ReduceSetActiveTerminalSessionAction(
            TerminalGroupState inState,
            SetActiveTerminalSessionAction setActiveTerminalSessionAction)
        {
            return inState with
            {
                ActiveTerminalSessionKey = setActiveTerminalSessionAction.TerminalSessionKey
            };
        }
    }
}
