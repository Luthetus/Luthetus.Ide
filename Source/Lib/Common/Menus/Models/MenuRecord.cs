namespace Luthetus.Common.RazorLib.Menus.Models;

/// <summary>
/// Once the 'MenuOptionList' is exposed publically,
/// it should NOT be modified.
/// Make a shallow copy 'new List<MenuOptionRecord>(menuRecord.MenuOptionList);'
/// and modify the shallow copy if modification of the list
/// after exposing it publically is necessary.
/// </summary>
public record MenuRecord(IReadOnlyList<MenuOptionRecord> MenuOptionList)
{
	public static readonly IReadOnlyList<MenuOptionRecord> NoMenuOptionsExistList = new List<MenuOptionRecord>
    {
        new("No menu options exist for this item.", MenuOptionKind.Other)
    };
}
