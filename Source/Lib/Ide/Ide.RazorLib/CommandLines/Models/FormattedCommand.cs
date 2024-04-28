using System.Text;

namespace Luthetus.Ide.RazorLib.CommandLines.Models;

public class FormattedCommand
{
    public FormattedCommand(string targetFileName, IEnumerable<string> argumentsList)
    {
        TargetFileName = targetFileName;
        ArgumentsList = argumentsList;
    }

    public string TargetFileName { get; }
    public IEnumerable<string> ArgumentsList { get; }

    /// <summary>
    /// This property is being used tentatively during the development of an
    /// integrated terminal.<br/><br/>
    /// 
    /// This allows for the terminal to be usable without lexing the arguments
    /// individually, but instead as just a raw string that is passed along.<br/><br/>
    /// 
    /// (2024-04-07)
    /// </summary>
    public string? HACK_ArgumentsString { get; set; }

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
