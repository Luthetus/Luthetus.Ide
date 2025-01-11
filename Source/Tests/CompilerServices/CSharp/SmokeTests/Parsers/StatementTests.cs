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
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.Facts;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Parsers;

public class StatementTests
{
	public class Test
	{
		public Test(string sourceText)
		{
			SourceText = sourceText;
			ResourceUri = new ResourceUri("./unitTesting.txt");
			CompilationUnit = new CSharpCompilationUnit(ResourceUri, new CSharpBinder());
			CompilationUnit.LexerOutput = CSharpLexer.Lex(ResourceUri, SourceText);
			CompilationUnit.BinderSession = (CSharpBinderSession)CompilationUnit.Binder.StartBinderSession(ResourceUri);
	        CSharpParser.Parse(CompilationUnit);
		}
		
		public string SourceText { get; set; }
		public ResourceUri ResourceUri { get; set; }
		public CSharpLexerOutput LexerOutput { get; set; }
		public IBinder Binder => CompilationUnit.Binder;
		public CSharpCompilationUnit CompilationUnit { get; set; }
	}

	[Fact]
    public void NamespaceStatementNode_Test()
    {
    	var test = new Test(@"namespace Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Parsers;");
		
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.NamespaceStatementNode, namespaceStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void UsingStatementNode_Test()
    {
    	var test = new Test(@"using Luthetus.CompilerServices.CSharp.Tests.SmokeTests.Parsers;");
		
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var usingStatementNode = (UsingStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.UsingStatementNode, usingStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void TypeDefinitionNode_Class_Test()
    {
    	var test = new Test(@"public class Aaa { }");
		
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionNode.SyntaxKind);
    }
    
    [Fact]
    public void TypeDefinitionNode_Interface_Test()
    {
    	var test = new Test(@"public interface Aaa { }");
		
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
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
    public void FunctionDefinitionNode_Generic_Test()
    {
    	var test = new Test(@"public Task WriteAppDataAsync<AppData>(AppData appData)
		where AppData : IAppData
	{
		return Task.CompletedTask;
	}");
        
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
    public void ConstructorDefinitionNode_Arguments_Test()
    {
    	var test = new Test(@"public class Aaa { public Aaa(string firstName, string lastName) { } }");
        
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
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
    public void VariableDeclarationNodeAndAssignment_Var_Test_DetermineImplicitType()
    {
    	var test = new Test(@"var aaa = 2;");
		
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock);
		
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
    public void AmbiguousParenthesizedExpressionNode_Aaa_Test()
    {
    	var test = new Test(@"(");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_Bbb_Test()
    {
    	var test = new Test(@"(int");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_KeywordType_Transforms_To_ExplicitCast()
    {
    	var test = new Test(@"(int)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var explicitCastNode = (ExplicitCastNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ExplicitCastNode, explicitCastNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_IdentifierType_Transforms_To_ExplicitCast()
    {
    	var test = new Test(@"(WeatherForecastService);");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var explicitCastNode = (ExplicitCastNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ExplicitCastNode, explicitCastNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_With_GenericArguments_Transforms_To_ExplicitCast()
    {
    	var test = new Test(@"(List<bool>)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_WhatDoesThisResultIn()
    {
    	// List is less than bool and bool is greater than int?
    	var test = new Test(@"(List < bool > int)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_Transforms_To_TypeClauseNode()
    {
    	var test = new Test(@"(int, bool)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_With_Names_Transforms_To_TypeClauseNode()
    {
    	var test = new Test(@"(int Count, bool ShouldReturn)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_Transforms_To_TupleExpressionNode()
    {
    	var test = new Test(@"(7, true)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_With_VariableReference_Transforms_To_TupleExpressionNode()
    {
    	var test = new Test(@"var x = 7; var y = true; (x, y)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_With_VariableDeclaration_ImplicitType_Transforms_To_LambdaExpressionNode()
    {
    	var test = new Test(@"(x, y) => 2;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_With_VariableDeclaration_ExplicitType_Transforms_To_LambdaExpressionNode()
    {
    	var test = new Test(@"(int x, bool y) => 2;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_With_VariableDeclaration_ImplicitType_OuterScopeShadowed_Transforms_To_LambdaExpressionNode()
    {
    	var test = new Test(@"var x = 2; var y = true; (x, y) => 2;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_With_VariableDeclaration_ExplicitType_OuterScopeShadowed_Transforms_To_LambdaExpressionNode()
    {
    	var test = new Test(@"var x = 2; var y = true; (int x, bool y) => 2;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var ambiguousParenthesizedExpressionNode = (AmbiguousParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.AmbiguousParenthesizedExpressionNode, ambiguousParenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_Transforms_To_ParenthesizedExpressionNode()
    {
    	var test = new Test(@"(2");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ParenthesizedExpressionNode, parenthesizedExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_As_GenericArgument_NoName()
    {
    	var test = new Test(@"List<(int, bool)> myListOne;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_As_GenericArgument_WithName()
    {
    	var test = new Test(@"List<(int Aaa, bool Bbb)> myListTwo;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void LambdaExpressionNode_NoParenthesisForArguments()
    {
    	// Wrapping the lambda expression in a ParenthesizedExpressionNode in order
    	// to trigger the expression loop while parsing the inner expression
    	// (rather than having it parsed as a statement).
    	var test = new Test(@"(x => 2);");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var lambdaExpressionNode = (LambdaExpressionNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.LambdaExpressionNode, lambdaExpressionNode.SyntaxKind);
    }
    
    [Fact]
    public void LambdaExpressionNode_CodeBlockStatementBody()
    {
    	// Wrapping the lambda expression in a ParenthesizedExpressionNode in order
    	// to trigger the expression loop while parsing the inner expression
    	// (rather than having it parsed as a statement).
    	var test = new Test(@"(x => { return x; });");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var parenthesizedExpressionNode = (ParenthesizedExpressionNode)topCodeBlock.GetChildList().Single();
		var lambdaExpressionNode = (LambdaExpressionNode)parenthesizedExpressionNode.InnerExpression;
		Assert.Equal(SyntaxKind.LambdaExpressionNode, lambdaExpressionNode.SyntaxKind);
		Assert.False(lambdaExpressionNode.CodeBlockNodeIsExpression);
		Assert.NotNull(lambdaExpressionNode.CodeBlockNode);
		
		var returnStatementNode = (ReturnStatementNode)lambdaExpressionNode.CodeBlockNode.GetChildList().Single();
    }
    
    [Fact]
    public void ReturnStatement()
    {
    	var test = new Test(@"return 2;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var returnStatementNode = (ReturnStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ReturnStatementNode, returnStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void ReturnStatement_Tuple()
    {
    	var test = new Test(@"var aaa = 2; var bbb = ""cat"" return (aaa, bbb);");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var returnStatementNode = (ReturnStatementNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ReturnStatementNode, returnStatementNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableDeclaration_TupleNamed_Test()
    {
    	var test = new Test(@"(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode) expressionShortCircuitTuple;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
		
		Assert.Equal("expressionShortCircuitTuple", variableDeclarationNode.IdentifierToken.TextSpan.GetText());
    }
    
    [Fact]
    public void VariableDeclaration_TupleNoName_Test()
    {
    	//                    (item1,      item2)
    	var test = new Test(@"(SyntaxKind, IExpressionNode) expressionShortCircuitTuple;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void VariableDeclaration_GenericArgumentTuple_Test()
    {
    	var test = new Test(@"List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> ExpressionList;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
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
		
		WriteChildrenIndentedRecursive(topCodeBlock);
		
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
