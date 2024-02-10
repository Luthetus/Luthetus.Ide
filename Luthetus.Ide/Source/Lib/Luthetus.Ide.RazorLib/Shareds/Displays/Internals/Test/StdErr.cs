using System.Text;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

public class StdErr : Std
{
    public StdErr(IntegratedTerminal integratedTerminal)
        : base(integratedTerminal)
    {
    }

    public override void Render(StringBuilder stringBuilder)
    {
    }
}
