using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;

public class GenericLanguageDefinitionTests
{
    public GenericLanguageDefinition(
        string stringStart,
        string stringEnd,
        string functionInvocationStart,
        string functionInvocationEnd,
        string memberAccessToken,
        string commentSingleLineStart,
        ImmutableArray<string> commentSingleLineEndingsBag,
        string commentMultiLineStart,
        string commentMultiLineEnd,
        ImmutableArray<string> keywordsBag,
        GenericPreprocessorDefinition preprocessorDefinition)
    {
        StringStart = stringStart;
        StringEnd = stringEnd;
        FunctionInvocationStart = functionInvocationStart;
        FunctionInvocationEnd = functionInvocationEnd;
        MemberAccessToken = memberAccessToken;
        CommentSingleLineStart = commentSingleLineStart;
        CommentSingleLineEndingsBag = commentSingleLineEndingsBag;
        CommentMultiLineStart = commentMultiLineStart;
        CommentMultiLineEnd = commentMultiLineEnd;
        KeywordsBag = keywordsBag;
        PreprocessorDefinition = preprocessorDefinition;
    }

    public string StringStart { get; }
    public string StringEnd { get; }
    public string FunctionInvocationStart { get; }
    public string FunctionInvocationEnd { get; }
    public string MemberAccessToken { get; }
    public string CommentSingleLineStart { get; }
    public ImmutableArray<string> CommentSingleLineEndingsBag { get; }
    public string CommentMultiLineStart { get; }
    public string CommentMultiLineEnd { get; }
    public ImmutableArray<string> KeywordsBag { get; }
    public GenericPreprocessorDefinition PreprocessorDefinition { get; }
}