using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseScopeTests
{
	/// <summary>
	/// Goal: correctly parse all the scopes (2024-10-13)
	/// =================================================
	///
	/// Example:
	/// --------------------------------------------------------------------------------
	/// 	namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;
	/// 	
	/// 	public class ParseScopeTests
	/// 	{
	/// 		
	/// 	}
	///
	/// --------------------------------------------------------------------------------
	///
	/// When the cursor enters a child scope, it seemingly cannot return
	/// to the parent scope (this is erroneous).
	///
	/// The source text defines a file scoped namespace,
	/// then inside of that a type definition.
	///
	/// If one starts with the cursor at positionIndex 0,
	/// then presses 'ArrowDown' until they reach the end of the file,
	/// they'll see that the scopes appear to be fine.
	///
	/// But, once one reaches the end of the file it is shown
	/// that your current scope is within the type definition.
	///
	/// However, that type definition ended a few characters prior
	/// to the end of the file.
	///
	/// So, why is the type definition erroneously the active
	/// scope in this situation?
	///
	/// Generally, it appears that the wording of the problem is
	/// that a child scope cannot return to its parent scope.
	///
	/// The reason for this statement is that sibling nodes
	/// don't seem to be an issue.
	///
	/// Example:
	/// --------------------------------------------------------------------------------
	/// 	namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;
	/// 	
	/// 	public class OneParseScopeTests
	/// 	{
	/// 		
	/// 	}
	///	 
	/// 	public class TwoParseScopeTests
	/// 	{
	/// 		
	/// 	}
	/// 
	/// --------------------------------------------------------------------------------
	/// 
	/// In this example, both OneParseScopeTests and TwoParseScopeTests appear
	/// to have their scope correctly parsed.
	///
	/// But again, upon reaching the end of file,
	/// the most recently parsed scope seems to still be active.
	///
	/// In this case 'TwoParseScopeTests' has its scope active,
	/// but again it had closed its scope a few characters
	/// prior to the end of file.
	///
	/// It is of note though, after parsing OneParseScopeTests
	/// that TwoParseScopeTests was still correctly parsed,
	/// even though upon leaving OneParseScopeTests scope
	/// the active scope remained as 'OneParseScopeTests'
	/// rather than be returned to the parent scope.
	///
	/// i.e.: if the user's cursor is between the two
	/// type definitions, they'll see their active scope
	/// as the first type definition (erroneously).
	///
	/// -----------------------------------------------------
	///
	/// The issue is fixed.
	///
	/// It's funny because I was so full of anxiety and dread
	/// that this wasn't working.
	///
	/// I was struggling to bring myself to look at the code.
	///
	/// But, I took one look at the
	/// 'public IScope? GetScope(ResourceUri resourceUri, int positionIndex)'
	/// method.
	///
	/// And instantly I saw it and was like, "yeah that'll do it"
	/// I never compared the ending of the scope, only the start of the scope.
	///
	/// Oddly enough this was only for this positionIndex overload.
	/// The other overloads were correct so I just copy and pasted it.
	/// 
	/// Even better the TextSpan overload now just invokes
	/// the positionIndex overload instead of copy/paste duplicating code.
	///
	/// In other words: the issue was getting the scope,
	/// not that the scopes were parsed incorrectly.
	///
	/// That being said, there are some cases of scopes being parsed
	/// incorrectly. These being object/collection initialization,
	/// so I can fix that next.
	/// </summary>
	[Fact]
	public void Aaa1()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
 
 	   // There is an extra space on the final line of this text
		// for the sake of my sanity, so I can go 1 positionIndex further
		// in addition to the other spot. My head is just yelling at
		// me with anxiety to do this.
        var sourceText = @"namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseScopeTests
{
	
}
 ";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

        var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();
        var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.ChildList.Single();
    }
}
