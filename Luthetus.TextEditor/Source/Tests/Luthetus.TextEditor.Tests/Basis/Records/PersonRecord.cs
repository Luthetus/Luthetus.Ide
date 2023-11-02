namespace Luthetus.TextEditor.Tests.Basics.Records;

internal record PersonRecord
{
    public PersonRecord(
        Guid id,
        string description,
        string firstName,
        string lastName)
    {
        Id = id;
        Description = description;
        FirstName = firstName;
        LastName = lastName;
    }

    public Guid Id { get; init; }
    public string Description { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
