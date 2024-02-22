using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public abstract class Std
{
    protected readonly IntegratedTerminal _integratedTerminal;

    public Std(IntegratedTerminal integratedTerminal)
    {
        _integratedTerminal = integratedTerminal;
    }

    public abstract RenderTreeBuilder GetRenderTreeBuilder(RenderTreeBuilder builder, ref int sequence);
}
