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
			var lexerOutput = CSharpLexer.Lex(ResourceUri, SourceText);
			CompilationUnit.BinderSession = (CSharpBinderSession)CompilationUnit.Binder.StartBinderSession(ResourceUri);
	        CSharpParser.Parse(CompilationUnit, ref lexerOutput);
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
		
		var functionDefinitionNode = (FunctionDefinitionNode)topCodeBlock.GetChildList().Single();
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
    
    /// <summary>
    /// Implicit type inference - "simple" case.
    /// </summary>
    [Fact]
    public void VariableDeclarationNodeAndAssignment_Var_Test()
    {
    	/*
    	// Talking to myself.
    	- Color code property initialization syntax
	    	{
	    		PROPERTY = EXPRESSION,
	    		PROPERTY = EXPRESSION...
	    	}
	    	// Meaning, 'PROPERTY' should be a different color from 'EXPRESSION'
	    	// and neither should have the color of plain text.
	    	//
	    	// Just do this to show what the current code is achieving,
	    	// because there is some code for this but how well it works is unknown.
	    	//
	    	// An issue relates to the Binder. I don't know the type of everything yet,
	    	// so I can't always bind the 'PROPERTY' properly.
	    	//
	    	// But in the case that I do know the type, I should try to bind it.
	    - Color code collection initialization syntax
	    	{
	    		ITEM_1,
	    		ITEM_2...
	    	}
	    	// Consider a collection that is tuple / key value pair, I think there is a special syntax.
	    	var dictionary = new Dictionary<int, bool>()
	    	{
	    		{ 0, false },
	    		{ 1, true }...
	    	}
	    	// I'm quite certain this goes to an arbitrary amount of entries in the tuple / kvp.
	    	// This upcoming example is quite nonsensical but I think there is syntax for something like this:
	    	var dictionary = new Dictionary<int, bool, string>()
	    	{
	    		{ 0, false, "false" ... },
	    		{ 1, true, "true" ...  }...
	    	}
    	- Member access autocompletion
    		Some code exists for this, but I've never seen it work "out in the wild".
    		Not referring to member access tooltips here, specifically the autocompletion.
    	- Implicit type inference
    		- 'var aaa = 2;'
    		// The above statement should be
    		// "easy enough" to do tonight.
    		//
    		// If I hover 'aaa', I'd like to see 'int'.
    		// Currently, it would say 'var'.
    		//
    		// Extra points for today would be if hovering 'var'
    		// in that declaration showed 'int' tooltip,
    		//
    		// and that 'goto-definition' on that 'var'
    		// takes you to 'int' source code definition.
    		//
    		// (can't go to 'int' definition at the moment,
    		//  but if it works for one you have the source code
    		//  that you can 'goto-definition' on via the 'var'
    		//  that'd be nice).
    	- Parse lambda expression - statement code blocks.
    		- Use deferred parsing, as I said I think I only have to acknowledge
    		  the existence of the lambda expression while in the 'expression' loop.
    		- Then, once I finish parsing the 'expressions' I can
    		  ask the 'statement' loop to 'rewind' the TokenIndex and
    		  parse the lambda expression's statement code block.
    	- Parse the interpolated string's expressions.
    	- Comments need to be moved out of the TokenWalker?
    		- Maybe I just need to use more 'Match(...)' invocations
    		    because then internally to that method if current token is
    		    a comment, I can just skip over it.
    	- Explicit namespace qualification.
    	- Diagnostics don't render anymore, why?
    		- I think I changed the last few lines of
    		    CSharpParser's Parse(...) method and
    		    missed a part about the diangostics.
    		- When you do add them back,
    		    only use the ones that are accurate.
    		    It is very obnoxious when erroneous diagnostic
    		    errors are being shown all over the code.
    	- In this method, the type 'Test' is being used.
    	  And when member accessed, the tooltips are not working.
    	  Is this because it is a Type which is defined within another Type?
    	  This shouldn't be an issue, just use the fully
    	  qualified name as if the outer Type were a namespace.
    	- When hovering a TypeClauseNode which is a ValueTuple,
    	  each member of the ValueTuple should have its corresponding parts
    	  rendered respectively.
    	  - At the moment the entirety of '(int Aaa, bool Bbb)' is being colored
    	    'blue' which is the color given to Types.
    	    - Do this by having an 'int' index on TypeClauseNode
    	      that accesses meta-deta (or just "extra information").
    	- Keyword "functions?".
    	    - 'nameof(topCodeBlock)'
    	    - 'sizeof(Test)'
    	- Enums
    	    - I think the big reason I keep putting off doing this
    	      is that I don't know what node type to make
    	      the enum members.
    	    - They aren't a property, field, local?
    	    - I can probably make it a variable declaration node,
    	      but just add another 'VariableKind.Enum'.
    	- It has been icy / snow-y outside lately.
    	  And so, I haven't been going for a walk.
    	  I think this is having a negative effect on my mood.
    	  If I don't want to go for a walk outside because of weather,
    	  I can still do something.
    	  Even if it is "just" squats with my bodyweight.
    	- I feel anxious and like I want to procrastinate
    	  when I think about 'Implicit type inference'.
    	  - I wonder why I feel this way though.
    	  - I might worried about how a 'var aaa' variable declaration node
    	    would be "told" about its implicit type.
    	    - But, the variable declaration node already has
    	      the assigned expression as a property.
    	    - And this expression has a return type clause node.
    	    - So, if I see 'a' has a TypeClauseNode of 'var'
    	      I can check the return type clause node of the assigned expression.
    	      - I can probably get the tooltip working the same way.
    	        In the tooltip UI if I see 'var', check if its expression were assigned yet
    	        and if so, use its return type clause node instead of 'var'.
    	  - I see the issue, I don't actually have the assigned expression on the variable declaration node.
    	    - Okay, I could try to forget 'hovering var and it saying an int tooltip' for now.
    	        Because, I don't want to add another property to 'VariableDeclarationNode'
    	        which is 'ImplicitTypeClauseNode'.
    	        - Well wait I can still do the int tooltip by making the symbol text span
    	          map to the text 'var' so you hover it and see a symbol for 'int' rather than 'var'.
    	          - I'm getting a bit lost in the weeds at this point I need to just try something.
    	- Nested arbitrary code block nodes
    	- PositionIndex 0 arbitrary code block node.
    	- "bubble up" the statements if the delimiter of an outer statement is found within
    	  a code block that does not it as its own delimiter.
    	- "bubble up" implicit open code blocks that are nested when encountering a semicolon.
    	- Array indexing as an expression
    	  (for some reason the statement loop seems to bind all the variables).
    	  (the expression loop seems to return 'BadExpressionNode'? (not confirmed just is plain text syntax highlighting though))
    	  (but the final answer is probably that it would only be parsed by the expression loop.)
    	*/
    	var test = new Test(@"var aaa = 2;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
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
    public void AmbiguousParenthesizedExpressionNode_IdentifierType_Transforms_To_ExplicitCast_GenericArgs_Are_TupleTypeClauseNode()
    {
    	// var test = new Test(@"var myVariable = 2; (List<(int, bool)>)myVariable;");
    	var test = new Test(@"(List<(int, bool)>);");
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
		
		var explicitCastNode = (ExplicitCastNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.ExplicitCastNode, explicitCastNode.SyntaxKind);
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
    public void AmbiguousParenthesizedExpressionNode_KeywordTypes_Transforms_To_TypeClauseNode()
    {
    	var test = new Test(@"(int, bool)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_IdentifierTypes_Transforms_To_TypeClauseNode()
    {
    	var test = new Test(@"(Apple, Banana)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var typeClauseNode = (TypeClauseNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.TypeClauseNode, typeClauseNode.SyntaxKind);
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
    	var test = new Test(@"return (x, y) => 2;");
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
    public void AmbiguousParenthesizedExpressionNode_KeywordTypes_As_GenericArgument_NoName()
    {
    	var test = new Test(@"List<(int, bool)> myListOne;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_IdentifierTypes_As_GenericArgument_NoName()
    {
    	var test = new Test(@"List<(Apple, Banana)> myListOne;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_KeywordTypes_As_GenericArgument_WithName()
    {
    	var test = new Test(@"List<(int Aaa, bool Bbb)> myListTwo;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_IdentifierTypes_As_GenericArgument_WithName()
    {
    	var test = new Test(@"List<(Apple Aaa, Banana Bbb)> myListTwo;");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList().Single();
		Assert.Equal(SyntaxKind.VariableDeclarationNode, variableDeclarationNode.SyntaxKind);
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_ValueTuple()
    {
    	var test = new Test(@"(aaa, 2);");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		throw new NotImplementedException();
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_ValueTuple_Leading_Literal()
    {
    	var test = new Test(@"(2, aaa);");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		throw new NotImplementedException();
    }
    
    [Fact]
    public void AmbiguousParenthesizedExpressionNode_Inside_If_Does_Not_Break_Scope()
    {
    	// If statement erroneously has the semicolon as its closing scope.
    	//
    	// As well, the function has its closing scope as what
    	// should be the closing brace of the if statement's scope.
    	//
    	// What should be the opening brace of the if statement's scope
    	// is not the delimiter of any scope.
    
    	var test = new Test(@"public IExpressionNode HandleBinaryOperator()
{
	if ((aaa, 2))
	{
		;
	}
}");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
		throw new NotImplementedException();
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
    
    [Fact]
    public void DeferredParsing()
    {
    	var test = new Test(
@"
public class Person
{
	public Person(string firstName)
	{
		FirstName = firstName;
	}
	
	public string FirstName { get; set; }
}
");

		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		
		var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
		var variableAssignmentNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
		
		// Assert.Equal(SyntaxKind.WhileStatementNode, whileStatementNode.SyntaxKind);
    }
    
    /// <summary>
    /// Parse minimal combinations of the statement / codeblock delimiters.
    ///
    /// Purpose: deferred parsing (if it has a bug) can cause an infinite loop.
    ///          So this is testing the delimiters to see if they cause an infinite loop
    ///              just by typing them as a top level statement.
    ///          If the test completes then it was successful.
    ///
    /// Follow-up: This test has an infinite loop at the moment.
    ///            I'm going to break out the inputs into individual
    ///            [Fact](s) and find the ones that are causing issues.
    /// </summary>
    [Fact]
    public void TopLevelStatement_Simple_Delimiter_Typing()
    {
    	// Fixed the infinite loop,
    	// althought it is a short term "#if DEBUG" 'static' fix
    	// so now this test is still failing since they all are
    	// in the same test the static checks think its an infinite loop.
    	_ = new Test(string.Empty);
    	_ = new Test(";");
    	_ = new Test("{");
    	_ = new Test("}");
    	_ = new Test("; {");
    	_ = new Test("; }");
    	_ = new Test("{ ;");
    	_ = new Test("} ;");
    }
    
    // aaa passed
    [Fact]
    public void TopLevelStatement_Simple_Delimiter_Typing_StringEmpty()
    {
    	_ = new Test(string.Empty);
    }
    
    // aaa passed
    [Fact]
    public void TopLevelStatement_Simple_Delimiter_Typing_StatementDelimiterToken()
    {
    	_ = new Test(";");
    }

	// infinite looped
	// aaa passed (after fix)
    [Fact]
    public void TopLevelStatement_Simple_Delimiter_Typing_OpenBraceToken()
    {
    	_ = new Test("{");
    }
    
    // aaa passed
    [Fact]
    public void TopLevelStatement_Simple_Delimiter_Typing_CloseBraceToken()
    {
    	_ = new Test("}");
    }
    
    // aaa passed
    [Fact]
    public void TopLevelStatement_Simple_Delimiter_Typing_StatementDelimiterToken_OpenBraceToken()
    {
    	_ = new Test("; {");
    }
    
    // aaa passed
    [Fact]
    public void TopLevelStatement_Simple_Delimiter_Typing_StatementDelimiterToken_CloseBraceToken()
    {
    	_ = new Test("; }");
    }
    
    // aaa passed
    [Fact]
    public void TopLevelStatement_Simple_Delimiter_Typing_OpenBraceToken_StatementDelimiterToken()
    {
    	_ = new Test("{ ;");
    }
    
    // aaa passed
    [Fact]
    public void TopLevelStatement_Simple_Delimiter_Typing_CloseBraceToken_StatementDelimiterToken()
    {
    	_ = new Test("} ;");
    }
    
    [Fact]
    public void Aaa()
    {
    	_ = new Test(
@"
namespace BlazorCrudAppAaa.ServerSide.Persons;

public class Person
{
	
}
");
	}

	[Fact]
    public void Bbb()
    {
    	var test = new Test(
@"
namespace BlazorCrudAppAaa.ServerSide.Persons
{
	public class Person
	{
		
	}
}
");

		foreach (var scope in test.CompilationUnit.BinderSession.ScopeList)
		{
			Console.WriteLine($"scope.CodeBlockOwner.SyntaxKind: {scope.CodeBlockOwner.SyntaxKind}");
		}
		Console.WriteLine($"ScopeList.Count: {test.CompilationUnit.BinderSession.ScopeList.Count}");
    }
    
    [Fact]
    public void Ccc()
    {
    	var test = new Test(
@"
namespace BlazorCrudAppAaa.ServerSide.Persons
{
	public class Person
	{
		if (false)
			return;
	}
}
".ReplaceLineEndings("\n"));

		foreach (var scope in test.CompilationUnit.BinderSession.ScopeList)
		{
			Console.WriteLine($"scope.CodeBlockOwner.SyntaxKind: {scope.CodeBlockOwner.SyntaxKind}");
		}
		Console.WriteLine($"ScopeList.Count: {test.CompilationUnit.BinderSession.ScopeList.Count}");
		
		var ifStatementScope = test.CompilationUnit.BinderSession.ScopeList.Single(x =>
			x.CodeBlockOwner.SyntaxKind == SyntaxKind.IfStatementNode);
			
		Console.WriteLine($"CodeBlockOwner: {ifStatementScope.CodeBlockOwner}");
		Console.WriteLine($"IndexKey: {ifStatementScope.IndexKey}");
		Console.WriteLine($"ParentIndexKey: {ifStatementScope.ParentIndexKey}");
		
		Console.WriteLine($"(89)scope.StartingIndexInclusive: {ifStatementScope.StartingIndexInclusive}");
		Console.WriteLine($"(95)scope.EndingIndexExclusive: {ifStatementScope.EndingIndexExclusive}");
		
		if (ifStatementScope.CodeBlockOwner.OpenCodeBlockTextSpan is null)
			Console.WriteLine($"(89)OpenCodeBlockTextSpan: null");
		else
			Console.WriteLine($"(89)OpenCodeBlockTextSpan: {ifStatementScope.CodeBlockOwner.OpenCodeBlockTextSpan.Value.StartingIndexInclusive}");
			
		if (ifStatementScope.CodeBlockOwner.CloseCodeBlockTextSpan is null)
			Console.WriteLine($"(89)CloseCodeBlockTextSpan: null");
		else
			Console.WriteLine($"(89)CloseCodeBlockTextSpan: {ifStatementScope.CodeBlockOwner.CloseCodeBlockTextSpan.Value.StartingIndexInclusive}");
    }
    
    [Fact]
    public void ForLoop()
    {
    	var test = new Test(@"for (int i = 0; i < 5; i++)");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
    	throw new NotImplementedException("See ExpressionAsStatementTests");
    }
    
    [Fact]
    public void InfiniteLoopBug()
    {
    	var test = new Test(
@"
using System.Text;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Extensions.Git.States;
using Luthetus.Extensions.Git.BackgroundTasks.Models;
using Luthetus.Extensions.Git.CommandLines.Models;

namespace Luthetus.Extensions.Git.Models;

public class GitIdeApi
{
    private readonly GitBackgroundTaskApi _gitBackgroundTaskApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IState<GitState> _gitStateWrap;
	private readonly GitCliOutputParser _gitCliOutputParser;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IDispatcher _dispatcher;

    public GitIdeApi(
    	GitBackgroundTaskApi gitBackgroundTaskApi,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
        IState<TerminalState> terminalStateWrap,
        IState<GitState> gitStateWrap,
		GitCliOutputParser gitCliOutputParser,
		IEnvironmentProvider environmentProvider,
		IBackgroundTaskService backgroundTaskService,
        ICommonComponentRenderers commonComponentRenderers,
        IDispatcher dispatcher)
    {
    	_gitBackgroundTaskApi = gitBackgroundTaskApi;
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
        _terminalStateWrap = terminalStateWrap;
		_gitStateWrap = gitStateWrap;
		_gitCliOutputParser = gitCliOutputParser;
		_environmentProvider = environmentProvider;
		_backgroundTaskService = backgroundTaskService;
        _commonComponentRenderers = commonComponentRenderers;
        _dispatcher = dispatcher;
    }

    public Key<TerminalCommandRequest> GitTerminalCommandRequestKey { get; } = Key<TerminalCommandRequest>.NewKey();

    public void StatusEnqueue()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git status -u"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null)
					return ValueTask.CompletedTask;

                var gitStatusDashUCommand = $""{GitCliFacts.STATUS_COMMAND} -u"";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { gitStatusDashUCommand })
                {
                    HACK_ArgumentsString = gitStatusDashUCommand,
                    Tag = GitCliOutputParser.TagConstants.StatusEnqueue,
				};

                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		var textSpanList = _gitCliOutputParser.StatusParseEntire(
                			parsedCommand.OutputCache.ToString());
                			
                		_gitCliOutputParser.DispatchSetStatusAction();
                			
                		parsedCommand.TextSpanList = textSpanList;
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
            });
    }

    public void RefreshEnqueue(GitRepo repoAtTimeOfRequest)
    {
		StatusEnqueue();
        BranchGetAllEnqueue(repoAtTimeOfRequest);
        GetActiveBranchNameEnqueue(repoAtTimeOfRequest);
        GetOriginNameEnqueue(repoAtTimeOfRequest);
    }

    public void GetActiveBranchNameEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git get active branch name"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var terminalCommandArgs = $""branch --show-current"";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.GetActiveBranchNameEnqueue
				};

                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		_gitCliOutputParser.GetBranchParse(
                			parsedCommand.OutputCache.ToString());
                			
                		_gitCliOutputParser.DispatchSetBranchAction();
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }

    public void GetOriginNameEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git get origin name"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var terminalCommandArgs = $""config --get remote.origin.url"";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.GetOriginNameEnqueue,
				};
                    
                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		_gitCliOutputParser.GetOriginParse(
                			parsedCommand.OutputCache.ToString());
                		
                		_gitCliOutputParser.DispatchSetOriginAction();
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
		});
    }

    public void AddEnqueue(GitRepo repoAtTimeOfRequest)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
            ""git add"",
            () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var filesBuilder =  new StringBuilder();
		
		        foreach (var gitFile in localGitState.SelectedFileList)
		        {
		            var relativePathString = gitFile.RelativePathString;
		
		            if (_environmentProvider.DirectorySeparatorChar == '\\')
		            {
                        // The following fails (directory separator character):
                        //     git add "".\MyApp\""
                        //
                        // Whereas the following succeeds
                        //     git add ""./MyApp/""
                        relativePathString = relativePathString.Replace(
		                    _environmentProvider.DirectorySeparatorChar,
		                    _environmentProvider.AltDirectorySeparatorChar);
		            }
		
		            filesBuilder.Append($""\""{relativePathString}\"" "");
		        }
		
		        var terminalCommandArgs = ""add "" + filesBuilder.ToString();
		
		        var formattedCommand = new FormattedCommand(
		            GitCliFacts.TARGET_FILE_NAME,
		            new string[] { terminalCommandArgs })
		        {
		            HACK_ArgumentsString = terminalCommandArgs
		        };
					
				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
            	{
            		ContinueWithFunc = parsedCommand =>
            		{
            			StatusEnqueue();
						return Task.CompletedTask;
            		}
            	};
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
	
	public void UnstageEnqueue(GitRepo repoAtTimeOfRequest)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
            ""git unstage"",
            () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var filesBuilder =  new StringBuilder();
		
		        foreach (var gitFile in localGitState.SelectedFileList)
		        {
		            var relativePathString = gitFile.RelativePathString;
		
		            if (_environmentProvider.DirectorySeparatorChar == '\\')
		            {
                        // The following fails (directory separator character):
                        //     git restore --staged "".\MyApp\""
                        //
                        // Whereas the following succeeds
                        //     git restore --staged ""./MyApp/""
                        relativePathString = relativePathString.Replace(
		                    _environmentProvider.DirectorySeparatorChar,
		                    _environmentProvider.AltDirectorySeparatorChar);
		            }
		
		            filesBuilder.Append($""\""{relativePathString}\"" "");
		        }
		
		        var terminalCommandArgs = ""restore --staged "" + filesBuilder.ToString();
		
		        var formattedCommand = new FormattedCommand(
		            GitCliFacts.TARGET_FILE_NAME,
		            new string[] { terminalCommandArgs })
		        {
		            HACK_ArgumentsString = terminalCommandArgs
		        };
		        
				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		StatusEnqueue();
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
	
	public void CommitEnqueue(GitRepo repoAtTimeOfRequest, string commitSummary)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
            ""git commit"",
            () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var argumentsString = $""commit -m \""{commitSummary}\"""";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };

                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
	                	StatusEnqueue();
	
						NotificationHelper.DispatchInformative(
							""Git: committed"",
	                        commitSummary,
							_commonComponentRenderers,
							_dispatcher,
							TimeSpan.FromSeconds(5));
	
						return Task.CompletedTask;
					}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }

    public void BranchNewEnqueue(GitRepo repoAtTimeOfRequest, string branchName)
    {
        if (string.IsNullOrWhiteSpace(branchName))
            NotificationHelper.DispatchError(nameof(BranchNewEnqueue), ""branchName was null or whitespace"", _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(6));

        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git new branch"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var argumentsString = ""checkout -b "" + branchName;

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };
                
				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }

    public void BranchGetAllEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git branch -a"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var argumentsString = ""branch -a"";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.BranchGetAllEnqueue,
				};
                    
                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		_gitCliOutputParser.GetBranchListEntire(
                			parsedCommand.OutputCache.ToString());
                			
                		_gitCliOutputParser.DispatchSetBranchListAction();
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    public void BranchSetEnqueue(GitRepo repoAtTimeOfRequest, string branchName)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            $""git checkout {branchName}"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return ValueTask.CompletedTask;

                var argumentsString = $""checkout {branchName}"";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.BranchSetEnqueue,
				};

				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		RefreshEnqueue(repoAtTimeOfRequest);
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    public void PushToOriginWithTrackingEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git push -u origin {branchName will go here}"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				// This command will push to origin, and then set the upstream variable
				// (unsure if this is the correct description)
				var argumentsString = $""push -u origin {localGitState.Branch}"";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.PushToOriginWithTrackingEnqueue
				};

				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
	                	RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }

    public void PullEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git pull"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var argumentsString = $""pull"";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.PullEnqueue
				};

				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    public void FetchEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git fetch"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var argumentsString = $""fetch"";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.FetchEnqueue
				};

			    var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    public void LogFileEnqueue(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, Task> callback)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git log file"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var terminalCommandArgs = $""log -p {relativePathToFile}"";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.LogFileEnqueue
				};

                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		return callback.Invoke(
                			_gitCliOutputParser,
                			parsedCommand.OutputCache.ToString());
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    public void ShowFileEnqueue(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, Task> callback)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git show file"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				// Example output:
				/*
				PS C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg> git log BlazorApp4NetCoreDbg/Pages/FetchData.razor
				commit 87c2893b1006defc36770c166ab13fdbc6b7f959
				Author: Luthetus <45454132+huntercfreeman@users.noreply.github.com>
				Date:   Fri May 3 16:15:17 2024 -0400
				
				    Abc123 3
				*/
				
				var skipTextLength = ""commit "".Length;
				var takeTextLength = ""87c2893b1006defc36770c166ab13fdbc6b7f959"".Length;
				
				var logTerminalCommandArgs = $""log -p {relativePathToFile}"";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { logTerminalCommandArgs })
                {
                    HACK_ArgumentsString = logTerminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.LogFileEnqueue
				};

                var logTerminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = gitHashParsedCommand =>
                	{
                		var hash = gitHashParsedCommand.OutputCache
                			.ToString()
                			.Substring(skipTextLength, takeTextLength)
                			.Trim();
                	
                		var showTerminalCommandRequest = new TerminalCommandRequest(
                			$""git show {hash}:{relativePathToFile}"",
                			localGitState.Repo.AbsolutePath.Value)
                		{
                			ContinueWithFunc = parsedCommand =>
                			{
                				var output = parsedCommand.OutputCache.ToString();
                			
                				if (output.StartsWith(""﻿""))
                					output = output[""﻿"".Length..];
                			
                				return callback.Invoke(
		                			_gitCliOutputParser,
		                			output);
                			}
                		};
                		
                		_terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(showTerminalCommandRequest);
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(logTerminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    private List<int> GetPlusMarkedLineIndexList(string gitLogDashPOutput)
	{
		var stringWalker = new StringWalker(new ResourceUri(""/getPlusMarkedLineIndexList.txt""), gitLogDashPOutput);
		var linesReadCount = 0;
		
		// The hunk header looks like:
		// ""@@ -1,6 +1,23 @@""
		var atAtReadCount = 0;
		var oldAndNewHaveEqualFirstLine = false;
		int? sourceLineNumber = null;
		
		var isFirstCharacterOnLine = false;
		
		var plusMarkedLineIndexList = new List<int>();
		
		while (!stringWalker.IsEof)
		{
			if (WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
			{
				if (stringWalker.CurrentCharacter == '\r' && stringWalker.NextCharacter == '\n')
					_ = stringWalker.ReadCharacter();
					
				linesReadCount++;
				isFirstCharacterOnLine = true;
				
				if (sourceLineNumber is not null)
					sourceLineNumber++;
			}
			else if (linesReadCount == 4 && sourceLineNumber is null)
			{
				// Naively going to assume that the 5th line is always the start of the hunk header... for now
				if (stringWalker.CurrentCharacter == '@' && stringWalker.NextCharacter == '@')
				{
					atAtReadCount++;
					
					if (atAtReadCount == 2)
					{
						if (WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.PeekCharacter(2)))
						{
							// If immediately after the hunk header there is a newline character,
							// then the old and new text are NOT equal with respects to the first line.
						}
						else
						{
							// If after the hunk header there is NOT a newline character,
							// then the old and new text are equal with respects to the first line.
							//
							// (Does modificaton imply deletion of old, and insertion of new?)
							oldAndNewHaveEqualFirstLine = true;
						}
						
						sourceLineNumber = 0;
					}
				}
			}
			else if (sourceLineNumber is not null)
			{
				if (isFirstCharacterOnLine && stringWalker.CurrentCharacter == '+')
					plusMarkedLineIndexList.Add(sourceLineNumber.Value - 1);
			}
		
			_ = stringWalker.ReadCharacter();
		}
		
		return plusMarkedLineIndexList;
	}
    
    public void DiffFileEnqueue(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, List<int>, Task> callback)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            ""git diff file"",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var terminalCommandArgs = $""diff -p {relativePathToFile}"";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.LogFileEnqueue
				};

                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		var plusMarkedLineIndexList = GetPlusMarkedLineIndexList(parsedCommand.OutputCache.ToString());
                	
                		return callback.Invoke(
                			_gitCliOutputParser,
                			parsedCommand.OutputCache.ToString(),
                			plusMarkedLineIndexList);
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
}

");
		var topCodeBlock = test.CompilationUnit.RootCodeBlockNode;
		WriteChildrenIndentedRecursive(topCodeBlock, nameof(topCodeBlock));
    	throw new NotImplementedException("See ExpressionAsStatementTests");
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
