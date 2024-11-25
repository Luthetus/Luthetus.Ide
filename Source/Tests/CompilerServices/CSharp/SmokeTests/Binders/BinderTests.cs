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

namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Binders;

public class BinderTests
{
	public class Test
	{
		public Test(string sourceText)
		{
			SourceText = sourceText;
			ResourceUri = new ResourceUri("./unitTesting.txt");
			Lexer = new CSharpLexer(ResourceUri, SourceText);
	        Lexer.Lex();
	        Parser = new CSharpParser(Lexer);
	        CompilationUnit = Parser.Parse();
		}
		
		public string SourceText { get; set; }
		public ResourceUri ResourceUri { get; set; }
		public CSharpLexer Lexer { get; set; }
		public CSharpParser Parser { get; set; }
		public CompilationUnit CompilationUnit { get; set; }
	}
	
	[Fact]
    public void TypeDefinitionNode_IdentifierToken()
    {
		var test = new Test(@"public class Aaa { }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
		var binder = test.CompilationUnit.Binder;
		var node = binder.GetSyntaxNode(13, test.ResourceUri, test.CompilationUnit);
		
		Assert.NotNull(node);
		Assert.Equal(SyntaxKind.TypeDefinitionNode, node.SyntaxKind);
    }
    
    [Fact]
    public void FunctionDefinitionNode_IdentifierToken()
    {
		var test = new Test(@"public void Aaa() { }");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
		var binder = test.CompilationUnit.Binder;
		var node = binder.GetSyntaxNode(12, test.ResourceUri, test.CompilationUnit);
		
		Assert.NotNull(node);
		Assert.Equal(SyntaxKind.FunctionDefinitionNode, node.SyntaxKind);
    }
    
    [Fact]
    public void GlobalScope_ThreeNodes_TypeDefinitionNode()
    {
		var test = new Test(
@"
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTask : IBackgroundTask
{
}
".ReplaceLineEndings("\n"));
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
		var binder = test.CompilationUnit.Binder;
		var node = binder.GetSyntaxNode(119, test.ResourceUri, test.CompilationUnit);
		
		Assert.NotNull(node);
		Assert.Equal(SyntaxKind.TypeDefinitionNode, node.SyntaxKind);
    }
    
    private void WriteChildrenIndented(ISyntaxNode node, string name = "node")
    {
    	Console.WriteLine($"foreach (var child in {name}.GetChildList())");
		foreach (var child in node.GetChildList())
		{
			Console.WriteLine("\t" + child.SyntaxKind);
		}
		Console.WriteLine();
    }
    
    private void WriteChildrenIndentedRecursive(ISyntaxNode node, string name = "node", int indentation = 0)
    {
    	var indentationStringBuilder = new StringBuilder();
    	for (int i = 0; i < indentation; i++)
    		indentationStringBuilder.Append('\t');
    	
    	Console.WriteLine($"{indentationStringBuilder.ToString()}{node.SyntaxKind}");
    	
    	// For the child tokens
    	indentationStringBuilder.Append('\t');
    	var childIndentation = indentationStringBuilder.ToString();
    	
		foreach (var child in node.GetChildList())
		{
			if (child is ISyntaxNode syntaxNode)
			{
				WriteChildrenIndentedRecursive(syntaxNode, "node", indentation + 1);
			}
			else if (child is ISyntaxToken syntaxToken)
			{
				Console.WriteLine($"{childIndentation}{child.SyntaxKind}__{syntaxToken.TextSpan.GetText()}");
			}
		}
		
		if (indentation == 0)
			Console.WriteLine();
    }
}