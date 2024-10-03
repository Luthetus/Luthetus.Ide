namespace Luthetus.CompilerServices.Python.Facts;

public partial class PythonLanguageFacts
{
    public const char STRING_LITERAL_START = '"';
    public const char STRING_LITERAL_END = '"';

    public const char COMMENT_SINGLE_LINE_STARTING_CHAR = '/';
    public const string COMMENT_SINGLE_LINE_STARTING_SUBSTRING = "//";

    public const string COMMENT_MULTI_LINE_STARTING_SUBSTRING = "/*";
    public const char COMMENT_MULTI_LINE_ENDING_IDENTIFYING_CHAR = '*';
    public const string COMMENT_MULTI_LINE_ENDING_SUBSTRING = "*/";

    public const char PREPROCESSOR_DIRECTIVE_TRANSITION_CHAR = '#';

    public const char LIBRARY_REFERENCE_ABSOLUTE_PATH_STARTING_CHAR = '<';
    public const char LIBRARY_REFERENCE_ABSOLUTE_PATH_ENDING_CHAR = '>';

    public const char LIBRARY_REFERENCE_RELATIVE_PATH_STARTING_CHAR = '"';
    public const char LIBRARY_REFERENCE_RELATIVE_PATH_ENDING_CHAR = '"';

    public const char STATEMENT_DELIMITER_CHAR = ';';
}