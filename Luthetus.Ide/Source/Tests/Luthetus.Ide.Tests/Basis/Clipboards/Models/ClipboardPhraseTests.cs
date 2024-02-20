namespace Luthetus.Ide.Tests.Basis.Clipboards.Models;

public class ClipboardPhraseTests
{
    public ClipboardPhrase(string command, string dataType, string value)
    {
        Command = command;
        DataType = dataType;
        Value = value;
    }

    public string Command { get; set; }
    public string DataType { get; set; }
    public string Value { get; set; }
}