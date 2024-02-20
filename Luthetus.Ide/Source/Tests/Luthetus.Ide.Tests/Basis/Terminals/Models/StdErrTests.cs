using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

public class StdErrTests
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
