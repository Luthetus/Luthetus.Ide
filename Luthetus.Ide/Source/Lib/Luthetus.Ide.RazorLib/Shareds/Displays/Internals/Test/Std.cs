using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

public abstract class Std
{
    protected readonly IntegratedTerminal _integratedTerminal;

    public Std(IntegratedTerminal integratedTerminal)
    {
        _integratedTerminal = integratedTerminal;
    }

    public abstract RenderTreeBuilder GetRenderTreeBuilder(RenderTreeBuilder builder, ref int sequence);
}
