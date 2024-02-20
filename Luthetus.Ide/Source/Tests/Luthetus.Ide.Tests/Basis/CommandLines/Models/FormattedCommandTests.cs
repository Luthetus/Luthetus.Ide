using System.Text;

namespace Luthetus.Ide.Tests.Basis.CommandLines.Models;

public class FormattedCommandTests
{
    public FormattedCommand(string targetFileName, IEnumerable<string> argumentsList)
    {
        TargetFileName = targetFileName;
        ArgumentsList = argumentsList;
    }

    public string TargetFileName { get; }
    public IEnumerable<string> ArgumentsList { get; }

    /// <summary>The command in string form.</summary>
    public string Value => FormattedCommandToStringHelper();

    private string FormattedCommandToStringHelper()
    {
        var interpolatedCommandBuilder = new StringBuilder(TargetFileName);

        foreach (var argument in ArgumentsList)
        {
            interpolatedCommandBuilder.Append($" {argument}");
        }

        return interpolatedCommandBuilder.ToString();
    }
}
