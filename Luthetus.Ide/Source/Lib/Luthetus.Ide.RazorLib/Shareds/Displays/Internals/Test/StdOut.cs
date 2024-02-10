using System.Text;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

public class StdOut : Std
{
    public StdOut(IntegratedTerminal integratedTerminal, string initialContent)
        : base(integratedTerminal)
    {
        Content = initialContent;
    }

    public string Content { get; internal set; }

    public override void Render(StringBuilder stringBuilder)
    {
        stringBuilder.Append(Content);
    }
}
