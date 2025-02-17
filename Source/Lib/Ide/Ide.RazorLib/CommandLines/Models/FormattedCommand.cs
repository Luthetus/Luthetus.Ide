using System.Text;

namespace Luthetus.Ide.RazorLib.CommandLines.Models;

public class FormattedCommand
{
    public FormattedCommand(string targetFileName, IReadOnlyList<string> argumentsList)
    {
        TargetFileName = targetFileName;
        ArgumentsList = argumentsList;
    }

    public string TargetFileName { get; }
    public IReadOnlyList<string> ArgumentsList { get; }

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
    /// <summary>
    /// It is uncertain whether this tag property is a good idea.
    /// It allows one to easily re-use an <see cref="Terminals.Models.IOutputParser"/>.<br/><br/>
    /// 
    /// By, the output parser being provided the <see cref="Terminals.Models.TerminalCommand"/>
    /// which caused the output.<br/><br/>
    /// 
    /// But, determining the command that was ran would require parsing the <see cref="HACK_ArgumentsString"/>,
    /// because even though <see cref="TargetFileName"/> would be for example "git",
    /// one still doesn't know if it was "git add" or "git commit" etc...<br/><br/>
    /// 
    /// Therefore this <see cref="Tag"/> can be used to identify more specifically what command was ran,
    /// allowing the <see cref="Terminals.Models.IOutputParser"/> to parse accordingly.<br/><br/>
    /// 
    /// (2024-05-20)
    /// </summary>
    public string Tag { get; set; } = string.Empty;

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
