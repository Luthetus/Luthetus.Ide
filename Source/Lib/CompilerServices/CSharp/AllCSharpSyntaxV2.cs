/*// This demo file is just to showcase features and not intended to represent well written code.
namespace Luthetus.CompilerServices.CSharp;

public class Person : IPerson
{
	public Person(string firstName, string lastName)
	{
		FirstName = firstName;
		LastName = lastName;
	}

	public Key<Person> Id { get; } = Key<Person>.NewKey();
	public string FirstName { get; set; }
	public string LastName { get; set; }
	
	public string DisplayName => $"{FirstName} {LastName}";
}

public interface IPerson { }

public record PersonRecord(string FirstName, string LastName) // : IPerson; TODO: Inheritance here isn't working
{
	public string DisplayName => $"{FirstName} {LastName}";
}

public class PersonRepository
{
	private readonly List<Person> _people = new();
	
	public PersonRepository()
	{
		_people.Add(new Person("Jane", "Doe"));
		_people.Add(new("John", "Doe"));
	}
	
	public List<Person> GetPeople()
	{
		// Return a shallow copy.
		return new(_people);
	}
	
	public Person CreatePerson(string firstName, string lastName)
	{
		// Named parameters
		Person person = new(
			firstName: firstName,
			lastName: lastName);
		
		person.FirstName;
		//     ^ hover mouse here for member access tooltip.
		
		FirstName;
		// ^ contrast the 'member access tooltip' with what happens if the identifier is alone;
		
		return person;
	}
	
	public Person GetPerson(Key<Person> id)
	{
		return _people.Single(x => x.Id == id);
	}
}

public record struct Key<T>(Guid Guid)
{
    public static readonly Key<T> Empty = new Key<T>(Guid.Empty);

    public static Key<T> NewKey()
    {
        return new(Guid.NewGuid());
    }
}

public partial class Counter : ComponentBase
{
	[Parameter, EditorRequired]
	public string Text { get; set; } = null!;
	
	private int[] _array = new int[10];
	private int[][] _arrayJagged = new int[6][];
	
	// TODO: multi dimensional arrays. The parser does recover however even though it cannot parse this correctly.
	private int[,] multiDimensionalArray1 = new int[2, 3];
	
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
	}
}
*/