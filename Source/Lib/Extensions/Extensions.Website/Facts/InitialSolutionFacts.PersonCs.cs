namespace Luthetus.Ide.Wasm.Facts;

public partial class InitialSolutionFacts
{
    public const string PERSON_CS_ABSOLUTE_FILE_PATH = @"/BlazorCrudApp/BlazorCrudApp.Wasm/Persons/Person.cs";
    public const string PERSON_CS_CONTENTS =
"""""""""
// This demo file is just to showcase features and not intended to represent well written code.
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

public record PersonRecord /**/ (string FirstName, string LastName) /**/ : /**/ IPerson /**/
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
	
	public Person CreatePerson_Constructor(string firstName, string lastName)
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
	
	public Person CreatePerson_ObjectInitialization(string firstName, string lastName)
	{
		Person person = new Person()
		{
			FirstName = firstName,
			LastName = lastName,
		};
		
		person = new()
		{
			FirstName = firstName,
			LastName = lastName,
		};
		
		return person;
	}
	
	public Person CreatePerson_Record(string firstName, string lastName)
	{
		PersonRecord person = new PersonRecord(firstName, lastName);
		
		person = person with
		{
			FirstName = "Asdfg",
		};
		
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
	[CascadingValue(Name=nameof(Layer))]
	public int Layer { get; set; } = null!;
	
	private readonly object _lock = new();
	
	private int[] _array = new int[10];
	private int[][] _arrayJagged = new int[6][];
	
	// TODO: multi dimensional arrays. The parser does recover however even though it cannot parse this correctly.
	private int[,] multiDimensionalArray1 = new int[2, 3];
	
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
	}
	
	public int GetLength(List<string> stringList)
	{
		var count = 0;
		
		foreach (var item in stringList) // TODO: 'ContextualKeyword as foreach variable identifier'
		{
			Console.WriteLine(item);
			count++; // stringList.Count is how to do this, just showcasing the foreach loop.
		}
		
		return count;
	}
	
	public void RemoveStringFromListInPlace(string stringToRemove, List<string> stringList)
	{
		for (int i = stringList.Count - 1; i >= 0; i--)
		{
			if (stringList[i] == stringToRemove)
				stringList.RemoveAt(i);
		}
		
		// Bubble up scope ending for nested "single statement body" code blocks.
		for (int i = 0; i < 5; i++)
			for (int q; q < 5; q++)
				for (int z; z < 5; z++)
					Console.WriteLine("Abc123");
	}
	
	public void ThrowException()
	{
		try
		{
			List<bool> aaa = new() { true, false };
			Console.WriteLine(aaa[2]);
		}
		catch (Exception e)
		{
			throw;
		}
		finally
		{
			Console.WriteLine($"The method: '{nameof(ThrowException)}' completed.}");
		}
		
		// Catch without capturing the variable reference.
		try
		{
			lock (_lock)
			{
				// TODO: Something
			}
		}
		catch (Exception)
		{
			throw;
		}
		
		while (false)
		{
			do
			{
				#if DEBUG
					throw new NotImplementedException();
				#endif
			} while(false);
		}
	}
}

// Value Tuple TypeClauseNode
(int, bool) myVariableOne;              // (not named)
(Apple, Banana) myVariableTwo;          // (not named)
(int Aaa, bool Bbb) myVariableThree;    // (is named)
(Apple Aaa, Banana Bbb) myVariableFour; // (is named)

// Value Tuple TypeClauseNode as a generic argument
List<(int, bool)> myListOne;              // (not named)
List<(Apple, Banana)> myListTwo;          // (not named)
List<(int Aaa, bool Bbb)> myListThree;    // (is named)
List<(Apple Aaa, Banana Bbb)> myListFour; // (is named)

// Explicit cast
(int)myVariableOne;                            // (no generic argument)
(Apple)myVariableOne;                          // (no generic argument)
(GenericParametersListingNode?)null;           // (nullable)
(List<int>)myVariableThree;                    // (with generic argument)
(List<Apple>)myVariableTwo;                    // (with generic argument)
(List<(int, bool)>)myVariableFour;             // (with generic argument, which is a tuple, not named)
(List<(Apple, Banana)>)myVariableTwo;          // (with generic argument, which is a tuple, not named)
(List<(int Aaa, bool Bbb)>)myVariableThree;    // (with generic argument, which is a tuple, is named)
(List<(Apple Aaa, Banana Bbb)>)myVariableFour; // (with generic argument, which is a tuple, is named)

return (aaa, bbb, ccc) => 2; // Lambda Function with implicitly typed parameters, expression body

return (int aaa, bool bbb, string ccc) => (aaa, bbb, ccc); // Lambda Function with explicitly typed parameters, expression body

return xasas => 3; // Lambda Function, parameters declared without parenthesis.

return async xasas => Task.CompletedTask; // Lambda Function, async.

return x =>
{
	return x;  // Lambda Function with statement block body.
};

public enum SomeKind
{
	Something,
	Anything,
	Nothing,
}

// Note how "normal" escape characters don't get escaped, instead the '"' does.
// There are two colors assigned to an escape character,
// they alternate if contiguous.
var verbatimString = @"\n""""""""
\t\r";

var normalEscapeCharacters = "\n\t\r";

// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/raw-string
var singleLine = """This is a "raw string literal". It can contain characters like \, ' and ".""";

var xml = """
        <element attr="content">
            <body>
            </body>
        </element>
        """;

var moreQuotes = """" As you can see,"""Raw string literals""" can start and end with more than three double-quotes when needed."""";

var MultiLineQuotes = """"
               """Raw string literals""" can start and end with more than three double-quotes when needed.
               """";
               
int X = 2;
int Y = 3;

var pointMessage = $"""The point "{X}, {Y}" is {Math.Sqrt(X * X + Y * Y):F3} from the origin""";

Console.WriteLine(pointMessage);
// Output is:
// The point "2, 3" is 3.606 from the origin

pointMessage = $$"""{The point {{{X}}, {{Y}}} is {{Math.Sqrt(X * X + Y * Y):F3}} from the origin}""";
Console.WriteLine(pointMessage);
// Output is:
// {The point {2, 3} is 3.606 from the origin}

/*
Not everything in this file works perfectly yet.
I am laying it all out so I see what is and isn't working.
As well, if something doesn't work, whether the parser can recover.

The WASM demo is painfully slow relative to running it natively
Presumably I use this most often natively which is multi-threaded,
and so the single threaded optimization wasn't on my mind enough.
I will look into the single threaded optimization, but
the C# parsing is for sure my #1 priority at the moment.
*/
""""""""";
}
