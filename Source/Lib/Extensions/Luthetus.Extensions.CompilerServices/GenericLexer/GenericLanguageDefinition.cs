namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;

public class GenericLanguageDefinition
{
    public GenericLanguageDefinition(
        string stringStart,
        string stringEnd,
        string functionInvocationStart,
        string functionInvocationEnd,
        string memberAccessToken,
        string commentSingleLineStart,
        List<string> commentSingleLineEndingsList,
        string commentMultiLineStart,
        string commentMultiLineEnd,
        string[] keywordsList,
        GenericPreprocessorDefinition preprocessorDefinition)
    {
        StringStart = stringStart;
        StringEnd = stringEnd;
        FunctionInvocationStart = functionInvocationStart;
        FunctionInvocationEnd = functionInvocationEnd;
        MemberAccessToken = memberAccessToken;
        CommentSingleLineStart = commentSingleLineStart;
        CommentSingleLineEndingsList = commentSingleLineEndingsList;
        CommentMultiLineStart = commentMultiLineStart;
        CommentMultiLineEnd = commentMultiLineEnd;
        KeywordsList = keywordsList;
        PreprocessorDefinition = preprocessorDefinition;
    }

    public string StringStart { get; }
    public string StringEnd { get; }
    public string FunctionInvocationStart { get; }
    public string FunctionInvocationEnd { get; }
    public string MemberAccessToken { get; }
    public string CommentSingleLineStart { get; }
    public List<string> CommentSingleLineEndingsList { get; }
    public string CommentMultiLineStart { get; }
    public string CommentMultiLineEnd { get; }
    public string[] KeywordsList { get; }
    public GenericPreprocessorDefinition PreprocessorDefinition { get; }
}