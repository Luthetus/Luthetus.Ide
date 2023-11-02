using Fluxor;

namespace Luthetus.Common.RazorLib.Dropdowns.States;

public partial record DropdownStateTests
{
    private class Reducer
    {
        [ReducerMethod]
        public static DropdownState ReduceAddActiveAction(
            DropdownState inState,
            AddActiveAction addActiveAction)
        {
            if (inState.ActiveKeyBag.Any(x => x == addActiveAction.Key))
                return inState;

            return inState with
            {
                ActiveKeyBag = inState.ActiveKeyBag.Add(addActiveAction.Key)
            };
        }

        [ReducerMethod]
        public static DropdownState ReduceRemoveActiveAction(
            DropdownState inState,
            RemoveActiveAction removeActiveAction)
        {
            return inState with
            {
                ActiveKeyBag = inState.ActiveKeyBag.Remove(removeActiveAction.Key)
            };
        }

        [ReducerMethod(typeof(ClearActivesAction))]
        public static DropdownState ReduceClearActivesAction(
            DropdownState inState)
        {
            return inState with
            {
                ActiveKeyBag = inState.ActiveKeyBag.Clear()
            };
        }
    }
}