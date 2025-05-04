namespace Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;

/// <summary>
/// The unit of measurement is Pixels (px)
/// </summary>
/// <param name="CharacterWidth">The unit of measurement is Pixels (px)</param>
/// <param name="LineHeight">The unit of measurement is Pixels (px)</param>
public record struct CharAndLineMeasurements(
    double CharacterWidth,
    double LineHeight);