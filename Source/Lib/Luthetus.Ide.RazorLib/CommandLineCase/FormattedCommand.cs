using System.Text;

namespace Luthetus.Ide.ClassLib.CommandLineCase;

public class FormattedCommand
{
    public FormattedCommand(string targetFileName, IEnumerable<string> arguments)
    {
        TargetFileName = targetFileName;
        Arguments = arguments;
    }

    public string TargetFileName { get; }
    public IEnumerable<string> Arguments { get; }

    /// <summary>The command in string form.</summary>
    public string Value => FormattedCommandToStringHelper();

    private string FormattedCommandToStringHelper()
    {
        var interpolatedCommandBuilder = new StringBuilder(TargetFileName);

        foreach (var argument in Arguments)
        {
            interpolatedCommandBuilder.Append($" {argument}");
        }

        return interpolatedCommandBuilder.ToString();
    }
}
