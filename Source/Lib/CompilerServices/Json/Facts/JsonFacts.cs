using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.Json.Facts;

public static class JsonFacts
{
    public const string COMMENT_LINE_START = "//";
    public static readonly IReadOnlyList<char> COMMENT_LINE_ENDINGS = new List<char>
    {
        WhitespaceFacts.CARRIAGE_RETURN,
        WhitespaceFacts.LINE_FEED,
    };

    public const string COMMENT_BLOCK_START = "/*";
    public const string COMMENT_BLOCK_END = "*/";

    public const char OBJECT_START = '{';
    public const char OBJECT_END = '}';

    public const char PROPERTY_KEY_START = '"';
    public const char PROPERTY_KEY_END = '"';

    public const char PROPERTY_DELIMITER_BETWEEN_KEY_AND_VALUE = ':';

    public const char STRING_START = '"';
    public const char STRING_END = '"';

    public const char ARRAY_START = '[';
    public const char ARRAY_END = ']';

    public const char PROPERTY_ENTRY_DELIMITER = ',';
    public const char ARRAY_ENTRY_DELIMITER = ',';

    public const char NUMBER_DECIMAL_PLACE_SEPARATOR = '.';

    /// <summary>
    /// Json schema states that the booleans are to be case sensitive.
    /// <br/><br/>
    /// https://json-schema.org/understanding-json-schema/reference/boolean.html#boolean
    /// </summary>
    public const string BOOLEAN_TRUE_STRING_VALUE = "true";
    /// <summary>
    /// Json schema states that the booleans are to be case sensitive.
    /// <br/><br/>
    /// https://json-schema.org/understanding-json-schema/reference/boolean.html#boolean
    /// </summary>
    public const string BOOLEAN_FALSE_STRING_VALUE = "false";

    public static readonly IReadOnlyList<string> BOOLEAN_ALL_STRING_VALUES = new List<string>
    {
        BOOLEAN_TRUE_STRING_VALUE,
        BOOLEAN_FALSE_STRING_VALUE,
    };

    /// <summary>
    /// Json schema does not directly state that null is to be case sensitive.
    /// However, the JSON schema does not have an example of not using null in all lowercase.
    /// Therefore, the presumption that null is to be case sensitive will be made.
    /// <br/><br/>
    /// https://json-schema.org/understanding-json-schema/reference/null.html#null
    /// </summary>
    public const string NULL_STRING_VALUE = "null";
}