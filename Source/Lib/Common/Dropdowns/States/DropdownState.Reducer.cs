using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.States;

public partial record DropdownState
{
    public class Reducer
    {
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
			var indexExistingDropdown = inState.DropdownList.FindIndex(
				x => x.Key == registerAction.Dropdown.Key);

			if (indexExistingDropdown != -1)
				return inState;

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
			var indexExistingDropdown = inState.DropdownList.FindIndex(
				x => x.Key == disposeAction.Key);

			if (indexExistingDropdown == -1)
				return inState;

            return inState with
            {
                DropdownList = inState.DropdownList.RemoveAt(indexExistingDropdown)
            };
        }

		[ReducerMethod]
        public static DropdownState ReduceClearAction(
            DropdownState inState,
			ClearAction clearAction)
        {
            return inState with
            {
                DropdownList = ImmutableList<DropdownRecord>.Empty
            };
        }

		[ReducerMethod]
        public static DropdownState ReduceFitOnScreenAction(
            DropdownState inState,
			FitOnScreenAction fitOnScreenAction)
        {
			var indexExistingDropdown = inState.DropdownList.FindIndex(
				x => x.Key == fitOnScreenAction.Dropdown.Key);

			if (indexExistingDropdown == -1)
				return inState;

			var inDropdown = inState.DropdownList[indexExistingDropdown];

			var outDropdown = inDropdown with
			{
				Width = fitOnScreenAction.Dropdown.Width,
				Height = fitOnScreenAction.Dropdown.Height,
				Left = fitOnScreenAction.Dropdown.Left,
				Top = fitOnScreenAction.Dropdown.Top
			};

            return inState with
            {
                DropdownList = inState.DropdownList.SetItem(indexExistingDropdown, outDropdown)
            };
        }
    }
}