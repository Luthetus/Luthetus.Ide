using Fluxor;

namespace Luthetus.Common.RazorLib.Reflectives.States;

public partial record ReflectiveState
{
    public class Reducer
    {
        [ReducerMethod]
        public static ReflectiveState ReduceRegisterAction(
            ReflectiveState inState,
            RegisterAction registerAction)
        {
            if (inState.ReflectiveModelBag.Any(x => x.Key == registerAction.Entry.Key))
                return inState;

            var insertionIndex = 0;

            if (registerAction.InsertionIndex >= 0 && registerAction.InsertionIndex < 1 + inState.ReflectiveModelBag.Count)
                insertionIndex = registerAction.InsertionIndex;

            var outDisplayStateBag = inState.ReflectiveModelBag.Insert(
                insertionIndex,
                registerAction.Entry);

            return new ReflectiveState { ReflectiveModelBag = outDisplayStateBag };
        }

        [ReducerMethod]
        public static ReflectiveState ReduceWithAction(
            ReflectiveState inState,
            WithAction withAction)
        {
            var inDisplayState = inState.ReflectiveModelBag.FirstOrDefault(
                x => x.Key == withAction.Key);

            if (inDisplayState is null)
                return inState;

            var outDisplayStateBag = inState.ReflectiveModelBag.Replace(
                inDisplayState,
                withAction.WithFunc.Invoke(inDisplayState));

            return new ReflectiveState { ReflectiveModelBag = outDisplayStateBag };
        }

        [ReducerMethod]
        public static ReflectiveState ReduceDisposeAction(
            ReflectiveState inState,
            DisposeAction disposeAction)
        {
            var inDisplayState = inState.ReflectiveModelBag.FirstOrDefault(
                x => x.Key == disposeAction.Key);

            if (inDisplayState is null)
                return inState;

            var outDisplayStateBag = inState.ReflectiveModelBag.Remove(inDisplayState);

            return new ReflectiveState
            {
                ReflectiveModelBag = outDisplayStateBag
            };
        }
    }
}