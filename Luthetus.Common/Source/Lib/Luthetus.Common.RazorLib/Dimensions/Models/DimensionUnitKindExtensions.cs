namespace Luthetus.Common.RazorLib.Dimensions.Models;

public static class DimensionUnitKindExtensions
{
    public static string GetStyleString(this DimensionUnitKind dimensionUnitKind)
    {
        return dimensionUnitKind switch
        {
            DimensionUnitKind.Pixels => "px",
            DimensionUnitKind.ViewportWidth => "vw",
            DimensionUnitKind.ViewportHeight => "vh",
            DimensionUnitKind.Percentage => "%",
            DimensionUnitKind.RootCharacterWidth => "rch",
            DimensionUnitKind.RootCharacterHeight => "rem",
            DimensionUnitKind.CharacterWidth => "ch",
            DimensionUnitKind.CharacterHeight => "em",
            _ => throw new ApplicationException($"The {nameof(DimensionUnitKind)}: '{dimensionUnitKind}' was not recognized.")
        };
    }
}