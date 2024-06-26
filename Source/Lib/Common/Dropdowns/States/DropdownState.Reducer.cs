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

        [ReducerMethod]
        public static DropdownState ReduceRegisterAction(
            DropdownState inState,
			RegisterAction registerAction)
        {
            return inState with
            {
                DropdownList = inState.DropdownList.Add(registerAction.Dropdown)
            };
        }

		[ReducerMethod]
        public static DropdownState ReduceDisposeAction(
            DropdownState inState,
			DisposeAction disposeAction)
        {
			var indexExistingDropdown = inState.DropdownList.FindIndex(x => x.Key == disposeAction.Key);

			if (indexExistingDropdown == -1)
				return inState;

            return inState with
            {
                DropdownList = inState.DropdownList.RemoveAt(indexExistingDropdown)
            };
        }
    }
}