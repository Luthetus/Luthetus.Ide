using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase.Internals;

/*
This file was made after working on 'ParsePrototypes.cs'
and getting the idea that breadth first search is a requirement
(probably other ways to do this like parsing twice or something I'm not sure).

One has to perform a breadth first search per scope?

I should be parse starting with top level statements.

From there any immediate child scopes need to be breadth first searched, do not go
any deeper you need to handle the topmost scopes first?

How would I handle one C# class in some file, referencing a C# class in another file?

If by happenstance I parse the first C# class, prior to parsing the class being referenced,
then the reference would be an error?

Do I therefore need to create a dependency graph?

Would I create a dependency graph prior to parsing?

How would I know the dependencies prior to parsing?

What if there is a circular reference, how do I detect this,
as opposed to a possible infinite loop in the parser?
*/

public class ParseBreadthFirst
{
	/// <summary>
	/// I need to enter the class definition,
	/// then see the 'MyClass' constructor BUT, do not enter the constructor scope.
	/// Instead continue at the class definition scope
	/// Thereby finding the FirstName property,
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

/*
I want to type out what I would do if someone asked me as a human to manually parse this.

The test data starts with only a class definition at the top level.
But, algorithmically I would start at character index 0,
I see a class definition,

so I skip ahead to its opening brace, then I skip until its ending brace.

I go to the next character and  its the end of the file.

So, now I have finished parsing the top most scope, but how do I go back and
parse the internals of the class definition.

I need to restart from character index 0.
I'm presuming that for any syntax, I would know the parts of it that I
come back to. What I mean is, the class internals are bounded by brace characters.
And, a second example would be a function definition, the arguments I come back to,
and they are bounded by parenthesis, therefore I can do that. And the function code block
is bounded by brace characters so I can come back to that as well.

I can mark the start and stop of each sub-syntax?

A function has arguments.
And yet, what if the function returns a type that was defined in the same file
but at deeper into the document?

Presumably this isn't an issue because again, I just mark where these "sub-syntax"(s)
are.

I'm not looking to make the nodes mutable though. So how do I construct the node initially,
then fill in the rest later, while still being immutable?

The first idea is to have a 'Builder' pattern.

Then, I can parse each scope, breadth first, and afterwards iterate from the top to bottom of that scope
and if the node is a 'Builder' then I know it isn't finished. It needs to be parsed again
but at the scope of the syntax node itself.

Its sort of odd to think about but a generic function definition,
it has the generic argument deeper in the document then the return type,

public T Copy<T>(T item)
{
	
}

With the copy method I wrote here. The
return type is 'T', and the argument list
contains an entry of which the type is 'T'.

Where is the type 'T'? It is after the function identifier.

So, given naive parsing of this, the argument list
could know of the type 'T' since it is deeper in the document.

But, the return type 'T' comes prior to the generic argument,
and so a naive parsing of this would fail to know of the type 'T'
in the return signature.

I suppose what I'm getting at is, there seems to be not just
an order in which one parses "scopes" but also an order
for parsing any syntax at all.

All the syntaxes need to define the order in which to parse their "sub-syntax"?

Regarding the 'Builder' idea. Am I going to create a second class for every node type?
In other words TypeDefinitionNode needs a TypeDefinitionNodeBuilder.
Or do I use one 'Builder' type for all the nodes?

If the child list of nodes is immutable. Then everytime I 'Build' a 'Builder'
I'd need to replace the entire list of nodes?

I guess I can start with "create a second class for every node type",
and only focus on creating a second class for those that are necessary
to solve this unit test specifically.

Then after I solve this unit test, I'll have a better intuition
for how to go about things.

I don't need to add the nodes as I see them?

I can just track the "would be index" alongisde each node.

Then build the list at the end?

I need to see where the class definition parsing code is.
Because I want to identify the point that it goes into the
class's code block.

I'm not sure what I want to do but I just think
its a good start.

- Parse(...)
	  - while (true)
	      - ParseKeywordToken(...)
	            - HandleClassTokenKeyword(...)
		              - HandleStorageModifierTokenKeyword(...)
		                    - model.SyntaxStack.Push(typeDefinitionNode);
				            - return;
		              - return;
	            - return;
	      - continue;

I'm noticing that the class's code block does not track the open and closing braces?

There is a method named 'HandleStorageModifierTokenKeyword(...)'.
This method accepts a parameter 'ISyntaxToken consumedStorageModifierToken'.
Therefore, one would presume if I invoked 'HandleStorageModifierTokenKeyword(...)',
that I would provide a "storage modifier token".

But when parsing the class token keyword, I pass the 'class' token to the
'HandleStorageModifierTokenKeyword(...)'.

And the argument 'ISyntaxToken consumedStorageModifierToken' is actually the
'class' keyword?!?!?

Maybe I wrote the method with the idea that one provides the preceeding keyword,
and I named the method argument incorrectly?

================================================================================

I slept and came back to this file.

I don't know what I talkking about with the
"And the argument 'ISyntaxToken consumedStorageModifierToken' is actually the 'class' keyword?!?!?"
comment.

The storage modifier tokens are:
{
	Struct,
    Class,
    Interface,
    Enum,
    Record
}

So I'm not sure what I was getting at yesterday.

Interestingly, in 'HandleStorageModifierTokenKeyword(...)' I
currently am constructing a 'TypeDefinitionNode' which has not yet
had its code block parsed (among other things).

var typeDefinitionNode = new TypeDefinitionNode(
    accessModifierKind,
    hasPartialModifier,
    storageModifierKind.Value,
    identifierToken,
    valueType: null,
    genericArgumentsListingNode,
    primaryConstructorFunctionArgumentsListingNode: null,
    inheritedTypeClauseNode: null,
    typeBodyCodeBlockNode: null);

So where do I go from here?

I am pushing the incomplete typeDefinitionNode onto a stack.

Seemingly I then follow a sequence of 'return' statements back to the
CSharpParser's top level method with the while true loop over the tokens.

Maybe I can type what I think the stack is at this point.

Stack =
{
	***PUSH_AND_POP_LOCATION is on this side***
	
	typeDefinitionNode,
}

And I can write out the method invocations as they occur,
I'm at the while (true) here.

I'm not sure where the code will go next, so I'm adding
'Console.WriteLine(token.SyntaxKind);' temporarily
so I can see what token we are at.

Somehow, I am going from 'HandleStorageModifierTokenKeyword(...)'
to the OpenBraceToken.

Okay, I'm just being a fool that makes sense.

"public class MyClass { }"

With the knowledge that 'HandleStorageModifierTokenKeyword(...)'
awkwardly consumes the identifier token "MyClass",
then yes an open brace token at this step makes sense.

And, this step seems pivotal to the change I'm trying to make.
I don't want to go into the OpenBraceToken, I want to store
its start position, then come back after I parse everything else
at my current scope.

Considering that I've been looking at the code for
parsing a class definition.

It would perhaps be best that I look at two unit tests
for the moment.

One test having a single class definition,
the other test having two class definitions.

For the single class definition, I need to assert that
the open brace token was not entered, but instead skipped.
And that they just happened to be no further code at that scope,
so therefore go back to the open brace token and begin parsing
the deeper scope.

Got the two class definitions, I neeed to do
the same thing as with the single class definition.
i.e.: assert that the first class definition does not enter the
the open brace token for the class's code block,
but instead skips to the next syntax at the same scope level.
In this case there would be another syntax, its the second class definition,
so start parsing it, but again, do not enter the second class's body.
Now we get to the end of the scope, so go back to the first class's body,
then the second classes body.
And assertions on the order of these things would be very good to include.

If the Stack and the current code block's child list are used,
then the Stack allows for storing the 'changing' nodes that are not finalized.

Then once they are finalized, then move them to the code block's child list.

But, a stack would reverse the order of the child nodes,
when they are moved to the current code block's child list.

I can insert into the current code block's child list at index 0,
but would this mean every insertion needs to shift the other entries?

Is there a performance issue in relation to this shifting
being done every insertion?

If the stack where instead a queue, then I could insert at the end
of the current code block's child list and avoid any shifting.

The stack is nice, because I can see a token,
and in a sense say, "I don't have enough information"
so I push it onto the stack.

Then as I work my way through the tokens, I might
make sense of that token which I didn't have enough information for.

A queue doesn't provide the same functionality, because
the parser already goes left to right,
the stack permits me to reverse parsing direction from right to left
temporarily, as needed.

But, maybe a queue which is dedicated to deferred parsing
would be of use?

The difference here being that it isn't about lack of information
necessarily, but instead that I explicitly want to come back to
that spot later, because it would create a scope,
and I haven't finished parsing the current scope.

Ugh, I have all the code going through that main while loop
though.

It sounds like a pain to store a similar loop,
to be iterated later.

Sort of thinking to myself that I'd have to duplicate
the while loop code, but with token indices to start and end at?

Althought, I have access to the token walker.
The token walker has the current token index...
maybe I can add code to the token walker
such that it handles tracking my 'inner scope while loops' for me.

Because the CSharpParser's while true loop is invoking
'var token = model.TokenWalker.Consume();'.

So, that TokenWalker.Consume(...) method could internally
track "inner while loops".

And it would be entirely abstracted from the CSharpParser's
while true loop.

So, if I continue from where I currently am,
the current token is an open brace token for the class definition.

I need to therefore enqueue the parsing from inclusive TokenWalker.Index
to some undertermined exclusive ending index.

The "undertermined exclusive ending index" would be the closing brace.
In my mind I'm feeling very uncomfortable because how would I handle
syntax "matching". So invalid syntax maybe a missing closing brace?

I guess the exclusive ending index would just be the matched token index...
or one more than that I'm not sure.

Also, an inner scope, which is invalid due to missing its closing token,
would it be closed by the closing token of the parent scope,
in order to perserve the validity of any scopes below it?

How would I track these things?

Anyway I should focus on the simple case first. Then build up test cases
over time, lest I just sit here worrying and making zero progress.

From the while true loop, the 'ParseOpenBraceToken(...)' method is invoked.

Gee, 'ParseOpenBraceToken(...)' is 130 lines of code.
I mean I guess thats not that many lines of code,
but I'm horribly sensitive to complexity at the moment
due to the anxiety I'm feeling while working on this feature.

The 'ParseOpenBraceToken(...)' has a lot of if, else if, else; going on.

I'm going to copy and paste it here and continually abstract away
parts of it.

public static void ParseOpenBraceToken(
    OpenBraceToken consumedOpenBraceToken,
    ParserModel model)
{
    var closureCurrentCodeBlockBuilder = model.CurrentCodeBlockBuilder;
    ISyntaxNode? nextCodeBlockOwner = null;
    TypeClauseNode? scopeReturnTypeClauseNode = null;

    {
        nextCodeBlockOwner = ...;

        model.FinalizeCodeBlockNodeActionStack.Push(codeBlockNode =>
        {
            closureCurrentCodeBlockBuilder.ChildList.Add(codeBlockNode);
        });
    }

    model.Binder.RegisterBoundScope(scopeReturnTypeClauseNode, consumedOpenBraceToken.TextSpan, model);

    if (model.SyntaxStack.TryPeek(out syntax) && syntax.SyntaxKind == SyntaxKind.NamespaceStatementNode)
    {
        var namespaceStatementNode = (NamespaceStatementNode)model.SyntaxStack.Pop();

        var namespaceString = namespaceStatementNode
            .IdentifierToken
            .TextSpan
            .GetText();

        model.Binder.AddNamespaceToCurrentScope(namespaceString, model);
        model.SyntaxStack.Push(namespaceStatementNode);
    }

    model.CurrentCodeBlockBuilder = new(model.CurrentCodeBlockBuilder, nextCodeBlockOwner);
}

I wonder if I could enqueue 'ParseOpenBraceToken(...)' exactly as it is,
then dequeue everything once I find a 'CloseBraceToken' of the current scope
that everything would "just work"?

Althought, not every programming language supports an "upwards and downwards" scope.
As well, even within C#, not every "code block owner" supports an "upwards and downwards" scope.

I think the "code block owner" needs to define whether they support "upwards and downwards" scope or not.

And yet, I need to invoke 'ParseOpenBraceToken(...)' in order to determine
the "code block owner" (given the current way the code is written).

Inside of 'ParseOpenBraceToken(...)' I'm thinking of adding code that would
invoke a new method named 'EnqueueParseChildScope(...)'.

But, I realize now, its not the to be parsed child scope that matters in relation to
whether it supports "upwards and downwards" scope.

It instead is the parent scope that I need to look at for whether it supports "upwards and downwards" scope.

Could a parent scope which supports "upwards and downwards" scope have an exception to this case
where a specific kind of child scope is only allowed to receive the downwards scope?

I guess it doesn't matter, because if this is the case, I can determine the child scope,
and I'll have both the parent and child scope, compare them, then choose to enqueue the child scope or not.
So, by "this doesn't matter", I mean to say, "this is already possible".

I wonder if a local function definition, within a method, can access the parent scope,
"upwards and downwards", regardless of where the local function definition exists
within that method?

This is kind of the inverse case where a "downwards" only scope gives a child
"upwards and downwards" scope. That is, if what I'm thinking even is a thing,
I need to check, but not get distracted at the moment.

Okay, inside 'ParseOpenBraceToken(...)' I have access to the variable
'model.CurrentCodeBlockBuilder'.

And, the type 'CodeBlockBuilder' has a property for the 'ISyntaxNode? CodeBlockOwner'

I want to have the codeBlockOwner tell me whether it supports "upwards and downwards" scope or not.
And yet, the CodeBlockOwner property has a very general interface for its type, 'ISyntaxNode?'.

Is it the case that all syntax nodes are capable of being a code block owner?
If it isn't the case, I'd prefer not add a property on every syntax node, where
some use it and others don't.

I think a new interface 'ICodeBlockOwner' could be useful here.
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
	public void Namespace_EmptyClass()
	{
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
*/

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
	public void Namespace_Class()
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
}
