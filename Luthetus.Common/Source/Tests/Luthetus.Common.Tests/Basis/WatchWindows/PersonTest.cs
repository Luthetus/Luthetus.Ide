using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.Tests.Basis.WatchWindows;

public class PersonTest
{
    public PersonTest(string firstName, string lastName, List<PersonTest> relatives)
    {
        FirstName = firstName;
        LastName = lastName;
        Relatives = relatives;
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<PersonTest> Relatives { get; set; } = new();

    public string DisplayName => $"{FirstName} {LastName}";
}
