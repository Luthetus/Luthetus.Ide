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
	    int position_StartInclusiveIndex,
	    int position_EndExclusiveIndex,
	    int virtualizationSpan_StartInclusiveIndex,
	    int virtualizationSpan_EndExclusiveIndex,
	    double widthInPixels,
	    double heightInPixels,
	    double leftInPixels,
	    double topInPixels)
	{
		LineIndex = lineIndex;
	    Position_StartInclusiveIndex = position_StartInclusiveIndex;
	    Position_EndExclusiveIndex = position_EndExclusiveIndex;
	    VirtualizationSpan_StartInclusiveIndex = virtualizationSpan_StartInclusiveIndex;
	    VirtualizationSpan_EndExclusiveIndex = virtualizationSpan_EndExclusiveIndex;
	    WidthInPixels = widthInPixels;
	    HeightInPixels = heightInPixels;
	    LeftInPixels = leftInPixels;
	    TopInPixels = topInPixels;
	}
	
	public int LineIndex { get; }
    public int Position_StartInclusiveIndex { get; }
    public int Position_EndExclusiveIndex { get; }
    public int VirtualizationSpan_StartInclusiveIndex { get; set; }
    public int VirtualizationSpan_EndExclusiveIndex { get; set; }
    public double WidthInPixels { get; }
    public double HeightInPixels { get; }
    public double LeftInPixels { get; }
    public double TopInPixels { get; }
}