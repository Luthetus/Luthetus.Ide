namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public struct MyStruct(int Aaa)
{
	public MyStruct() : this(0)
	{
	}

	public int Number { get; set; }

	public void MyMethod()
	{
	}
}