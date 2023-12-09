using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxObjects;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer.SyntaxActors;

public class GenericSyntaxWalkerTests
{
    public List<GenericStringSyntax> StringSyntaxBag { get; } = new();
    public List<GenericCommentSingleLineSyntax> CommentSingleLineSyntaxBag { get; } = new();
    public List<GenericCommentMultiLineSyntax> CommentMultiLineSyntaxBag { get; } = new();
    public List<GenericKeywordSyntax> KeywordSyntaxBag { get; } = new();
    public List<GenericFunctionSyntax> FunctionSyntaxBag { get; } = new();
    public List<GenericPreprocessorDirectiveSyntax> PreprocessorDirectiveSyntaxBag { get; } = new();
    public List<GenericDeliminationExtendedSyntax> DeliminationExtendedSyntaxBag { get; } = new();

    public void Visit(IGenericSyntax node)
    {
        foreach (var child in node.ChildBag)
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
        StringSyntaxBag.Add(node);
    }

    private void VisitCommentSingleLineSyntax(GenericCommentSingleLineSyntax node)
    {
        CommentSingleLineSyntaxBag.Add(node);
    }

    private void VisitCommentMultiLineSyntax(GenericCommentMultiLineSyntax node)
    {
        CommentMultiLineSyntaxBag.Add(node);
    }

    private void VisitKeywordSyntax(GenericKeywordSyntax node)
    {
        KeywordSyntaxBag.Add(node);
    }

    private void VisitFunctionSyntax(GenericFunctionSyntax node)
    {
        FunctionSyntaxBag.Add(node);
    }

    private void VisitPreprocessorDirectiveSyntax(GenericPreprocessorDirectiveSyntax node)
    {
        PreprocessorDirectiveSyntaxBag.Add(node);
    }

    private void VisitDeliminationExtendedSyntax(GenericDeliminationExtendedSyntax node)
    {
        DeliminationExtendedSyntaxBag.Add(node);
    }
}