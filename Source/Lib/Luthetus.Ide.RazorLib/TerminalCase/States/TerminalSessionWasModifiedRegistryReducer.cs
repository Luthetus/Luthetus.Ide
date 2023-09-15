using Fluxor;
using Luthetus.Ide.RazorLib.StateCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.Models;

namespace Luthetus.Ide.RazorLib.TerminalCase.States;

public class TerminalSessionWasModifiedRegistryReducer
{
    public record SetTerminalSessionStateKeyAction(
        TerminalSessionKey TerminalSessionKey,
        StateKey StateKey);

    [ReducerMethod]
    public static TerminalSessionWasModifiedRegistry ReduceSetTerminalSessionStateKeyAction(
        TerminalSessionWasModifiedRegistry inTerminalSessionWasModifiedState,
        SetTerminalSessionStateKeyAction setTerminalSessionStateKeyAction)
    {
        if (inTerminalSessionWasModifiedState.TerminalSessionWasModifiedMap.ContainsKey(
                setTerminalSessionStateKeyAction.TerminalSessionKey))
        {
            var nextMap = inTerminalSessionWasModifiedState.TerminalSessionWasModifiedMap
                .SetItem(
                    setTerminalSessionStateKeyAction.TerminalSessionKey,
                    setTerminalSessionStateKeyAction.StateKey);

            return inTerminalSessionWasModifiedState with
            {
                TerminalSessionWasModifiedMap = nextMap
            };
        }
        else
        {
            var nextMap = inTerminalSessionWasModifiedState.TerminalSessionWasModifiedMap
                .Add(
                    setTerminalSessionStateKeyAction.TerminalSessionKey,
                    setTerminalSessionStateKeyAction.StateKey);

            return inTerminalSessionWasModifiedState with
            {
                TerminalSessionWasModifiedMap = nextMap
            };
        }
    }
}