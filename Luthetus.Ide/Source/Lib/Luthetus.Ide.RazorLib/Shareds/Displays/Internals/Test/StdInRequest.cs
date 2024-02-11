using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

public class StdInRequest : Std
{
    public StdInRequest(IntegratedTerminal integratedTerminal)
        : base(integratedTerminal)
    {
    }

    public override RenderTreeBuilder GetRenderTreeBuilder(RenderTreeBuilder builder, ref int sequence)
    {
        builder.OpenElement(sequence++, "input");
        builder.CloseElement();

        return builder;
    }
}
