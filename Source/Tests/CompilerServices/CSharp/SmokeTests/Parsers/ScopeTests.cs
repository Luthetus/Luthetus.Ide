using System.Text;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

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
		    Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
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
		    Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
		    Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
		    Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
    public void GlobalScope_ArbitraryScope_Depth_ArbitraryScope()
    {
    	// Position index 0 arbitrary code block nodes do not parse properly at the time of this test being written.
    	// That being said this test is for something different, "nested arbitrary code block nodes".
    	var test = new Test(
@" {
	{
	}
}
".ReplaceLineEndings("\n"));
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(3, binderSession.ScopeList.Count);
		
		IScope arbitraryScope;
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
		    Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // Arbitrary scope
			    arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(1, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(10, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			    
			    { // Arbitrary scope
				    arbitraryScope = binderSession.ScopeList[2];
					Assert.Equal(2, arbitraryScope.IndexKey);
				    Assert.Equal(1, arbitraryScope.ParentIndexKey);
				    Assert.Equal(4, arbitraryScope.StartingIndexInclusive);
				    Assert.Equal(8, arbitraryScope.EndingIndexExclusive);
			    	Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
				}
		    }
	    }
    }
    
    [Fact]
    public void GlobalScope_FunctionDefinitionNode_Depth_ArbitraryScope_Depth_ArbitraryScope()
    {
    	// Position index 0 arbitrary code block nodes do not parse properly at the time of this test being written.
    	// That being said this test is for something different, "nested arbitrary code block nodes".
    	var test = new Test(
@"void Aaa()
{
	 {
		{
		}
	}

}".ReplaceLineEndings("\n"));
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(4, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    IScope arbitraryScope;
		    
		    { // Function definition node
			    var functionDefinitionNodeScope = binderSession.ScopeList[1];
				Assert.Equal(1, functionDefinitionNodeScope.IndexKey);
			    Assert.Equal(0, functionDefinitionNodeScope.ParentIndexKey);
			    Assert.Equal(11, functionDefinitionNodeScope.StartingIndexInclusive);
			    Assert.Equal(30, functionDefinitionNodeScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionNodeScope.CodeBlockOwner.SyntaxKind);
			    
			    { // Arbitrary scope
				    arbitraryScope = binderSession.ScopeList[2];
					Assert.Equal(2, arbitraryScope.IndexKey);
				    Assert.Equal(1, arbitraryScope.ParentIndexKey);
				    Assert.Equal(15, arbitraryScope.StartingIndexInclusive);
				    Assert.Equal(27, arbitraryScope.EndingIndexExclusive);
			    	Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			    	
			    	{ // Arbitrary scope
					    arbitraryScope = binderSession.ScopeList[3];
						Assert.Equal(3, arbitraryScope.IndexKey);
					    Assert.Equal(2, arbitraryScope.ParentIndexKey);
					    Assert.Equal(19, arbitraryScope.StartingIndexInclusive);
					    Assert.Equal(24, arbitraryScope.EndingIndexExclusive);
				    	Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
					}
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
		    Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
    public void GlobalScope_TypeDefinitionNode_WithPrimaryConstructorSyntax_ImplicitStartCodeBlock()
    {
    	var test = new Test(@"public class Person(string FirstName, string LastName);");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(19, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(55, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
				
				var typeDefinitionNode = typeDefinitionScope.CodeBlockOwner;
				
				Assert.Equal(1, typeDefinitionNode.ScopeIndexKey);
				
				Assert.Null(typeDefinitionNode.OpenCodeBlockTextSpan);
				
				Assert.Equal(54, typeDefinitionNode.CloseCodeBlockTextSpan.Value.StartingIndexInclusive);
				Assert.Equal(55, typeDefinitionNode.CloseCodeBlockTextSpan.Value.EndingIndexExclusive);
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_WithPrimaryConstructorSyntax_ExplicitStartCodeBlock()
    {
    	var test = new Test(@"public class Person(string FirstName, string LastName) { }");
    	
    	var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(19, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(58, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
				
				var typeDefinitionNode = typeDefinitionScope.CodeBlockOwner;
				
				Assert.Equal(1, typeDefinitionNode.ScopeIndexKey);
				
				Assert.Equal(55, typeDefinitionNode.OpenCodeBlockTextSpan.Value.StartingIndexInclusive);
				Assert.Equal(56, typeDefinitionNode.OpenCodeBlockTextSpan.Value.EndingIndexExclusive);
				
				Assert.Equal(57, typeDefinitionNode.CloseCodeBlockTextSpan.Value.StartingIndexInclusive);
				Assert.Equal(58, typeDefinitionNode.CloseCodeBlockTextSpan.Value.EndingIndexExclusive);
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(43, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
				
				{ // Constructor definition
					var constructorDefinitionScope = binderSession.ScopeList[2];
					Assert.Equal(2, constructorDefinitionScope.IndexKey);
				    Assert.Equal(1, constructorDefinitionScope.ParentIndexKey);
				    Assert.Equal(38, constructorDefinitionScope.StartingIndexInclusive);
				    Assert.Equal(41, constructorDefinitionScope.EndingIndexExclusive);
					Assert.Equal(SyntaxKind.ConstructorDefinitionNode, constructorDefinitionScope.CodeBlockOwner.SyntaxKind);
				}
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_TypeDefinitionNode_Depth_ConstructorDefinitionNode_PropertyInitialized()
    {
    	// Erroneous behavior:
    	// ===================
    	// Property definitions being initialized can break the scope of constructors that appear in source code text-wise above them.
    	//
    	// This is not happening if the property definition with initialization is moved above the constructor, only if it is below.
    	//
    	// When this behavior occurs, instead of a scope being defined at the constructor's OpenBraceToken
    	// to its CloseBraceToken.
    	// |
    	// It will instead create a scope at the semicolon of the property definition with initialization.
    	//
    	// The constructor having or not having arguments has no impact on the behavior.
    
    	var test = new Test(
@"public class Person
{
	public Person(string firstName)
	{
		FirstName = firstName;
	}
	
	public string FirstName { get; set; } = ""abc"";
}".ReplaceLineEndings("\n"));

		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(3, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(137, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
				
				{ // Constructor definition
					var constructorDefinitionScope = binderSession.ScopeList[2];
					Assert.Equal(2, constructorDefinitionScope.IndexKey);
				    Assert.Equal(1, constructorDefinitionScope.ParentIndexKey);
				    Assert.Equal(56, constructorDefinitionScope.StartingIndexInclusive);
				    Assert.Equal(85, constructorDefinitionScope.EndingIndexExclusive);
					Assert.Equal(SyntaxKind.ConstructorDefinitionNode, constructorDefinitionScope.CodeBlockOwner.SyntaxKind);
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
    
    /// <summary>
    /// When I copy and paste this tests input string to a locally running WASM website of the IDE.
    /// I can paste in the code and it works as it is currently being asserted.
    /// But it keeps saying
    ///
    /// 	Expected: 108
    /// 	Actual:   106
    ///
    /// When I run this test???
    /// I checked the line ending kinds, that they were all line feed '\n'.
    /// </summary>
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
    public void GlobalScope_TypeDefinitionNode_Inheritance()
    {
    	var test = new Test(@"public class Person : OtherType { }");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // Type definition
			    var typeDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, typeDefinitionScope.IndexKey);
			    Assert.Equal(0, typeDefinitionScope.ParentIndexKey);
			    Assert.Equal(20, typeDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(35, typeDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.TypeDefinitionNode, typeDefinitionScope.CodeBlockOwner.SyntaxKind);
				
				var typeDefinitionNode = typeDefinitionScope.CodeBlockOwner;
				
				Assert.Equal(1, typeDefinitionNode.ScopeIndexKey);
				
				Assert.Equal(32, typeDefinitionNode.OpenCodeBlockTextSpan.Value.StartingIndexInclusive);
				Assert.Equal(33, typeDefinitionNode.OpenCodeBlockTextSpan.Value.EndingIndexExclusive);
				
				Assert.Equal(34, typeDefinitionNode.CloseCodeBlockTextSpan.Value.StartingIndexInclusive);
				Assert.Equal(35, typeDefinitionNode.CloseCodeBlockTextSpan.Value.EndingIndexExclusive);
			}
	    }
    }
    
    [Fact]
    public void GlobalScope_FunctionDefinitionNode()
    {
    	var test = new Test(@"public void MyMethod() { }");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // Function definition
			    var functionDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, functionDefinitionScope.IndexKey);
			    Assert.Equal(0, functionDefinitionScope.ParentIndexKey);
			    Assert.Equal(23, functionDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(26, functionDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionScope.CodeBlockOwner.SyntaxKind);
			}
	    }
    }
    
    [Fact]
    public void FunctionDefinitionNode_ExpressionBound()
    {
    	var test = new Test(@"public int Cba(int aaa) => aaa; ");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // Function definition
			    var functionDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, functionDefinitionScope.IndexKey);
			    Assert.Equal(0, functionDefinitionScope.ParentIndexKey);
			    Assert.Equal(24, functionDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(31, functionDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionScope.CodeBlockOwner.SyntaxKind);
			}
	    }
	}
    
    /// <summary>
    /// A constructor is only sensible when defined within a type.
    /// 
    /// But the parser still needs to not crash if someone were ever to define a function without
    /// a return type that exists outside a type definition.
    ///
    /// If the function's name is equal to that of the encompasing code block owner,
    /// and that code block owner is a TypeDefinitionNode,
    /// then create a constructor symbol instead of a function symbol.
    /// </summary>
    [Fact]
    public void GlobalScope_ConstructorDefinitionNode()
    {
    	var test = new Test(@"public Person() { }");
		
		var success = test.Binder.TryGetBinderSession(test.ResourceUri, out var binderSession);
		Assert.True(success);
		Assert.Equal(2, binderSession.ScopeList.Count);
		
		{ // Global
			var globalScope = binderSession.ScopeList[0];
			Assert.Equal(0, globalScope.IndexKey);
		    Assert.Null(globalScope.ParentIndexKey);
		    Assert.Equal(0, globalScope.StartingIndexInclusive);
		    Assert.Null(globalScope.EndingIndexExclusive);
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // Function definition
			    var functionDefinitionScope = binderSession.ScopeList[1];
				Assert.Equal(1, functionDefinitionScope.IndexKey);
			    Assert.Equal(0, functionDefinitionScope.ParentIndexKey);
			    Assert.Equal(16, functionDefinitionScope.StartingIndexInclusive);
			    Assert.Equal(19, functionDefinitionScope.EndingIndexExclusive);
				Assert.Equal(SyntaxKind.FunctionDefinitionNode, functionDefinitionScope.CodeBlockOwner.SyntaxKind);
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
    public void GlobalScope_PropertyDefinitionNode_ArbitraryScope()
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
    public void GlobalScope_PropertyDefinitionNodeGetterAndSetterCodeBlock_ArbitraryScope()
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
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
    
    [Fact]
    public void Find_VariableDeclaration_ThatWasDeclaredInParentScope()
    {
    	// Erroneous behavior:
    	// ===================
    	//
    	// A variable declared in a parent scope,
    	// will not be found as a variable reference if it is referenced from a child scope.
    
    	var test = new Test(
@"var ccc = 2;
ccc;
{
	ccc;
}".ReplaceLineEndings("\n"));

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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // Arbitrary scope
				var arbitraryScope = binderSession.ScopeList[1];
				Assert.Equal(1, arbitraryScope.IndexKey);
			    Assert.Equal(0, arbitraryScope.ParentIndexKey);
			    Assert.Equal(18, arbitraryScope.StartingIndexInclusive);
			    Assert.Equal(27, arbitraryScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.ArbitraryCodeBlockNode, arbitraryScope.CodeBlockOwner.SyntaxKind);
			    
			    Assert.Equal(2, arbitraryScope.CodeBlockOwner.GetChildList().Count);
			    Assert.Equal(SyntaxKind.OpenBraceToken, arbitraryScope.CodeBlockOwner.GetChildList()[0].SyntaxKind);
			    Assert.Equal(SyntaxKind.VariableReferenceNode, arbitraryScope.CodeBlockOwner.CodeBlockNode.GetChildList().Single().SyntaxKind);
			}
		}
    }
    
    [Fact]
    public void Keyword_CreatesScopeWithOpenAndCloseBraceToken_ButGetsSingleStatementBody()
    {
    	// Erroneous behavior:
    	// ===================
    	//
    	// A keyword that creates a code block
    	// (either with OpenBraceToken and CloseBraceToken, or as a "single statement body" deliminated by a semicolon)
    	// At times will use the 'OpenBraceToken and CloseBraceToken' syntax, yet capture
    	// the next statement inside 'OpenBraceToken and CloseBraceToken' deliminated code block
    	// to be '"single statement body" deliminated by a semicolon' syntax.
    	//
    	// In the cases where this happens, "simplifying" the keyword's syntax within the parentheses (if it has this syntax)
    	// can sometimes fix the issue.
    	//
    	// As well, it seems that not all statements will be taken erroneously as the "single statement body".
    	// It is believed that at one point changing a FunctionInvocation statement to a variable declaration
    	// and assignment statement "fixed" this issue, but this cannot currently be replicated anymore it seems.
    	//
    	// More Details:
    	// -------------
    	// 
    	// The following code correctly puts the scope at the OpenBraceToken and CloseBraceToken:
    	// |
    	// ````if (false)
		// ````{
		// ````	_queue.RemoveLast();
		// ````}
		// 
		// The following code erroneously puts the scope at the StatementDelimiterToken that appears at the end of the 3rd line of the text.
		// |
		// ````if (batchEvent is not null)
		// ````{
		// ````	_queue.RemoveLast();
		// ````}
		//
		// It appears that the issue is with parsing an expression, and erroneously consuming an OpenBraceToken.
		// The console messages show that a 'BadExpressionNode + OpenBraceToken => BadExpressionNode'
		// Note the 'OpenBraceToken', this message is saying that it was consumed.
    
    	var test = new Test(
@"if (batchEvent is not null)
{
	_queue.RemoveLast();
}".ReplaceLineEndings("\n"));

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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // If statement scope
				var ifStatementScope = binderSession.ScopeList[1];
				Assert.Equal(1, ifStatementScope.IndexKey);
			    Assert.Equal(0, ifStatementScope.ParentIndexKey);
			    Assert.Equal(28, ifStatementScope.StartingIndexInclusive);
			    Assert.Equal(53, ifStatementScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.IfStatementNode, ifStatementScope.CodeBlockOwner.SyntaxKind);
			}
		}
    }
    
    [Fact]
    public void If_IsPatternMatchVariableDefinition_Misses_OpenBraceToken()
    {
    	// Erroneous behavior:
    	// ===================
    	//
    	// The OpenBraceToken of the if statement is being missed, and the scope for the IfStatementNode
    	// is placed at the StatementDelimiterToken which is at the end of the 3rd line of the text.
    
    	var test = new Test(
@"if (child is ISyntaxNode syntaxNode)
{
	WriteChildrenIndentedRecursive(syntaxNode, ""node"", indentation + 1);
}".ReplaceLineEndings("\n"));
	}

	[Fact]
    public void Statement_Inside_If_CodeBlock_Causes_Miss_CloseBraceToken_CaseA()
    {
    	// Erroneous behavior:
    	// ===================
    	//
    	// The issue occurs with the following input (and probably some others):
    	// ````if (false)
		// ````{
		// ````	_queue.RemoveLast();
		// ````}
		//
		// But, it does NOT occur with this input:
		// ````if (false)
		// ````{
		// ````	var x = _queue.RemoveLast();
		// ````}
		//
		// The scope should start at the OpenBraceToken, and end at the CloseBraceToken.
		//
		// However, in the erroneous example, the CloseBraceToken is somehow skipped over,
		// and the IfStatementNode will use the next CloseBraceToken as the end of the scope
		// (if there happens to be another CloseBraceToken deeper in the text file).
    
    	var test = new Test(
@"if (false)
{
	_queue.RemoveLast();
}".ReplaceLineEndings("\n"));

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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // If statement scope
				var ifStatementScope = binderSession.ScopeList[1];
				Assert.Equal(1, ifStatementScope.IndexKey);
			    Assert.Equal(0, ifStatementScope.ParentIndexKey);
			    Assert.Equal(28, ifStatementScope.StartingIndexInclusive);
			    Assert.Equal(53, ifStatementScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.IfStatementNode, ifStatementScope.CodeBlockOwner.SyntaxKind);
			}
		}
    }
    
    [Fact]
    public void Statement_Inside_If_CodeBlock_Causes_Miss_CloseBraceToken_CaseB()
    {
    	// See 'Statement_Inside_If_CodeBlock_Causes_Miss_CloseBraceToken_CaseA' above this.
    	// This is an extra test to ensure that a fix to the erroneous case doesn't break
    	// the related, and working, case.
    
    	var test = new Test(
@"if (false)
{
	var x = _queue.RemoveLast();
}".ReplaceLineEndings("\n"));

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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // If statement scope
				var ifStatementScope = binderSession.ScopeList[1];
				Assert.Equal(1, ifStatementScope.IndexKey);
			    Assert.Equal(0, ifStatementScope.ParentIndexKey);
			    Assert.Equal(28, ifStatementScope.StartingIndexInclusive);
			    Assert.Equal(53, ifStatementScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.IfStatementNode, ifStatementScope.CodeBlockOwner.SyntaxKind);
			}
		}
    }
    
    [Fact]
    public void Global_TypeDefinitionNode_Depth_CodeBlock_Surface_CodeBlock()
    {
    	// The first codeblock will not have its ending brace parsed properly.
    	
    	// 0: PublicTokenKeyword
    	// 1: ClassTokenKeyword
    	// 2: IdentifierToken
    	// 3: OpenBraceToken
    	// 4: 	OpenBraceToken
    	// 5: 	CloseBraceToken
    	// 6: CloseBraceToken
    	// 7: OpenBraceToken
    	// 8: CloseBraceToken
    	// 9: EndOfFileToken
    	
    	var test = new Test(
@"
public class Aaa
{
	{
	}
}

{
}
".ReplaceLineEndings("\n"));

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
			Assert.Equal(SyntaxKind.GlobalCodeBlockNode, globalScope.CodeBlockOwner.SyntaxKind);
		    
		    { // If statement scope
				var ifStatementScope = binderSession.ScopeList[1];
				Assert.Equal(1, ifStatementScope.IndexKey);
			    Assert.Equal(0, ifStatementScope.ParentIndexKey);
			    Assert.Equal(28, ifStatementScope.StartingIndexInclusive);
			    Assert.Equal(53, ifStatementScope.EndingIndexExclusive);
			    Assert.Equal(SyntaxKind.IfStatementNode, ifStatementScope.CodeBlockOwner.SyntaxKind);
			}
		}
    }
    
    [Fact]
    public void Nested_For_Loops_SingleStatementBody()
    {
    	var test = new Test(
@"
for (int i = 0; i < 5; i++)
	for (int q; q < 5; q++)
		Console.WriteLine(""Abc123"");
".ReplaceLineEndings("\n"));
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
			else if (child is SyntaxToken syntaxToken)
			{
				Console.WriteLine($"{childIndentation}{child.SyntaxKind}__{syntaxToken.TextSpan.GetText()}");
			}
		}
		
		if (indentation == 0)
			Console.WriteLine();
    }
}
