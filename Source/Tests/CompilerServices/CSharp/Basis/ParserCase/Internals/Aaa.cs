namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

/*
Goal: (2024-10-18)
==================

- [ ] Parse Scope properly (these syntax contain a close brace and it gets erroneously used to close the encompasing scope).
	- [ ] Object initialization
	- [ ] Collection initialization
*/

public class MyClassAaa
{
	public MyClassAaa FactoryMethodOne(string firstName, string lastName)
	{
		return new MyClassAaa
		{
			FirstName = firstName,
			LastName = lastName,
		};
	}
	
	public MyClassAaa FactoryMethodTwo(string firstName, string lastName)
	{
		var aaa = new MyClassAaa
		{
			FirstName = firstName,
			LastName = lastName,
		};
		
		return aaa;
	}
	
	public MyClassAaa FactoryMethodThree(string firstName, string lastName)
	{
		return new MyClassAaa()
		{
			FirstName = firstName,
			LastName = lastName,
		};
	}
	
	public MyClassAaa FactoryMethodFour(string firstName, string lastName)
	{
		var aaa = new MyClassAaa()
		{
			FirstName = firstName,
			LastName = lastName,
		};
		
		return aaa;
	}
	
	public MyClassAaa FactoryMethodFive(string firstName, string lastName)
	{
		var aaa = new MyClassAaa
		{
		};
		
		return aaa;
	}
	
	public MyClassAaa FactoryMethodSix(string firstName, string lastName)
	{
		var aaa = new MyClassAaa()
		{
		};
		
		return aaa;
	}
	
	public string FirstName { get; set; }
	public string LastName { get; set; }
}
