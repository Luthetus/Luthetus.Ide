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

/// <summary>
/// Scope being incorrectly parsed is an error that cascades further errors in large throughout the rest of the file.
///
/// If a singular statement is incorrectly parsed, it is intended to have no impact on the rest of the file,
/// because once the 'StatementDelimiterToken' or etc... is found to mark the end of the statement.
/// Then the state related to statements should reset.
///
/// It is believed that the number 1 most important detail when writing a programming language parser
/// is parsing the scope properly.
///
/// Following that, every statement, or expression, can be treated as their own "black box".
/// If you cannot parse it, just continue on to the next statement or expression.
///
/// (not that this is ideal, but in terms of programming,
///  having these well defined start and end points to every issue you encounter is vital).
/// 
/// You then (likely) won't have to worry about the entirety of a file when a statement fails to parse.
/// Instead, you can look at that (relatively) tiny snippet of code by itself.
/// <summary>
public class ScopeTests
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
		public IBinder Binder => CompilationUnit.Binder;
		public CompilationUnit CompilationUnit { get; set; }
	}
	
	[Fact]
    public void GlobalScope()
    {
    	var test = new Test(@"");
    	
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(1, binderSession.ScopeList.Count);
		
		var scope = test.Binder.GetScopeByPositionIndex(test.ResourceUri, 0);
		Assert.NotNull(scope);
		
		{ // Global
			var globalScope = binderSession.ScopeList.Single();
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
		    Assert.Null(globalScope.CodeBlockOwner);
		}
    }
    
    [Fact]
    public void GlobalScope_ArbitraryScope()
    {
    	var test = new Test(@"{}");
    	
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
		    Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Arbitrary scope
			    var arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(0, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(2, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_ArbitraryScope_ArbitraryScope()
    {
    	var test = new Test(@"{} {}");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(3, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Arbitrary scope 1
			    var arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(0, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(2, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
		    }
		    
		    { // Arbitrary scope 2
			    var arbitraryScope = binderSession.ScopeList[2];
				Assert.Equal(2, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(3, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(5, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_ArbitraryScope_Depth_ArbitraryScope_ArbitraryScope()
    {
    	var test = new Test(@"{ {} {} }");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(4, binderSession.ScopeList.Count);
		
		IScope arbitraryScope;
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Arbitrary scope
			    arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(0, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(9, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			    
			    { // Arbitrary scope 1
				    arbitraryScope = binderSession.ScopeList[2];
					Assert.Equal(2, arbitraryScope.IndexKey);
				    Assert.Equal(1, arbitraryScope.ParentIndexKey);
				    Assert.Equal(2, arbitraryScope.StartingIndexInclusive);
				    Assert.Equal(4, arbitraryScope.EndingIndexExclusive);
			    	Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
				}
				
				{ // Arbitrary scope 2
				    arbitraryScope = binderSession.ScopeList[3];
					Assert.Equal(3, arbitraryScope.IndexKey);
				    Assert.Equal(1, arbitraryScope.ParentIndexKey);
				    Assert.Equal(5, arbitraryScope.StartingIndexInclusive);
				    Assert.Equal(7, arbitraryScope.EndingIndexExclusive);
			    	Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
				}
		    }
	    }
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode()
    {
    	var test = new Test(@"public class Person { }");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(23, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_ConstructorDefinitionNode()
    {
    	var test = new Test(@"public class Person { public Person() { } }");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(3, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(43, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
				
				{ // Constructor definition
					var constructorDefinitionScope = binderSession.ScopeList[2];
					Assert.Equal(2, typeDefinitionScope.IndexKey);
				    Assert.Equal(1, typeDefinitionScope.ParentIndexKey);
				    Assert.Equal(38, typeDefinitionScope.StartingIndexInclusive);
				    Assert.Equal(41, typeDefinitionScope.EndingIndexExclusive);
					Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
				}
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_PropertyDefinitionNode()
    {
    	var test = new Test(@"public class Person { public string FirstName { get; set; } }");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(61, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_PropertyDefinitionNodeWithAttribute()
    {
	    // line_1: position_0
	    // line_2: position_20
	    // line_3: position_22
	    // line_4: position_51
	    // line_5: position_90
    	var test = new Test(
@"public class Person
{
	[Parameter, EditorRequired]
	public string FirstName { get; set; }
}".ReplaceLineEndings("\n"));
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(91, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_PropertyDefinitionNodeWithAttributeThatInvokesConstructor()
    {
    	// line_1: position_0
	    // line_2: position_20
	    // line_3: position_22
	    // line_4: position_68
	    // line_5: position_107
    	var test = new Test(
@"public class Person
{
	[Parameter(Name=""Aaa""), EditorRequired(3)]
	public string FirstName { get; set; }
}".ReplaceLineEndings("\n"));
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(108, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_PropertyDefinitionNodeGetterAndSetterCodeBlock()
    {
    	// line_~1: position_0
	    // line_~2: position_20
	    // line_~3: position_22
	    // line_~4: position_47
	    // line_~5: position_50
	    // line_~6: position_56
	    // line_~7: position_60
	    // line_~8: position_82
	    // line_~9: position_86
	    // line_10: position_92
	    // line_11: position_96
	    // line_12: position_119
	    // line_13: position_123
	    // line_14: position_126
    	var test = new Test(
@"public class Person
{
	public string FirstName
	{
		get
		{
			return _firstName;
		}
		set
		{
			_firstName = value;
		}
	}
}".ReplaceLineEndings("\n"));
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(127, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_FunctionDefinitionNode()
    {
    	var test = new Test(@"public class Person { public void MyMethod() { } }");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(3, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(50, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
				
				{ // Function definition
				    var functionDefinitionScope = binderSession.ScopeList[2];
					Assert.Equal(2, functionDefinitionScope.IndexKey);
				    Assert.Equal(1, functionDefinitionScope.ParentIndexKey);
				    Assert.Equal(45, functionDefinitionScope.StartingIndexInclusive);
				    Assert.Equal(48, functionDefinitionScope.EndingIndexExclusive);
					Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionScope.CodeBlockOwner.SyntaxKind);
				}
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_ArbitraryCodeBlock()
    {
    	var test = new Test(@"public class Person { { } }");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(3, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(27, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
				
				{ // Arbitrary scope
				    var arbitraryScope = binderSession.ScopeList[2];
					Assert.Equal(2, arbitraryScope.IndexKey);
				    Assert.Equal(1, arbitraryScope.ParentIndexKey);
				    Assert.Equal(22, arbitraryScope.StartingIndexInclusive);
				    Assert.Equal(25, arbitraryScope.EndingIndexExclusive);
					Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
				}
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_FunctionDefinitionNode_Depth_ArbitraryCodeBlock()
    {
    	var test = new Test(@"public void MyMethod() { { } }");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(3, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Function definition
			    var functionDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, functionDefinitionScope.IndexKey);
			    Assert.Equal(0, functionDefinitionScope.ParentIndexKey);
			    Assert.Equal(23, functionDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(30, functionDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionScope.CodeBlockOwner.SyntaxKind);
				
				{ // Arbitrary scope
				    var arbitraryScope = binderSession.ScopeList[2];
					Assert.Equal(2, arbitraryScope.IndexKey);
				    Assert.Equal(1, arbitraryScope.ParentIndexKey);
				    Assert.Equal(25, arbitraryScope.StartingIndexInclusive);
				    Assert.Equal(28, arbitraryScope.EndingIndexExclusive);
					Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
				}
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_PropertyDefinitionNode()
    {
    	// The property definition node isn't creating scope.
    	// So, the final 'ArbitraryCodeBlockNode'
    	// is there to check that the scope logic did not break due to the
    	// OpenBraceToken and CloseBraceToken being used as part of the property definition node syntax.
    
    	var test = new Test(@"public string FirstName { get; set; } {}");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		var scope = test.Binder.GetScopeByPositionIndex(test.ResourceUri, 0);
		Assert.NotNull(scope);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
		    Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Arbitrary scope
				var arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(38, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(40, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			}
		}
    }
    
    [Fact]
    public void GlobalScope_PropertyDefinitionNodeGetterAndSetterCodeBlock()
    {
    	// The property definition node isn't creating scope.
    	// So, the final 'ArbitraryCodeBlockNode'
    	// is there to check that the scope logic did not break due to the
    	// OpenBraceToken and CloseBraceToken being used as part of the property definition node syntax.
    
    	var test = new Test(
@"public string FirstName
{
	get
	{
		return _firstName;
	}
	set
	{
		_firstName = value;
	}
}

{}".ReplaceLineEndings("\n"));
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		var scope = test.Binder.GetScopeByPositionIndex(test.ResourceUri, 0);
		Assert.NotNull(scope);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
		    Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Arbitrary scope
				var arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(94, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(96, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			}
		}
    }
    
    [Fact]
    public void GlobalScope_RecordWith_ArbitraryScope()
    {
    	// The record 'with' keyword isn't creating scope.
    	// So, the final 'ArbitraryCodeBlockNode'
    	// is there to check that the scope logic did not break due to the
    	// OpenBraceToken and CloseBraceToken being used as part of the record 'with' keyword syntax.
    
    	var test = new Test(
@"var x = new PersonRecord();

x = x with
{
	FirstName = ""John"",
	LastName = ""Doe"",
}

{}".ReplaceLineEndings("\n"));

		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		var scope = test.Binder.GetScopeByPositionIndex(test.ResourceUri, 0);
		Assert.NotNull(scope);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
		    Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Arbitrary scope
				var arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(89, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(91, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			}
		}
    }
    
    [Fact]
    public void GlobalScope_ObjectInitialization_ArbitraryScope()
    {
    	// The object initialization syntax isn't creating scope.
    	// So, the final 'ArbitraryCodeBlockNode'
    	// is there to check that the scope logic did not break due to the
    	// OpenBraceToken and CloseBraceToken being used as part of the object initialization syntax.
    
    	var test = new Test(
@"var x = new PersonRecord
{
	FirstName = ""John"",
	LastName = ""Doe"",
};

{}".ReplaceLineEndings("\n"));

		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		var scope = test.Binder.GetScopeByPositionIndex(test.ResourceUri, 0);
		Assert.NotNull(scope);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
		    Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Arbitrary scope
				var arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(75, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(77, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			}
		}
    }
    
    [Fact]
    public void GlobalScope_CollectionInitialization_ArbitraryScope()
    {
    	// The collection initialization syntax isn't creating scope.
    	// So, the final 'ArbitraryCodeBlockNode'
    	// is there to check that the scope logic did not break due to the
    	// OpenBraceToken and CloseBraceToken being used as part of the collection initialization syntax.
    
    	var test = new Test(
@"var x = new List<int>
{
	1,
	2,
};

{}".ReplaceLineEndings("\n"));

		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		var scope = test.Binder.GetScopeByPositionIndex(test.ResourceUri, 0);
		Assert.NotNull(scope);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
		    Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Arbitrary scope
				var arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(36, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(38, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			}
		}
    }
    
    [Fact]
    public void GlobalScope_CollectionInitializationFromArray_ArbitraryScope()
    {
    	// The collection initialization syntax isn't creating scope.
    	// So, the final 'ArbitraryCodeBlockNode'
    	// is there to check that the scope logic did not break due to the
    	// OpenBraceToken and CloseBraceToken being used as part of the collection initialization syntax.
    
    	var test = new Test(
@"var x = new int[]
{
	1,
	2,
};

{}".ReplaceLineEndings("\n"));

		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		var scope = test.Binder.GetScopeByPositionIndex(test.ResourceUri, 0);
		Assert.NotNull(scope);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
		    Assert.Null(globalScope.CodeBlockOwner);
		    
		    { // Arbitrary scope
				var arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(32, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(34, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			}
		}
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
