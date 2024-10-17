namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public record struct MyRecordStruct(int Aaa)
{
	public MyRecordStruct() : this(0)
	{
	}

	public int Number { get; set; }

	public void MyMethod()
	{ 
	}
}

