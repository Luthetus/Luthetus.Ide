using Fluxor;

namespace Luthetus.Common.RazorLib.ComponentRunners.States;

public partial record ComponentRunnerState
{
    private record Reducer
    {
        [ReducerMethod]
        public static ComponentRunnerState ReduceRegisterAction(
            ComponentRunnerState inState,
            RegisterAction registerAction)
        {
            if (inState.ComponentRunnerDisplayStateBag.Any(x => x.Key == registerAction.Entry.Key))
                return inState;

            var insertionIndex = 0;

            if (registerAction.InsertionIndex >= 0 && registerAction.InsertionIndex < 1 + inState.ComponentRunnerDisplayStateBag.Count)
                insertionIndex = registerAction.InsertionIndex;

            var outDisplayStateBag = inState.ComponentRunnerDisplayStateBag.Insert(
                insertionIndex,
                registerAction.Entry);

            return new ComponentRunnerState { ComponentRunnerDisplayStateBag = outDisplayStateBag };
        }

        [ReducerMethod]
        public static ComponentRunnerState ReduceWithAction(
            ComponentRunnerState inState,
            WithAction withAction)
        {
            var inDisplayState = inState.ComponentRunnerDisplayStateBag.FirstOrDefault(
                x => x.Key == withAction.Key);

            if (inDisplayState is null)
                return inState;

            var outDisplayStateBag = inState.ComponentRunnerDisplayStateBag.Replace(
                inDisplayState,
                withAction.WithFunc.Invoke(inDisplayState));

            return new ComponentRunnerState { ComponentRunnerDisplayStateBag = outDisplayStateBag };
        }

        [ReducerMethod]
        public static ComponentRunnerState ReduceDisposeAction(
            ComponentRunnerState inState,
            DisposeAction disposeAction)
        {
            var inDisplayState = inState.ComponentRunnerDisplayStateBag.FirstOrDefault(
                x => x.Key == disposeAction.Key);

            if (inDisplayState is null)
                return inState;

            var outDisplayStateBag = inState.ComponentRunnerDisplayStateBag.Remove(inDisplayState);

            return new ComponentRunnerState
            {
                ComponentRunnerDisplayStateBag = outDisplayStateBag
            };
        }
    }
}