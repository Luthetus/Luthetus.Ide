namespace Luthetus.Ide.RazorLib.CompilerServiceCase;

public record CompilerServiceDisplayDimensions
{
    public int ViewBoxWidth { get; init; } = 100;
    public int ViewBoxHeight { get; init; } = 100;
    public double CircleRadiusInPixels { get; init; } = 10;
    public double MinimumMarginRightBetweenSiblingsAndSelf { get; init; } = 3;
    public double SvgPadding { get; init; } = 3;
    public double SvgFontSizeInPixels { get; init; } = 5;
}
