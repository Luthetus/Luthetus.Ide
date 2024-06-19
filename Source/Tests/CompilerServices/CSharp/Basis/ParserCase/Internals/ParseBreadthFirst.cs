using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseBreadthFirst
{
	/// <summary>
	/// I need to enter the class definition, then see the 'MyClass' constructor BUT, do not enter the constructor scope.
	/// Instead continue at the class definition scope Thereby finding the FirstName property,
	/// and somehow "remember" to go back and inside the constructor scope.
	/// </summary>
	[Fact]
	public void One_ClassDefinitions()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClass
{
	public MyClass(string firstName)
	{
		FirstName = firstName;
	}

	public string FirstName { get; set; }
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNode = (TypeDefinitionNode)topCodeBlock.ChildList.Single();
		Console.WriteLine(typeDefinitionNode);
	}

	[Fact]
	public void Two_ClassDefinitions()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"public class MyClassOne
{
	public MyClass(string firstName)
	{
		FirstName = firstName;
	}

	public string FirstName { get; set; }
}

public class MyClassTwo
{
	public MyClass(string firstName)
	{
		FirstName = firstName;
	}

	public string FirstName { get; set; }
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var typeDefinitionNodeOne = (TypeDefinitionNode)topCodeBlock.ChildList[0];
		var typeDefinitionNodeTwo = (TypeDefinitionNode)topCodeBlock.ChildList[1];
	}

	[Fact]
	public void Namespace_Test()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"namespace MyNamespace;";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();
	}

	[Fact]
	public void Namespace_FileScope_EmptyClass()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"namespace MyNamespace;

public class MyClass
{
	
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.ChildList.Single();
	}

	[Fact]
	public void Namespace_BlockScope_EmptyClass()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"namespace MyNamespace
{
	public class MyClass
	{
	
	}
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.ChildList.Single();
	}

	[Fact]
	public void Namespace_FileScope_Class()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"namespace MyNamespace;

public class MyClass
{
	public string FirstName { get; set; }
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.ChildList.Single();

		var propertyDefinitionNode = (PropertyDefinitionNode)typeDefinitionNode.TypeBodyCodeBlockNode.ChildList.Single();
	}

	[Fact]
	public void Namespace_BlockScope_Class()
	{
		var resourceUri = new ResourceUri("UnitTests");
        var sourceText =
@"namespace MyNamespace
{
	public class MyClass
	{
		public MyClass()
		{
		}

		public string FirstName { get; set; }
	}
}";

        var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;

		var namespaceStatementNode = (NamespaceStatementNode)topCodeBlock.ChildList.Single();

		var typeDefinitionNode = (TypeDefinitionNode)namespaceStatementNode.CodeBlockNode.ChildList.Single();

		var constructorDefinitionNode = (ConstructorDefinitionNode)typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[0];
		var propertyDefinitionNode = (PropertyDefinitionNode)typeDefinitionNode.TypeBodyCodeBlockNode.ChildList[1];

		Assert.Equal(0, compilationUnit.DiagnosticsList.Length);
	}
}
here.
Then on this interface the property to know if "upwards and downwards" scope is supported
would exist.

I made all the changes I'd previously described.

At this point the TokenWalker is changing its underlying '_index'
to the opening of the child scope, then tracking the token
index for the closing of the child scope.

When '_index' gets to the closing, then restore the original _index.

But, I need to set the Stack somehow to what it was at the time
that I first saw the opening of the child scope.

For example:

public class MyClass
{
}

I'm seeing the open brace, then tracking this spot.

But when I return to this spot, the fact that its a class definition
is lost. So, the parser thinks its just an arbitrary code block.

I know that I'm looking at a class definition when I track the spot,
so I can either recreate the stack when I return, or
I track the start of the class definition instead of its start of body.

Then the parser would once again parse over the "public class" part and realize
its a class definition.

*/

/*
Inside of ParseTokens.ParseOpenBraceToken(...)
I have an if statement.

And, this if statement checks if the current scope's
scope direction is both or not.

If it is both, do not enter the child scope, instead
enqueue the child scope to be parsed once the
closing brace of the current scope is found (but not yet handled).

When I comment out the if statement, it results in the code running as
thought the current scope's scope direction were down.

If I run this unit test
(which is a file scoped namespace which has a type definition in it),
and I do NOT comment out the enqueue code, then this test will fail.

But, if I comment out the enqueue code, thereby running this test
as if the namespace's scope were only down, then this test passes.

Inside of the CSharpParser's Parse(...) method
I have a Console.WriteLine():
Console.WriteLine($"{nameof(Parse)}:{token.SyntaxKind}");

Not including the enqueueing code, then this console output
is the following:
```
Parse:NamespaceTokenKeyword
Parse:StatementDelimiterToken
Parse:PublicTokenKeyword
Parse:ClassTokenKeyword
Parse:OpenBraceToken
Parse:CloseBraceToken
Parse:EndOfFileToken
```

With the enqueueing code, then this console output
is the following:
```
Parse:NamespaceTokenKeyword
Parse:StatementDelimiterToken
Parse:PublicTokenKeyword
Parse:ClassTokenKeyword
Parse:OpenBraceToken
Parse:EndOfFileToken
```

If I put these console output side by
side, then it is the following:

No enqueue                    | With enqueue
Parse:NamespaceTokenKeyword   | Parse:NamespaceTokenKeyword
Parse:StatementDelimiterToken | Parse:StatementDelimiterToken
Parse:PublicTokenKeyword      | Parse:PublicTokenKeyword
Parse:ClassTokenKeyword       | Parse:ClassTokenKeyword
Parse:OpenBraceToken          | Parse:OpenBraceToken
Parse:CloseBraceToken         | Parse:EndOfFileToken
Parse:EndOfFileToken          |

The summary of differences is that:
'Parse:CloseBraceToken' was never written out.

This implies to me that the TokenWalker is restoring
an index that is 1 too large.

Or, that the TokenWalker is restoring the correct index,
and then immediately consuming it somehow.

Summary of my thoughts:
=======================
- I wonder if I need to capture any state of the ParserModel
      at the time that the enqueueing is performed.
      - The idea here being that parsing of a class might
		    be missing its preceeding StorageModifierKind
            or the AccessModifierKind kind, if one solely
            is tracking the open brace token.
      - A counter point to this idea, is that the 'ParseOpenBraceToken(...)'
            method seems to always pop the most recent syntax off the
            ParserModel.SyntaxStack, in which this most recent syntax
            is a TypeDefinitionNode, with null for the code block,
            but everything else is set.
            - I mentioned 'TypeDefinitionNode' specifically,
                  but this holds true for 'NamespaceStatementNode' and etc...
            - Therefore, perhaps I only need to push that singular
                  "most recent syntax" back onto the stack when I dequeue.

I want to wite out what I think the code is/should be doing,
for the enqueue to work.
===========================
Parse:NamespaceTokenKeyword
Parse:StatementDelimiterToken
Parse:PublicTokenKeyword
Parse:ClassTokenKeyword
Parse:OpenBraceToken
TokenWalker:FindCloseBraceToken
Parse:EndOfFileToken

Actually... I think the indices are off by
one beause the while loop 'consumes' the token,
i.e.: it moves the index forward by 1 after returning
the token that was current.

I want to console write out TokenWalker.TokenList[index];
Where 'index' is all the indices I'm tracking for the
enqueueing code. I presume the output won't
be what I intended.

The output was:
===============
Parse:NamespaceTokenKeyword
Parse:StatementDelimiterToken
Parse:PublicTokenKeyword
Parse:ClassTokenKeyword
Parse:OpenBraceToken
TokenWalker::startIndexInclusive::7::CloseBraceToken
TokenWalker::endIndexExclusive::8::EndOfFileToken
Parse:EndOfFileToken

What immediately stands out in the console write output, is that
the 'startIndexInclusive' is pointing to the class definition's
CloseBraceToken.

This makes sense considering that I was thinking how I am off by 1.
There is no syntax between the open and closing braces of the type
definition, since it is empty.

So off by 1 puts me at the close brace.

I went and subtracted 1 from the 'startIndexInclusive', and
now I'm seeing the 'startIndexInclusive' correctly pointing at
the 'OpenBraceToken' for the type definition.

Yet, even with the off by 1 for the 'startIndexInclusive' having
been fixed, the 'Parse:...' output lines are identical.

The initial 'Parse:OpenBraceToken' makes sense,
as what follows is that the parsing of the open brace
gets enqueued.

With the off by 1 fixed, it is intended that the
'Parse:OpenBraceToken' would occur for a second time.

I added a console write line to the 'DeferredParsing(...)'
method, which should be getting invoked upon dequeueing.

But, this output never got written, the 'DeferredParsing(...)'
isn't getting invoked.

The reason appears to be my reliance on a close brace token,
being the indicator for the end of the parent/current scope.

Since this test uses a file scoped namespace, then the dequeueing never occurs.

*/

/*
This test is failing, and I'm getting the following output:
===========================================================
Parse:NamespaceTokenKeyword
Parse:OpenBraceToken
Parse:PublicTokenKeyword
Parse:ClassTokenKeyword
Parse:OpenBraceToken
TokenWalker::startIndexInclusive::6::OpenBraceToken
TokenWalker::endIndexExclusive::8::CloseBraceToken
Parse:CloseBraceToken
DeferredParsing
Parse:OpenBraceToken
TokenWalker::startIndexInclusive::6::OpenBraceToken
TokenWalker::endIndexExclusive::8::CloseBraceToken
Parse:EndOfFileToken

I think I see the issue.

When I first started working on this feature,
I added to the 'ParseOpenBraceToken(...)' method
the optional bool argument 'bool wasEnqueued = false'.

The idea here was to re-invoke 'ParseOpenBraceToken(...)',
but with wasEnqueued set to true, so that the child scope
wasn't just yet again enqueued.

But, at some point I had the idea that its best to
return to the main while loop.

This would allow child scopes to be parsed
exactly the same as anything else,
i.e.: the main while loop.

For that reason I made 'ParseCloseBraceToken(...)'
invoke the ParseChildScopeQueue's action,
then return to the main while loop.
Here, the "ParseChildScopeQueue's action"
is just changing the token index,
and telling the token walker to restore
the index at which lies the end of the parent scope,
once the child scope is found.

Even if what I describe here were to work,
it is written in a rather "spaghetti code" way.
I like to get a feel for a problem that I'm trying
to solve, by writing code to work through it step by step.
Even if I delete all of the written code afterwards.
I believe this is the most effective way to solve
a problem, provided the environment is safe in which
the experimental code is being written/ran.

If I spend too long a time planning out a solution.
If it is a problem I've never solved before,
I'm likely to plan out a delusional solution.
And the moment I sit down to type it out,
I'll immediately realize the entire plan was wrong from the start.

The more experience I have with a problem, the more
I value the planning phase.

And, the less experience, I focus on finding
a safe way to experiment and build my intuition for
the reality of the problem.

It can backfire to work this way though.
If you simply get the initial solution working,
but don't thoroughly think out a final solution,
then one ends up being strangled by spaghetti code
and their project gets into an unrecoverable state
of nonsensical code.

I guess with problems I don't understand,
I like to sort of do the oppose of plan, then do.
I do (safely) experiments, then plan the solution after.
I don't believe in magically understanding a problem.
If you've never seen the problem before, you
need to get uncomfortable and grapple with it,
to truly understand its nature.

As for problems one has solved before,
planning nearly all of the solution ahead of time
is the best way. Such as a CRUD application.

What was I doing again?

I remember now, the optional boolean approach was written,
but then I took a different approach. And current,
theres a sort of mix of the two and its getting me nowhere.

I'm going to remove the optional boolean then,
and go from there.

Fun fact, that singular optional boolean was causing
the entire file to throw an exception during parsing,
thereby I only had syntax highlighting for the
lexer's output.

The moment I removed the optional boolean, the file
got very colorful.

With the removal of the boolean, I need some other way
to know when I'm inside of the method 'ParseOpenBraceToken(...)'
with the goal of enqueueing or that I've been dequeued and need
to actually parse it then and there.

I added an int property named 'DequeueChildScopeCounter' to the
'ParserModel'.

If the 'ParseCloseBraceToken' dequeues, it can then increment
this value.

Then the next time one invokes 'ParseOpenBraceToken(...)',
the method internally will see that the counter is > 0,
and parse instead of enqueue.

I made the changes, and now the RootCodeBlockNode's single child
is no longer the 'NamespaceStatementNode', but instead
the 'TypeDefinitionNode'.

So, somehow the child/parent relations are getting mixed up.

Inside of 'ParseCloseBraceToken(...)' there is code to
change the 'model.CurrentCodeBlockBuilder' to the
'model.CurrentCodeBlockBuilder.Parent'.

This makes me think that the type definition for some reason
isn't setting the 'NamespaceStatementNode' as the 'model.CurrentCodeBlockBuilder'.

Furthermore, the reason the 'TypeDefinitionNode' is a child node to the
compilationUnit.RootCodeBlockNode, I think is because the 'TypeDefinitionNode'
was the 'model.CurrentCodeBlockBuilder' once the main while loop finished,
and therefore it was put on the global code block builder.

If the 'TypeDefinitionNode' correctly sets the 'model.CurrentCodeBlockBuilder'
to the 'NamespaceStatementNode' after its deferred parsing is performed,
then the global code block builder would take its child to be
the 'NamespaceStatementNode' instead (and correctly).

As well, the 'NamespaceStatementNode' has the 'TypeDefinitionNode'
as its child, so all would be correct.

The console output, with the logging, seems correct.
That is, I see 'Parse:CloseBraceToken' twice in the output.
The first time is during the enqueue,
then the second time is because of the dequeue, which will
actually parse the token this time around.

Well, since this is a block scoped namespace I'm
being a fool. The output looks wrong then.

Yeah the output is longer than what I was looking at.:
Parse:NamespaceTokenKeyword
Parse:OpenBraceToken
Parse:PublicTokenKeyword
Parse:ClassTokenKeyword
Parse:OpenBraceToken
TokenWalker::startIndexInclusive::6::OpenBraceToken
TokenWalker::endIndexExclusive::8::CloseBraceToken
Parse:CloseBraceToken
enter::ParseCloseBraceToken::NamespaceStatementNode
DeferredParsing
Parse:OpenBraceToken
DequeueChildScopeCounter::NamespaceStatementNode
Parse:CloseBraceToken
enter::ParseCloseBraceToken::TypeDefinitionNode
middle::ParseCloseBraceToken::TypeDefinitionNode
middle::ParseCloseBraceToken::PARENT::NamespaceStatementNode
middle::ParseCloseBraceToken::Stack::True
endif::ParseCloseBraceToken::NamespaceStatementNode
Parse:EndOfFileToken

*/