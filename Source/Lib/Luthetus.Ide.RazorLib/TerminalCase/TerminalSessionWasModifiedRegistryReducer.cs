using Fluxor;
using Luthetus.Ide.RazorLib.StateCase;

namespace Luthetus.Ide.RazorLib.TerminalCase;

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