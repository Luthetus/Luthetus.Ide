using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Common.RazorLib.Dimensions.Models;

public class DimensionUnit
{
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
    public Func<double> ValueFunc { get; set; }
    
    public DimensionUnitKind DimensionUnitKind { get; set; }
    public DimensionOperatorKind DimensionOperatorKind { get; set; } = DimensionOperatorKind.Add;
    public string Purpose { get; set; } = string.Empty;
}