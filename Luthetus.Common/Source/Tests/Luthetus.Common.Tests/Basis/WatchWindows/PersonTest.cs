namespace Luthetus.Common.Tests.Basis.WatchWindows;

public class PersonTest : IPersonTest
{
    public PersonTest(string firstName, string lastName, List<IPersonTest> relatives)
    {
        FirstName = firstName;
        LastName = lastName;
        Relatives = relatives;

        // Supress unused field warning
        _ = Id;
    }

    public Guid Id = Guid.NewGuid();

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<IPersonTest> Relatives { get; set; } = new();

    public string DisplayName => $"{FirstName} {LastName}";
}
