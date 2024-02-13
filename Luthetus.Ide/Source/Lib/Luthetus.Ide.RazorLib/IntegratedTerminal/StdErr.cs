using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.RazorLib.IntegratedTerminal;

public class StdErr : Std
{
    public StdErr(IntegratedTerminal integratedTerminal)
        : base(integratedTerminal)
    {
    }

    public override RenderTreeBuilder GetRenderTreeBuilder(RenderTreeBuilder builder, ref int sequence)
    {
        return builder;
    }
}
