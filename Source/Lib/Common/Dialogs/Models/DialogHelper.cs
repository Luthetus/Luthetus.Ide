using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public static class DialogHelper
{
	public static ElementDimensions ConstructDefaultElementDimensions()
	{
		var elementDimensions = new ElementDimensions
        {
            ElementPositionKind = ElementPositionKind.Fixed
        };

        elementDimensions.WidthDimensionAttribute.DimensionUnitList.Add(
        	new DimensionUnit(60, DimensionUnitKind.ViewportWidth));

        elementDimensions.HeightDimensionAttribute.DimensionUnitList.Add(
        	new DimensionUnit(60, DimensionUnitKind.ViewportHeight));

        elementDimensions.LeftDimensionAttribute.DimensionUnitList.Add(
        	new DimensionUnit(20, DimensionUnitKind.ViewportWidth));

        elementDimensions.TopDimensionAttribute.DimensionUnitList.Add(
        	new DimensionUnit(20, DimensionUnitKind.ViewportHeight));

        return elementDimensions;
	}
}

