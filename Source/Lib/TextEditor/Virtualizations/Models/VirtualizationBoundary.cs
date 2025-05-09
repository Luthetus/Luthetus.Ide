namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

/// <summary>
/// Use -1 to indicate '100%' as the CSS unit of measurement.
///
/// (I'm not even sure that anyone was using the null value?
//  (back when this was nullable))
/// </summary>
public struct VirtualizationBoundary
{
	public VirtualizationBoundary(double widthInPixels, double heightInPixels)
	{
		WidthInPixels = widthInPixels;
		HeightInPixels = heightInPixels;
	}

	public double WidthInPixels;
    public double HeightInPixels;
}