using Fluxor;
using static Luthetus.Ide.RazorLib.DotNetSolutions.States.DotNetSolutionSync;

namespace Luthetus.Ide.Tests.Basis.DotNetSolutions.States;

public class DotNetSolutionStateReducerTests
{
    private class Reducer
    {
        [ReducerMethod]
        public static DotNetSolutionState ReduceRegisterAction(
            DotNetSolutionState inState,
            RegisterAction registerAction)
        {
            var dotNetSolutionModel = inState.DotNetSolutionModel;

            if (dotNetSolutionModel is not null)
                return inState;

            var nextList = inState.DotNetSolutionsList.Add(
                registerAction.DotNetSolutionModel);

            return inState with
            {
                DotNetSolutionsList = nextList
            };
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceDisposeAction(
            DotNetSolutionState inState,
            DisposeAction disposeAction)
        {
            var dotNetSolutionModel = inState.DotNetSolutionModel;

            if (dotNetSolutionModel is null)
                return inState;

            var nextList = inState.DotNetSolutionsList.Remove(
                dotNetSolutionModel);

            return inState with
            {
                DotNetSolutionsList = nextList
            };
        }

        [ReducerMethod]
        public static DotNetSolutionState ReduceWithAction(
            DotNetSolutionState inState,
            IWithAction withActionInterface)
        {
            var withAction = (WithAction)withActionInterface;
            return withAction.WithFunc.Invoke(inState);
        }
    }
}