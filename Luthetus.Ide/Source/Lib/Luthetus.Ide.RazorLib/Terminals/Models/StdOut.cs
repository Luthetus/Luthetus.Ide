namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class StdOut : Std
{
    public StdOut(IntegratedTerminal integratedTerminal, string initialContent, StdOutKind stdOutKind)
        : base(integratedTerminal)
    {
        Content = initialContent;
        StdOutKind = stdOutKind;
    }

    public string Content { get; internal set; }
    public StdOutKind StdOutKind { get; internal set; }
    public override StdKind StdKind => StdKind.StdOut;
}
