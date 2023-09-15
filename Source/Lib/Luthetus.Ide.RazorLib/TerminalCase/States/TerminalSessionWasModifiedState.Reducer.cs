using Fluxor;

namespace Luthetus.Ide.RazorLib.TerminalCase.States;

public partial record TerminalSessionWasModifiedState
{
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