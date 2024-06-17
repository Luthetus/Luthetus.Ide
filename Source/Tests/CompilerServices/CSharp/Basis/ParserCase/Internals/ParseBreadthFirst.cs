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
	public void ClassDefinition()
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

		Assert.Equal(1, 2);

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
*/

		throw new NotImplementedException();
	}
}
