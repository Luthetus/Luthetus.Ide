using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Menus.Models;

public record MenuRecord(ImmutableArray<MenuOptionRecord> MenuOptionList)
{
    public static readonly MenuRecord Empty = new MenuRecord(
        new MenuOptionRecord[]
        {
            new("No menu options exist for this item.", MenuOptionKind.Other)
        }.ToImmutableArray());
}