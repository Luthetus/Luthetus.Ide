using Microsoft.AspNetCore.Components.Rendering;
using System.Text;

namespace Luthetus.Ide.RazorLib.IntegratedTerminal;

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

    public override RenderTreeBuilder GetRenderTreeBuilder(RenderTreeBuilder builder, ref int sequence)
    {
        builder.OpenElement(sequence++, "div");
        {
            if (StdOutKind == StdOutKind.Started || StdOutKind == StdOutKind.Exited)
                builder.AddAttribute(sequence++, "class", "luth_te_keyword");
            if (StdOutKind == StdOutKind.Error)
                builder.AddAttribute(sequence++, "class", "luth_te_keyword-contextual");

            var stringReader = new StringReader(Content);
            string? line;
            while ((line = stringReader.ReadLine()) is not null)
            {
                builder.AddContent(sequence++, line);
                builder.OpenElement(sequence++, "br");
                builder.CloseElement();
            }
        }
        builder.CloseElement();

        return builder;
    }
}
