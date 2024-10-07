namespace Luthetus.CompilerServices.CSharp.Tests.Basis.DataForVisualizing;

public class ClassOne
{
	public string LastName { get; set; }

	public ClassOne(string firstName, string lastName)
	{
		{
			FirstName = firstName;
		}
		
		{
			LastName = lastName;
		}
	}
	
	public string FirstName { get; set; }
}
