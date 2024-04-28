using System.Text;

namespace Luthetus.TextEditor.RazorLib.Htmls.Models;

public static class HtmlHelper
{
    private static readonly string SpaceString = "&nbsp;";
    private static readonly string TabString = "&nbsp;&nbsp;&nbsp;&nbsp;";
    private static readonly string NewLineString = "<br/>";
    private static readonly string AmpersandString = "&amp;";
    private static readonly string LeftAngleBracketString = "&lt;";
    private static readonly string RightAngleBracketString = "&gt;";
    private static readonly string DoubleQuoteString = "&quot;";
    private static readonly string SingleQuoteString = "&#39;";

    public static string EscapeHtml(this char input)
    {
        return input.ToString().EscapeHtml();
    }

    public static string EscapeHtml(this StringBuilder input)
    {
        return input.ToString().EscapeHtml();
    }

    /// <summary>
    /// Be careful if one alters the order of the replacements.
    /// Some replacements must be done in a particular order.
    /// </summary>
    public static string EscapeHtml(this string input)
    {
        return input
            .Replace("&", AmpersandString)
            .Replace("<", LeftAngleBracketString)
            .Replace(">", RightAngleBracketString)
            .Replace("\t", TabString)
            .Replace(" ", SpaceString)
            .Replace("\r\n", NewLineString)
            .Replace("\n", NewLineString)
            .Replace("\r", NewLineString)
            .Replace("\"", DoubleQuoteString)
            .Replace("'", SingleQuoteString);
    }
}