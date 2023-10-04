namespace Luthetus.Common.RazorLib.ComponentRunners.Internals.PersonCase;

public class PersonModel
{
    public PersonModel(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string DisplayName => $"{FirstName} {LastName}";
}
