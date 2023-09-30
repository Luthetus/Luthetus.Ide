using Fluxor;
using static Luthetus.Ide.RazorLib.DotNetSolutionCase.States.DotNetSolutionSync;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial record DotNetSolutionState
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

            var nextList = inState.DotNetSolutions.Add(
                registerAction.DotNetSolutionModel);

            return inState with
            {
                DotNetSolutions = nextList
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

            var nextList = inState.DotNetSolutions.Remove(
                dotNetSolutionModel);

            return inState with
            {
                DotNetSolutions = nextList
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