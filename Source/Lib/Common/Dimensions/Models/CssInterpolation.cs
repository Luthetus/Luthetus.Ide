namespace Luthetus.Common.RazorLib.Dimensions.Models;

/// <summary>
/// This file relates to HTML markup. Specifically, when one interpolates
/// a double from C# into a HTML tag's style attribute.
/// Depending on where one lives, one might see either, "5.2" or "5,2".
/// If one has the comma formatting, the styling will not parse correctly
/// when being rendered.
/// <br/><br/>
/// So, this file ensures period formatting regardless of localization.
/// </summary>
public static class CssInterpolation
{
    public static string ToCssValue(this double value) =>
        value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static string ToCssValue(this decimal value) =>
        value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static string ToCssValue(this float value) =>
        value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static string ToCssValue(this int value) =>
        value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}