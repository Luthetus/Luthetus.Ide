using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Common.RazorLib.Dimensions.Models;

/// <summary>
/// default when 'Purpose is null'.
/// </summary>
public struct DimensionUnit
{
	public DimensionUnit(
		Func<double> valueFunc,
		DimensionUnitKind dimensionUnitKind,
		DimensionOperatorKind dimensionOperatorKind,
		string purpose)
	{
		ValueFunc = valueFunc;
    
	    DimensionUnitKind = dimensionUnitKind;
	    DimensionOperatorKind = dimensionOperatorKind; // DimensionOperatorKind.Add;
	    Purpose = purpose; // string.Empty;
	}
	
	public DimensionUnit(
		double value,
		DimensionUnitKind dimensionUnitKind)
	{
		ValueFunc = null;
    
    	Value = value;
	    DimensionUnitKind = dimensionUnitKind;
	    DimensionOperatorKind = DimensionOperatorKind.Add;
	    Purpose = string.Empty;
	}
	
	public DimensionUnit(
		double value,
		DimensionUnitKind dimensionUnitKind,
		DimensionOperatorKind dimensionOperatorKind,
		string purpose)
	{
		ValueFunc = null;
    
	    Value = value;
	    DimensionUnitKind = dimensionUnitKind;
	    DimensionOperatorKind = dimensionOperatorKind;
	    Purpose = purpose;
	}
	
	public DimensionUnit(
		double value,
		DimensionUnitKind dimensionUnitKind,
		DimensionOperatorKind dimensionOperatorKind)
	{
		ValueFunc = null;
    
	    Value = value;
	    DimensionUnitKind = dimensionUnitKind;
	    DimensionOperatorKind = dimensionOperatorKind;
	    Purpose = string.Empty;
	}

	private double _value;

    public double Value
    {
    	get
    	{
    		var localValueFunc = ValueFunc;
    		
    		if (localValueFunc is null)
    			return _value;
    		else
    			return localValueFunc.Invoke();
    	}
    	set
    	{
    		var localValueFunc = ValueFunc;
    		
    		if (localValueFunc is null)
    			_value = value;
    		else
    			throw new LuthetusCommonException(
    				$"{nameof(DimensionUnit)} should use the setter for either the property '{nameof(Value)}' or '{nameof(ValueFunc)}', but not both. TODO: change this implementation as it is a bit hacky.");
    	}
    }
    
    /// <summary>
    /// <see cref="DimensionUnit"/> should use the setter for either the property
    /// <see cref="Value"/> or '{nameof(ValueFunc)}', but not both.
    ///
    /// TODO: change this implementation as it is a bit hacky...
    ///       ...The reason for this hacky addition was to support dimensions that are dependent on some other state.
    /// </summary>
    public Func<double> ValueFunc { get; }
    
    public DimensionUnitKind DimensionUnitKind { get; }
    public DimensionOperatorKind DimensionOperatorKind { get; } = DimensionOperatorKind.Add;
    public string Purpose { get; } = string.Empty;
}