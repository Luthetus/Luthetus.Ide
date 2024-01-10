using Fluxor;

namespace Luthetus.Common.RazorLib.Dropdowns.States;

public partial record DropdownState
{
    public class Reducer
    {
        [ReducerMethod]
        public static DropdownState ReduceAddActiveAction(
            DropdownState inState,
            AddActiveAction addActiveAction)
        {
            if (inState.ActiveKeyList.Any(x => x == addActiveAction.Key))
                return inState;

            return inState with
            {
                ActiveKeyList = inState.ActiveKeyList.Add(addActiveAction.Key)
            };
        }

        [ReducerMethod]
        public static DropdownState ReduceRemoveActiveAction(
            DropdownState inState,
            RemoveActiveAction removeActiveAction)
        {
            return inState with
            {
                ActiveKeyList = inState.ActiveKeyList.Remove(removeActiveAction.Key)
            };
        }

        [ReducerMethod(typeof(ClearActivesAction))]
        public static DropdownState ReduceClearActivesAction(
            DropdownState inState)
        {
            return inState with
            {
                ActiveKeyList = inState.ActiveKeyList.Clear()
            };
        }
    }
}