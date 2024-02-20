using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

public abstract class StdTests
{
    protected readonly IntegratedTerminal _integratedTerminal;

    public Std(IntegratedTerminal integratedTerminal)
    {
        _integratedTerminal = integratedTerminal;
    }

    public abstract RenderTreeBuilder GetRenderTreeBuilder(RenderTreeBuilder builder, ref int sequence);
}
