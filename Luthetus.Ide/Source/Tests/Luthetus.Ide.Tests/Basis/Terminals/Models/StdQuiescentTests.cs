using Luthetus.Ide.RazorLib.Terminals.Displays;
using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

public class StdQuiescentTests
{
    public StdQuiescent(IntegratedTerminal integratedTerminal)
        : base(integratedTerminal)
    {
        TargetFilePath = integratedTerminal.TargetFilePath;
        Arguments = integratedTerminal.Arguments;
    }

    public bool IsCompleted { get; set; }
    public string TargetFilePath { get; set; }
    public string Arguments { get; set; }
    public string Text { get; set; } = string.Empty;

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
        {
            builder.OpenElement(sequence++, "span");
            {
                builder.AddAttribute(sequence++, "class", "luth_te_method");
                builder.AddContent(sequence++, $"{showWorkingDirectory}>");
            }
            builder.CloseElement();

            builder.OpenComponent<StdQuiescentInputDisplay>(sequence++);
            builder.AddAttribute(sequence++, nameof(StdQuiescentInputDisplay.IntegratedTerminal), _integratedTerminal);
            builder.AddAttribute(sequence++, nameof(StdQuiescentInputDisplay.StdQuiescent), this);
            builder.CloseComponent();
        }
        builder.CloseElement();

        return builder;
    }
}
