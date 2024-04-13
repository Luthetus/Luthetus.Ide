using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Menus.Models;

/// <summary>
/// TODO: SphagettiCode - When running WASM, the first time one opens a context menu,
/// from a keyboard event, then focus is not properly set on the menu.
/// Any opening from a keyboard event after the first will properly set focus. (2023-09-19)
/// </summary>
public record MenuRecord(ImmutableArray<MenuOptionRecord> MenuOptionList)
{
    public static readonly MenuRecord Empty = new MenuRecord(
        new MenuOptionRecord[]
        {
            new("No menu options exist for this item.", MenuOptionKind.Other)
        }.ToImmutableArray());
}