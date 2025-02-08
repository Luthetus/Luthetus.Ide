using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.States;

public partial record DropdownState
{
    public class Reducer
    {
        [ReducerMethod]
        public static DropdownState ReduceRegisterAction(
            DropdownState inState,
			RegisterAction registerAction)
        {
			var indexExistingDropdown = inState.DropdownList.FindIndex(
				x => x.Key == registerAction.Dropdown.Key);

			if (indexExistingDropdown != -1)
				return inState;

			var outDropdownList = new List<DropdownRecord>(inState.DropdownList);
			outDropdownList.Add(registerAction.Dropdown);

            return inState with
            {
                DropdownList = outDropdownList
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
				
			var outDropdownList = new List<DropdownRecord>(inState.DropdownList);
			outDropdownList.RemoveAt(indexExistingDropdown);

            return inState with
            {
                DropdownList = outDropdownList
            };
        }

		[ReducerMethod(typeof(ClearAction))]
        public static DropdownState ReduceClearAction(
            DropdownState inState)
        {
        	var outDropdownList = new List<DropdownRecord>();
        
            return inState with
            {
                DropdownList = outDropdownList
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
			
			var outDropdownList = new List<DropdownRecord>(inState.DropdownList);
			outDropdownList[indexExistingDropdown] = outDropdown;

            return inState with
            {
                DropdownList = outDropdownList
            };
        }
    }
}