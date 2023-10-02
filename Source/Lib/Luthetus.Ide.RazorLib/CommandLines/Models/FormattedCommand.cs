using System.Text;

namespace Luthetus.Ide.RazorLib.CommandLines.Models;

public class FormattedCommand
{
    public FormattedCommand(string targetFileName, IEnumerable<string> argumentsBag)
    {
        TargetFileName = targetFileName;
        ArgumentsBag = argumentsBag;
    }

    public string TargetFileName { get; }
    public IEnumerable<string> ArgumentsBag { get; }

    /// <summary>The command in string form.</summary>
    public string Value => FormattedCommandToStringHelper();

    private string FormattedCommandToStringHelper()
    {
        var interpolatedCommandBuilder = new StringBuilder(TargetFileName);

        foreach (var argument in ArgumentsBag)
        {
            interpolatedCommandBuilder.Append($" {argument}");
        }

        return interpolatedCommandBuilder.ToString();
    }
}
