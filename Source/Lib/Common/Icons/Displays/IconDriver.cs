namespace Luthetus.Common.RazorLib.Icons.Displays;

public struct IconDriver
{
	public IconDriver(int widthInPixels, int heightInPixels)
    {
        WidthInPixels = widthInPixels;
        HeightInPixels = heightInPixels;
    }

    public double WidthInPixels { get; }
    public double HeightInPixels { get; }

    public override string ToString() => $"({WidthInPixels}, {HeightInPixels})";
}
