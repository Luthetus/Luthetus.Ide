namespace Luthetus.Common.RazorLib.Dimensions.Models;

public class DimensionUnit
{
    public double Value { get; set; }
    public DimensionUnitKind DimensionUnitKind { get; set; }
    public DimensionOperatorKind DimensionOperatorKind { get; set; } = DimensionOperatorKind.Add;
    public string Purpose { get; set; } = string.Empty;
}