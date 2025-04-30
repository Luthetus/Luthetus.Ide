using System.Text;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

/// <summary>
/// This type is intended to represent a line within a flat list.
/// The 'LineIndex' is just a marker for the offset within the flat list, not actually multi-dimensional list.
/// </summary>
public record struct VirtualizationLine
{
	public VirtualizationLine(
	    int lineIndex,
	    int positionIndexInclusiveStart,
	    int positionIndexExclusiveEnd,
	    int virtualizationSpanIndexInclusiveStart,
	    int virtualizationSpanIndexExclusiveEnd,
	    double widthInPixels,
	    double heightInPixels,
	    double leftInPixels,
	    double topInPixels,
	    StringBuilder stringBuilder)
	{
		LineIndex = lineIndex;
	    PositionIndexInclusiveStart = positionIndexInclusiveStart;
	    PositionIndexExclusiveEnd = positionIndexExclusiveEnd;
	    VirtualizationSpanIndexInclusiveStart = virtualizationSpanIndexInclusiveStart;
	    VirtualizationSpanIndexExclusiveEnd = virtualizationSpanIndexExclusiveEnd;
	    WidthInPixels = widthInPixels;
	    HeightInPixels = heightInPixels;
	    LeftInPixels = leftInPixels;
	    TopInPixels = topInPixels;
	    
	    stringBuilder.Clear();
    
        var topInPixelsInvariantCulture = topInPixels.ToCssValue();
        stringBuilder.Append("top: ");
        stringBuilder.Append(topInPixelsInvariantCulture);
        stringBuilder.Append("px;");
        TopCssStyle = stringBuilder.ToString();
	}
	
	public int LineIndex { get; }
    public int PositionIndexInclusiveStart { get; }
    public int PositionIndexExclusiveEnd { get; }
    public int VirtualizationSpanIndexInclusiveStart { get; set; }
    public int VirtualizationSpanIndexExclusiveEnd { get; set; }
    public double WidthInPixels { get; }
    public double HeightInPixels { get; }
    public double LeftInPixels { get; }
    public double TopInPixels { get; }
    public string TopCssStyle { get; }
}