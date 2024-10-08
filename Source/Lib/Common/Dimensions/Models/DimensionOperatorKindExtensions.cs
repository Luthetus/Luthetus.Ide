using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Common.RazorLib.Dimensions.Models;

public static class DimensionOperatorKindExtensions
{
    public static string GetStyleString(this DimensionOperatorKind dimensionOperatorKind)
    {
        return dimensionOperatorKind switch
        {
            DimensionOperatorKind.Add => "+",
            DimensionOperatorKind.Subtract => "-",
            DimensionOperatorKind.Multiply => "*",
            DimensionOperatorKind.Divide => "/",
            _ => throw new LuthetusCommonException($"The {nameof(DimensionOperatorKind)}: '{dimensionOperatorKind}' was not recognized.")
        };
    }
}