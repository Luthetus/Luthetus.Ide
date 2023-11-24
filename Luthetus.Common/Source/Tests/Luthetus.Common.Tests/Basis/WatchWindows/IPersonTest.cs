namespace Luthetus.Common.Tests.Basis.WatchWindows;

public interface IPersonTest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<IPersonTest> Relatives { get; set; }

    public string DisplayName { get; }
}
