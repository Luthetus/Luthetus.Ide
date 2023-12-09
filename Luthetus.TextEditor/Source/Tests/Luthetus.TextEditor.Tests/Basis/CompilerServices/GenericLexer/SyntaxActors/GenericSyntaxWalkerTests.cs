using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer.SyntaxActors;

public class GenericSyntaxWalkerTests
{
    [Fact]
    public void StringSyntaxBag()
    {
        //public List<GenericStringSyntax> StringSyntaxBag { get; } = new();
    }

    [Fact]
    public void CommentSingleLineSyntaxBag()
    {
        //public List<GenericCommentSingleLineSyntax> CommentSingleLineSyntaxBag { get; } = new();
    }

    [Fact]
    public void CommentMultiLineSyntaxBag()
    {
        //public List<GenericCommentMultiLineSyntax> CommentMultiLineSyntaxBag { get; } = new();
    }

    [Fact]
    public void KeywordSyntaxBag()
    {
		//public List<GenericKeywordSyntax> KeywordSyntaxBag { get; } = new();
	}

    [Fact]
    public void FunctionSyntaxBag()
    {
        //public List<GenericFunctionSyntax> FunctionSyntaxBag { get; } = new();
    }

    [Fact]
    public void PreprocessorDirectiveSyntaxBag()
    {
        // public List<GenericPreprocessorDirectiveSyntax> PreprocessorDirectiveSyntaxBag { get; } = new();
    }

	[Fact]
	public void DeliminationExtendedSyntaxBag()
	{
	    // public List<GenericDeliminationExtendedSyntax> DeliminationExtendedSyntaxBag { get; } = new();
	}

	[Fact]
	public void Visit()
	{
	    //public void Visit(IGenericSyntax node)
	}

	[Fact]
	public void VisitStringSyntax()
	{
	    //private void VisitStringSyntax(GenericStringSyntax node)
	}


	[Fact]
	public void VisitCommentSingleLineSyntax()
	{
        //private void VisitCommentSingleLineSyntax(GenericCommentSingleLineSyntax node)
	}
    
	[Fact]
	public void VisitCommentMultiLineSyntax()
	{
		// private void VisitCommentMultiLineSyntax(GenericCommentMultiLineSyntax node)
	}

	[Fact]
	public void VisitKeywordSyntax()
	{
		// private void VisitKeywordSyntax(GenericKeywordSyntax node)
	}

	[Fact]
	public void VisitFunctionSyntax()
	{
		//private void VisitFunctionSyntax(GenericFunctionSyntax node)
	}

	[Fact]
	public void VisitPreprocessorDirectiveSyntax()
	{
		//private void VisitPreprocessorDirectiveSyntax(GenericPreprocessorDirectiveSyntax node)
	}

	[Fact]
	public void VisitDeliminationExtendedSyntax()
	{
		// private void VisitDeliminationExtendedSyntax(GenericDeliminationExtendedSyntax node)
	}
}