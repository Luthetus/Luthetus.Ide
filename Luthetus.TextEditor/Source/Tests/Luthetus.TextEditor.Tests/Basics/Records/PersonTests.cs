using Xunit;

namespace Luthetus.TextEditor.Tests.Basics.Records;

public class PersonTests
{
    /// <summary>
    /// Working on a bug relating to an infinite render loop.
    /// Needed to make this test for my own sanity.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void RECORD_SHOULD_COPY_NON_INIT_PROPERTIES()
    {
        var person = new PersonRecord(
            Guid.NewGuid(),
            "Abcdefghijklmnopqrstuvwxyz",
            "Hunter",
            "Freeman");

        person = person with
        {
            Description = "A changed description"
        };

        Assert.Equal("Hunter", person.FirstName);

        person.FirstName = "John";

        person = person with
        {
            LastName = "Doe"
        };

        Assert.Equal("John", person.FirstName);
        Assert.Equal("Doe", person.LastName);
    }
}
