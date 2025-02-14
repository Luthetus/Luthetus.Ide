namespace Luthetus.CompilerServices.Python.Facts;

public partial class PythonLanguageFacts
{
    public class Keywords
    {
    	public const string FALSE_KEYWORD = "False";
    	public const string NONE_KEYWORD = "None";
    	public const string TRUE_KEYWORD = "True";
    	public const string AND_KEYWORD = "and";
    	public const string AS_KEYWORD = "as";
    	public const string ASSERT_KEYWORD = "assert";
    	public const string ASYNC_KEYWORD = "async";
    	public const string AWAIT_KEYWORD = "await";
    	public const string BREAK_KEYWORD = "break";
    	public const string CLASS_KEYWORD = "class";
    	public const string CONTINUE_KEYWORD = "continue";
    	public const string DEF_KEYWORD = "def";
    	public const string DEL_KEYWORD = "del";
    	public const string ELIF_KEYWORD = "elif";
    	public const string ELSE_KEYWORD = "else";
    	public const string EXCEPT_KEYWORD = "except";
    	public const string FINALLY_KEYWORD = "finally";
    	public const string FOR_KEYWORD = "for";
    	public const string FROM_KEYWORD = "from";
    	public const string GLOBAL_KEYWORD = "global";
    	public const string IF_KEYWORD = "if";
    	public const string IMPORT_KEYWORD = "import";
    	public const string IN_KEYWORD = "in";
    	public const string IS_KEYWORD = "is";
    	public const string LAMBDA_KEYWORD = "lambda";
    	public const string NONLOCAL_KEYWORD = "nonlocal";
    	public const string NOT_KEYWORD = "not";
    	public const string OR_KEYWORD = "or";
    	public const string PASS_KEYWORD = "pass";
    	public const string RAISE_KEYWORD = "raise";
    	public const string RETURN_KEYWORD = "return";
    	public const string TRY_KEYWORD = "try";
    	public const string WHILE_KEYWORD = "while";
    	public const string WITH_KEYWORD = "with";
    	public const string YIELD_KEYWORD = "yield";

        public static readonly string[] ALL_LIST = new[]
        {
            FALSE_KEYWORD,
	    	NONE_KEYWORD,
	    	TRUE_KEYWORD,
	    	AND_KEYWORD,
	    	AS_KEYWORD,
	    	ASSERT_KEYWORD,
	    	ASYNC_KEYWORD,
	    	AWAIT_KEYWORD,
	    	BREAK_KEYWORD,
	    	CLASS_KEYWORD,
	    	CONTINUE_KEYWORD,
	    	DEF_KEYWORD,
	    	DEL_KEYWORD,
	    	ELIF_KEYWORD,
	    	ELSE_KEYWORD,
	    	EXCEPT_KEYWORD,
	    	FINALLY_KEYWORD,
	    	FOR_KEYWORD,
	    	FROM_KEYWORD,
	    	GLOBAL_KEYWORD,
	    	IF_KEYWORD,
	    	IMPORT_KEYWORD,
	    	IN_KEYWORD,
	    	IS_KEYWORD,
	    	LAMBDA_KEYWORD,
	    	NONLOCAL_KEYWORD,
	    	NOT_KEYWORD,
	    	OR_KEYWORD,
	    	PASS_KEYWORD,
	    	RAISE_KEYWORD,
	    	RETURN_KEYWORD,
	    	TRY_KEYWORD,
	    	WHILE_KEYWORD,
	    	WITH_KEYWORD,
	    	YIELD_KEYWORD,
        };

        public static readonly string[] CONTROL_KEYWORDS = new[]
        {
        	IF_KEYWORD
        };
    }
}