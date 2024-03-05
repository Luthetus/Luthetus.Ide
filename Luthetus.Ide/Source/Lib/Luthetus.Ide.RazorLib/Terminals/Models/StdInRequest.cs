namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class StdInRequest : Std
{
    public StdInRequest(IntegratedTerminal integratedTerminal)
        : base(integratedTerminal)
    {
    }

    public bool IsCompleted { get; set; }
    public string Value { get; set; } = string.Empty;
    public override StdKind StdKind => StdKind.StdInRequest;
}
