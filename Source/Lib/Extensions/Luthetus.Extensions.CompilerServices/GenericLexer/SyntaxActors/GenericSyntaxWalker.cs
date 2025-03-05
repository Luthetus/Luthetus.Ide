using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxObjects;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;

public class GenericSyntaxWalker
{
    public List<GenericStringSyntax> StringSyntaxList { get; } = new();
    public List<GenericCommentSingleLineSyntax> CommentSingleLineSyntaxList { get; } = new();
    public List<GenericCommentMultiLineSyntax> CommentMultiLineSyntaxList { get; } = new();
    public List<GenericKeywordSyntax> KeywordSyntaxList { get; } = new();
    public List<GenericFunctionSyntax> FunctionSyntaxList { get; } = new();
    public List<GenericPreprocessorDirectiveSyntax> PreprocessorDirectiveSyntaxList { get; } = new();
    public List<GenericDeliminationExtendedSyntax> DeliminationExtendedSyntaxList { get; } = new();

    public void Visit(IGenericSyntax node)
    {
        foreach (var child in node.ChildList)
        {
            Visit(child);
        }

        switch (node.GenericSyntaxKind)
        {
            case GenericSyntaxKind.StringLiteral:
                VisitStringSyntax((GenericStringSyntax)node);
                break;
            case GenericSyntaxKind.CommentSingleLine:
                VisitCommentSingleLineSyntax((GenericCommentSingleLineSyntax)node);
                break;
            case GenericSyntaxKind.CommentMultiLine:
                VisitCommentMultiLineSyntax((GenericCommentMultiLineSyntax)node);
                break;
            case GenericSyntaxKind.Keyword:
                VisitKeywordSyntax((GenericKeywordSyntax)node);
                break;
            case GenericSyntaxKind.Function:
                VisitFunctionSyntax((GenericFunctionSyntax)node);
                break;
            case GenericSyntaxKind.PreprocessorDirective:
                VisitPreprocessorDirectiveSyntax((GenericPreprocessorDirectiveSyntax)node);
                break;
            case GenericSyntaxKind.DeliminationExtended:
                VisitDeliminationExtendedSyntax((GenericDeliminationExtendedSyntax)node);
                break;
        }
    }

    private void VisitStringSyntax(GenericStringSyntax node)
    {
        StringSyntaxList.Add(node);
    }

    private void VisitCommentSingleLineSyntax(GenericCommentSingleLineSyntax node)
    {
        CommentSingleLineSyntaxList.Add(node);
    }

    private void VisitCommentMultiLineSyntax(GenericCommentMultiLineSyntax node)
    {
        CommentMultiLineSyntaxList.Add(node);
    }

    private void VisitKeywordSyntax(GenericKeywordSyntax node)
    {
        KeywordSyntaxList.Add(node);
    }

    private void VisitFunctionSyntax(GenericFunctionSyntax node)
    {
        FunctionSyntaxList.Add(node);
    }

    private void VisitPreprocessorDirectiveSyntax(GenericPreprocessorDirectiveSyntax node)
    {
        PreprocessorDirectiveSyntaxList.Add(node);
    }

    private void VisitDeliminationExtendedSyntax(GenericDeliminationExtendedSyntax node)
    {
        DeliminationExtendedSyntaxList.Add(node);
    }
}