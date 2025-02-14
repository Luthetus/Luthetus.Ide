namespace Luthetus.Common.RazorLib.Menus.Models;

/// <summary>
/// Once the 'MenuOptionList' is exposed publically,
/// it should NOT be modified.
/// Make a shallow copy 'new List<MenuOptionRecord>(menuRecord.MenuOptionList);'
/// and modify the shallow copy if modification of the list
/// after exposing it publically is necessary.
/// </summary>
public record MenuRecord(List<MenuOptionRecord> MenuOptionList)
{
    private static readonly MenuRecord _empty = new MenuRecord(
        new List<MenuOptionRecord>
        {
            new("No menu options exist for this item.", MenuOptionKind.Other)
        });
        
    public static MenuRecord GetEmpty()
    {
    	if (_empty.MenuOptionList.Count != 1)
    		Console.WriteLine($"{nameof(MenuRecord)} {nameof(GetEmpty)} if (Empty.Count != 1)");
    		
    	return _empty;
    }
}