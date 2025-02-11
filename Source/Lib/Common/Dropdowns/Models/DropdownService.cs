using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Models;

public class DropdownService : IDropdownService
{
	private DropdownState _dropdownState = new();
	
	public event Action? DropdownStateChanged;
	
	public DropdownState GetDropdownState() => _dropdownState;
	
    public void ReduceRegisterAction(DropdownRecord dropdown)
    {
    	var inState = GetDropdownState();
    
		var indexExistingDropdown = inState.DropdownList.FindIndex(
			x => x.Key == dropdown.Key);

		if (indexExistingDropdown != -1)
		{
			DropdownStateChanged?.Invoke();
			return;
		}

		var outDropdownList = new List<DropdownRecord>(inState.DropdownList);
		outDropdownList.Add(dropdown);

        _dropdownState = inState with
        {
            DropdownList = outDropdownList
        };
        
        DropdownStateChanged?.Invoke();
		return;
    }

    public void ReduceDisposeAction(Key<DropdownRecord> key)
    {
    	var inState = GetDropdownState();
    
		var indexExistingDropdown = inState.DropdownList.FindIndex(
			x => x.Key == key);

		if (indexExistingDropdown == -1)
		{
			DropdownStateChanged?.Invoke();
			return;
		}
			
		var outDropdownList = new List<DropdownRecord>(inState.DropdownList);
		outDropdownList.RemoveAt(indexExistingDropdown);

        _dropdownState = inState with
        {
            DropdownList = outDropdownList
        };
        
        DropdownStateChanged?.Invoke();
		return;
    }

    public void ReduceClearAction()
    {
    	var inState = GetDropdownState();
    
    	var outDropdownList = new List<DropdownRecord>();
    
        _dropdownState = inState with
        {
            DropdownList = outDropdownList
        };
        
        DropdownStateChanged?.Invoke();
		return;
    }

    public void ReduceFitOnScreenAction(DropdownRecord dropdown)
    {
    	var inState = GetDropdownState();
    
		var indexExistingDropdown = inState.DropdownList.FindIndex(
			x => x.Key == dropdown.Key);

		if (indexExistingDropdown == -1)
		{
			DropdownStateChanged?.Invoke();
			return;
		}
		
		var inDropdown = inState.DropdownList[indexExistingDropdown];

		var outDropdown = inDropdown with
		{
			Width = dropdown.Width,
			Height = dropdown.Height,
			Left = dropdown.Left,
			Top = dropdown.Top
		};
		
		var outDropdownList = new List<DropdownRecord>(inState.DropdownList);
		outDropdownList[indexExistingDropdown] = outDropdown;

        _dropdownState = inState with
        {
            DropdownList = outDropdownList
        };
        
        DropdownStateChanged?.Invoke();
		return;
    }
}