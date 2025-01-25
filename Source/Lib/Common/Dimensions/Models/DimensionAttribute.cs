using System.Collections.Immutable;
using System.Text;

namespace Luthetus.Common.RazorLib.Dimensions.Models;

/// <summary>
/// default: 'DimensionUnitList is null'
/// </summary>
public struct DimensionAttribute
{
	public DimensionAttribute(DimensionAttributeKind dimensionAttributeKind)
	{
        DimensionAttributeKind = dimensionAttributeKind;
	    DimensionUnitList = new();
	}

    public List<DimensionUnit> DimensionUnitList { get; }
    public DimensionAttributeKind DimensionAttributeKind { get; }
    public string StyleString => GetStyleString();
    
    public void Increment(double amount, DimensionUnitKind dimensionUnitKind, DimensionUnit defaultIfNotExists)
    {
    	var index = DimensionUnitList.FindIndex(
            du => du.DimensionUnitKind == dimensionUnitKind);

        if (index == -1)
        {
        	// TODO: This is extremely thread unsafe.
        	index = DimensionUnitList.Count;
        	DimensionUnitList.Add(defaultIfNotExists);
        }
        
        var dimensionUnit = DimensionUnitList[index];
        DimensionUnitList[index] = dimensionUnit with
        {
        	Value = dimensionUnit.Value + amount
        };
    }
    
    public void Decrement(double amount, DimensionUnitKind dimensionUnitKind, DimensionUnit defaultIfNotExists)
    {
        var index = DimensionUnitList.FindIndex(
            du => du.DimensionUnitKind == dimensionUnitKind);

        if (index == -1)
        {
        	// TODO: This is extremely thread unsafe.
        	index = DimensionUnitList.Count;
        	DimensionUnitList.Add(defaultIfNotExists);
        }
        
        var dimensionUnit = DimensionUnitList[index];
        DimensionUnitList[index] = dimensionUnit with
        {
        	Value = dimensionUnit.Value - amount
        };
    }
    
    public void Set(double amount, DimensionUnitKind dimensionUnitKind)
    {
        var index = DimensionUnitList.FindIndex(
            du => du.DimensionUnitKind == dimensionUnitKind);

        if (index == -1)
        	return;
        
        var dimensionUnit = DimensionUnitList[index];
        DimensionUnitList[index] = dimensionUnit with
        {
        	Value = amount
        };
    }

    private string GetStyleString()
    {
        var immutableDimensionUnits = DimensionUnitList.ToImmutableArray();

        if (!immutableDimensionUnits.Any())
            return string.Empty;

        var styleBuilder = new StringBuilder($"{DimensionAttributeKind.ToString().ToLower()}: calc(");

        for (var index = 0; index < DimensionUnitList.Count; index++)
        {
            var dimensionUnit = DimensionUnitList[index];

            if (index != 0)
            {
                styleBuilder
                    .Append(' ')
                    .Append(dimensionUnit.DimensionOperatorKind.GetStyleString())
                    .Append(' ');
            }

            var dimensionUnitInvariantCulture = dimensionUnit.Value
                .ToCssValue();

            styleBuilder.Append(
                $"{dimensionUnitInvariantCulture}" +
                $"{dimensionUnit.DimensionUnitKind.GetStyleString()}");
        }

        styleBuilder.Append(");");

        return styleBuilder.ToString();
    }
}