using Microsoft.AspNetCore.Components.Rendering;
using System.Text;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

public class StdIn : Std
{
    public StdIn(IntegratedTerminal integratedTerminal)
        : base(integratedTerminal)
    {
    }

    public override RenderTreeBuilder GetRenderTreeBuilder(RenderTreeBuilder builder, ref int sequence)
    {
        var workingDirectoryAbsolutePath = _integratedTerminal.EnvironmentProvider.AbsolutePathFactory(
            _integratedTerminal.WorkingDirectory,
            true);

        var showWorkingDirectory = workingDirectoryAbsolutePath.NameNoExtension;

        var parentDirectory = workingDirectoryAbsolutePath.ParentDirectory;

        if (parentDirectory is not null)
            showWorkingDirectory = parentDirectory.Value + showWorkingDirectory;

        builder.OpenElement(sequence++, "div");
        builder.AddAttribute(sequence++, "class", "luth_te_method");
        builder.AddContent(sequence++, $"{showWorkingDirectory}>");
        builder.CloseElement();

        return builder;
    }
}
