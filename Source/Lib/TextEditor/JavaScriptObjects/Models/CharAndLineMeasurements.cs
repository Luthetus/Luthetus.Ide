namespace Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;

/// <summary>
/// The unit of measurement is Pixels (px)
/// </summary>
/// <param name="CharacterWidth">The unit of measurement is Pixels (px)</param>
/// <param name="RowHeight">The unit of measurement is Pixels (px)</param>
public record struct CharAndLineMeasurements(
    double CharacterWidth,
    double LineHeight);