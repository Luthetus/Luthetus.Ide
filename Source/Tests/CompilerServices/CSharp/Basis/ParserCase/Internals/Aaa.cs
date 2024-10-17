namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

/*
Goal: (2024-10-17)
==================

- [x] Primary Constructor Syntax
	- [x] At what scope are these things being declared at?
	- [x] 'record'
	- [x] 'record struct'
	- [x] 'class'  // Similarly as done for 'struct'. If they don't have the correct language version of C# let the compiler tell them that for now.
			       //
	- [x] 'struct' // Whether 'struct' is allowed or not to have one, it might be a better experience to allow the parser to
	               // read it as if were allowed syntax, then have the C# compiler tell them the error(?).
- [ ] 'var' contextual keyword as a type clause
	- [ ] If it is the first token in a scope, it doesn't work. But if it comes after the first token it does.
- [x] Instead of reconstructing an object to set an immutable property,
	add a private setter, and a public method.
- [ ] Finalize
	- [ ] Update NuGet packages
	- [ ] Accept PR
	- [ ] Check website

*/

/*public void MyMethod()
{
	var aaa = 1;
	var bbb = 2;
}*/
