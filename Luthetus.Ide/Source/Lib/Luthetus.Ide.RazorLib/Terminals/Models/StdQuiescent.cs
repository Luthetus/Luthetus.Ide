namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class StdQuiescent : Std
{
    public StdQuiescent(IntegratedTerminal integratedTerminal)
        : base(integratedTerminal)
    {
        TargetFilePath = integratedTerminal.TargetFilePath;
        Arguments = integratedTerminal.Arguments;
    }

    public bool IsCompleted { get; set; }
    public string TargetFilePath { get; set; }
    public string Arguments { get; set; }
    public string Text { get; set; } = string.Empty;
    public override StdKind StdKind => StdKind.StdQuiescent;
}
