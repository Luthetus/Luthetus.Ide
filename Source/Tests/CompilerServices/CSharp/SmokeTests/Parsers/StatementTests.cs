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
    public void TypeDefinitionNode_Test()
    {
    	var test = new Test(@"public class Aaa { }");
		
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionNode.SyntaxKind);
    }
    
    [Fact]
    public void FunctionDefinitionNode_Keyword_Test()
    {
    	var test = new Test(@"public void Aaa() { }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var publicKeywordToken = (KeywordToken)topCodeBlock.GetChildList()[0];
		Assert.Equal(SyntaxKind.PublicTokenKeyword, publicKeywordToken.SyntaxKind);
		
		var functionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.GetChildList()[1];
		Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionNode.SyntaxKind);
    }
    
    [Fact]
    public void FunctionDefinitionNode_Identifier_Test()
    {
    	var test = new Test(@"public Person Aaa() { }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var publicKeywordToken = (KeywordToken)topCodeBlock.GetChildList()[0];
		Assert.Equal(SyntaxKind.PublicTokenKeyword, publicKeywordToken.SyntaxKind);
		
		var functionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.GetChildList()[1];
		Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionNode.SyntaxKind);
    }
    
    [Fact]
    public void ConstructorDefinitionNode_Test()
    {
    	var test = new Test(@"public class Aaa { public Aaa() { } }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList().Single();
		
		//var publicKeywordToken = (KeywordToken)typeDefinitionNode.GetChildList()[0];
		//Assert.Equal(SyntaxKind.PublicTokenKeyword, publicKeywordToken.SyntaxKind);
		
		var constructorDefinitionNode = (ConstructorDefinitionNode)typeDefinitionNode.CodeBlockNode.GetChildList()[1];
		Assert.Equal(SyntaxKind.ConstructorDefinitionNode, constructorDefinitionNode.SyntaxKind);
    }
    
    [Fact]
    public void ArbitraryCodeBlockNode_Test()
    {
    	var test = new Test(@"{ }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var arbitraryCodeBlockNode = (ArbitraryCodeBlockNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryCodeBlockNode.SyntaxKind);
    }
    
    [Fact]
    public void DoWhileStatementNode_Test()
    {
    	var test = new Test(@"do { } while(false);");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var doWhileStatementNode = (DoWhileStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.DoWhileStatementNode, doWhileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void ForeachStatementNode_Test()
    {
    	var test = new Test(@"foreach (var item in list) { }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var foreachStatementNode = (ForeachStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ForeachStatementNode, foreachStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void ForStatementNode_Test()
    {
    	var test = new Test(@"for (int i = 0; i < list.Count; i++) { }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var forStatementNode = (ForStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ForStatementNode, forStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void IfStatementNode_Test()
    {
    	var test = new Test(@"if (false) { }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var ifStatementNode = (IfStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.IfStatementNode, ifStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void LockStatementNode_Test()
    {
    	var test = new Test(@"lock (objectLock) { }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var lockStatementNode = (LockStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.LockStatementNode, lockStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void NamespaceStatementNode_FileScope_Test()
    {
    	var test = new Test(@"namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Parsers;");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.NamespaceStatementNode, namespaceStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void NamespaceStatementNode_BlockScope_Test()
    {
    	var test = new Test(@"namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Parsers { }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.NamespaceStatementNode, namespaceStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void SwitchStatementNode_Test()
    {
    	var test = new Test(
@"switch (character)
{
	case 'a':
		break;
}");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var switchStatementNode = (SwitchStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.SwitchStatementNode, switchStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void TryStatementNode_Test()
    {
    	var test = new Test(
@"try
{
}
catch (Exception e)
{
}
finally
{
}");

        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var tryStatementNode = (TryStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TryStatementNode, tryStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void WhileStatementNode_Test()
    {
    	var test = new Test(@"while (false) { }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var whileStatementNode = (WhileStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableDeclaration_Keyword_Test()
    {
    	var test = new Test(@"int aaa;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableDeclaration_Identifier_Test()
    {
    	var test = new Test(@"Person aaa;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableDeclaration_Var_Test()
    {
    	var test = new Test(@"var aaa;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableDeclarationNodeAndAssignment_Var_Test()
    {
    	var test = new Test(@"var aaa = 2;");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableDeclarationNodeAndAssignment_IdentifierToken_Test()
    {
    	var test = new Test(@"Person aaa = 2;");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableDeclarationNodeAndAssignment_KeywordToken_Test()
    {
    	var test = new Test(@"int aaa = 2;");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableDeclarationNodeAndAssignment_Property_Auto_Test()
    {
    	var test = new Test(@"int Aaa { get; set; } = 2;");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableDeclarationNodeAndAssignment_Property_ExpressionBound_Test()
    {
    	var test = new Test(@"int Aaa => 2;");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void PropertyDeclaration()
    {
    	var test = new Test(@"public string FirstName { get; set; }");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		Assert.Equal(VariableKind.Property, variableDeclarationNode.VariableKind);
    }
    
    [Fact]
    public void PropertyDeclaration_Initialization()
    {
    	var test = new Test(@"public string FirstName { get; set; } = ""John"";");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void PropertyDeclaration_ExpressionBound()
    {
    	var test = new Test(@"public string FirstName => ""John"";");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void PropertyDeclaration_AccessModifiers()
    {
    	var test = new Test(@"public string FirstName { get; private set; }");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void PropertyDeclaration_GetterExpression_SetterBlock()
    {
    	var test = new Test(@"public string FirstName { get => _firstName; set { _firstName = value; } }");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void Attribute()
    {
    	var test = new Test(@"[Inject] private IDialogService DialogService { get; set; } = null!;");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
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
