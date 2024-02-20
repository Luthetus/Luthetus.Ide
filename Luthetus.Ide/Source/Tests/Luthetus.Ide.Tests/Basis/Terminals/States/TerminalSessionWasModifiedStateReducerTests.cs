using Fluxor;

namespace Luthetus.Ide.Tests.Basis.Terminals.States;

public partial record TerminalSessionWasModifiedStateReducerTests
{
    [ReducerMethod]
    public static TerminalSessionWasModifiedState ReduceSetTerminalSessionStateKeyAction(
        TerminalSessionWasModifiedState inState,
        SetTerminalSessionStateKeyAction setTerminalSessionStateKeyAction)
    {
        if (inState.TerminalSessionWasModifiedMap.ContainsKey(setTerminalSessionStateKeyAction.TerminalSessionKey))
        {
            var nextMap = inState.TerminalSessionWasModifiedMap.SetItem(
                setTerminalSessionStateKeyAction.TerminalSessionKey,
                setTerminalSessionStateKeyAction.StateKey);

            return inState with { TerminalSessionWasModifiedMap = nextMap };
        }
        else
        {
            var nextMap = inState.TerminalSessionWasModifiedMap.Add(
                setTerminalSessionStateKeyAction.TerminalSessionKey,
                setTerminalSessionStateKeyAction.StateKey);

            return inState with { TerminalSessionWasModifiedMap = nextMap };
        }
    }
}