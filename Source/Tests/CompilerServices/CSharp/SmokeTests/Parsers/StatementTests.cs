using System.Text;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Parsers;

public class StatementTests
{
	[Fact]
    public void FunctionDefinition()
    {
    	var resourceUri = new ResourceUri("./unitTesting.txt");
        
        var sourceText =
@"
public void Aaa()
{
}

";
        
		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
		var topCodeBlock = compilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		//var functionInvocationNode = (FunctionInvocationNode)topCodeBlock.GetChildList()[0];
		// WriteChildrenIndented(functionInvocationNode, nameof(functionInvocationNode));
		
		//var identifierToken = (IdentifierToken)functionInvocationNode.GetChildList()[0];
		
		/*var functionParametersListingNode = (FunctionParametersListingNode)functionInvocationNode.GetChildList()[1];
		{
			// Assertions relating to functionParametersListingNode's properties are in this code block.
			Assert.True(functionParametersListingNode.OpenParenthesisToken.ConstructorWasInvoked);
	        Assert.Equal(1, functionParametersListingNode.FunctionParameterEntryNodeList.Count);
	        Assert.True(functionParametersListingNode.CloseParenthesisToken.ConstructorWasInvoked);
		}*/
		
		// var typeClauseNode = (TypeClauseNode)functionInvocationNode.GetChildList()[2];
		
		throw new NotImplementedException();
    }
}
