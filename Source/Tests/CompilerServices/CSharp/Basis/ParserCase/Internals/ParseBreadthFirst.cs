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
	[Fact]
	public void Aaa()
	{
		throw new NotImplementedException();
	}
}
