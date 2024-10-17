namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class MyClass(int Aaa)
{
	public MyClass() : this(0)
	{
	}

	public int Number { get; set; }

	public void MyMethod()
	{
	}
}
