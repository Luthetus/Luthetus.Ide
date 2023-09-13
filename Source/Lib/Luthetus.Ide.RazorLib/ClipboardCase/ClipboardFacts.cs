namespace Luthetus.Ide.RazorLib.ClipboardCase;

public static class ClipboardFacts
{
    /// <summary>
    /// Indicates the start of a phrase.<br/><br/>
    /// Phrase is being defined as a tag, command, datatype and value in string form.<br/><br/>
    /// </summary>
    public const string Tag = "`'\";luth_clipboard";
    /// <summary>Deliminates tag_command_datatype_value</summary>
    public const string FieldDelimiter = "_";

    // Commands
    public const string CopyCommand = "copy";
    public const string CutCommand = "cut";
    // DataTypes
    public const string AbsolutePathDataType = "absolute-file-path";

    public static string FormatPhrase(string command, string dataType, string value)
    {
        return Tag + FieldDelimiter + command + FieldDelimiter + dataType + FieldDelimiter + value;
    }

    public static bool TryParseString(string clipboardContents, out ClipboardPhrase? clipboardPhrase)
    {
        clipboardPhrase = null;

        if (clipboardContents.StartsWith(Tag))
        {
            // Skip Tag
            clipboardContents = clipboardContents.Substring(Tag.Length);
            // Skip Delimiter following the Tag
            clipboardContents = clipboardContents.Substring(FieldDelimiter.Length);

            var nextDelimiter = clipboardContents.IndexOf(FieldDelimiter, StringComparison.Ordinal);

            // Take Command
            var command = clipboardContents.Substring(0, nextDelimiter);

            clipboardContents = clipboardContents.Substring(nextDelimiter + 1);

            nextDelimiter = clipboardContents.IndexOf(FieldDelimiter, StringComparison.Ordinal);

            // Take DataType
            var dataType = clipboardContents.Substring(0, nextDelimiter);

            // Value is whatever remains in the string
            var value = clipboardContents.Substring(nextDelimiter + 1);

            clipboardPhrase = new ClipboardPhrase(command, dataType, value);

            return true;
        }

        return false;
    }
}