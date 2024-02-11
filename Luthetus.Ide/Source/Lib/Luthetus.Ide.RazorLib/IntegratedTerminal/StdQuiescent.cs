using Microsoft.AspNetCore.Components.Rendering;

namespace Luthetus.Ide.RazorLib.IntegratedTerminal;

public class StdQuiescent : Std
{
    public StdQuiescent(IntegratedTerminal integratedTerminal)
        : base(integratedTerminal)
    {
    }

    public bool IsCompleted { get; set; }
    public string TargetFilePath { get; set; } = "\\Users\\hunte\\Repos\\Demos\\TestingCliWrap\\a.out";//"netcoredbg";
    public string Arguments { get; set; } = string.Empty;//"--interpreter=cli -- dotnet \\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg\\bin\\Debug\\net6.0\\BlazorApp4NetCoreDbg.dll";

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
