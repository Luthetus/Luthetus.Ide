using System.Collections.Immutable;

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
        ImmutableArray<string> commentSingleLineEndingsList,
        string commentMultiLineStart,
        string commentMultiLineEnd,
        ImmutableArray<string> keywordsList,
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
    public ImmutableArray<string> CommentSingleLineEndingsList { get; }
    public string CommentMultiLineStart { get; }
    public string CommentMultiLineEnd { get; }
    public ImmutableArray<string> KeywordsList { get; }
    public GenericPreprocessorDefinition PreprocessorDefinition { get; }
}