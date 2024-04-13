namespace Luthetus.Common.RazorLib.Reflectives.PersonCase;

/// <summary>
/// <see cref="PersonModel"/> is used from within the unit tests,
/// in order to keep around an un-changing Type for the Reflectives.
/// </summary>
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
