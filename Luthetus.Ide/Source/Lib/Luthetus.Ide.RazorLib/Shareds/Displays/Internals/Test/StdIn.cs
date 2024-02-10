using System.Text;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

public class StdIn : Std
{
    public StdIn(IntegratedTerminal integratedTerminal)
        : base(integratedTerminal)
    {
    }

    public override void Render(StringBuilder stringBuilder)
    {
        var workingDirectoryAbsolutePath = _integratedTerminal.EnvironmentProvider.AbsolutePathFactory(
            _integratedTerminal.WorkingDirectory,
            true);

        var showWorkingDirectory = workingDirectoryAbsolutePath.NameNoExtension;

        var parentDirectory = workingDirectoryAbsolutePath.ParentDirectory;

        if (parentDirectory is not null)
            showWorkingDirectory = parentDirectory.Value + showWorkingDirectory;

        stringBuilder.Append($"{showWorkingDirectory}>");
    }
}
