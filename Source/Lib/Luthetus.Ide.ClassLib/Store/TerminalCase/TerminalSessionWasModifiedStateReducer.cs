using Fluxor;
using Luthetus.Ide.ClassLib.State;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

public class TerminalSessionWasModifiedStateReducer
{
    public record SetTerminalSessionStateKeyAction(
        TerminalSessionKey TerminalSessionKey,
        StateKey StateKey);

    [ReducerMethod]
    public static TerminalSessionWasModifiedState ReduceSetTerminalSessionStateKeyAction(
        TerminalSessionWasModifiedState inTerminalSessionWasModifiedState,
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