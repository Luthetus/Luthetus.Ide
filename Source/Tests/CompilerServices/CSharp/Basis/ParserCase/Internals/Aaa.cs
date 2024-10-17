namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public record MyRecord(int Aaa)
{
	public MyRecord() : this(0)
	{
	}

	public int Number { get; set; }

	public void MyMethod()
	{
	}
}

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

// Struct too?
// Primary constructor with a semicolon?
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
