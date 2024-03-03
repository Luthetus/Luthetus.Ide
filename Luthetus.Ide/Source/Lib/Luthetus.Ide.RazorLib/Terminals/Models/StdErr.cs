using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class StdErr : Std
{
    public StdErr(IntegratedTerminal integratedTerminal)
        : base(integratedTerminal)
    {
    }

    public override StdKind StdKind => StdKind.StdErr;
}
