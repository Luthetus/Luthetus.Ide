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
            if (inState.ReflectiveModelList.Any(x => x.Key == registerAction.Entry.Key))
                return inState;

            var insertionIndex = 0;

            if (registerAction.InsertionIndex >= 0 && registerAction.InsertionIndex < 1 + inState.ReflectiveModelList.Count)
                insertionIndex = registerAction.InsertionIndex;

            var outDisplayStateList = inState.ReflectiveModelList.Insert(
                insertionIndex,
                registerAction.Entry);

            return new ReflectiveState { ReflectiveModelList = outDisplayStateList };
        }

        [ReducerMethod]
        public static ReflectiveState ReduceWithAction(
            ReflectiveState inState,
            WithAction withAction)
        {
            var inDisplayState = inState.ReflectiveModelList.FirstOrDefault(
                x => x.Key == withAction.Key);

            if (inDisplayState is null)
                return inState;

            var outDisplayStateList = inState.ReflectiveModelList.Replace(
                inDisplayState,
                withAction.WithFunc.Invoke(inDisplayState));

            return new ReflectiveState { ReflectiveModelList = outDisplayStateList };
        }

        [ReducerMethod]
        public static ReflectiveState ReduceDisposeAction(
            ReflectiveState inState,
            DisposeAction disposeAction)
        {
            var inDisplayState = inState.ReflectiveModelList.FirstOrDefault(
                x => x.Key == disposeAction.Key);

            if (inDisplayState is null)
                return inState;

            var outDisplayStateList = inState.ReflectiveModelList.Remove(inDisplayState);

            return new ReflectiveState
            {
                ReflectiveModelList = outDisplayStateList
            };
        }
    }
}