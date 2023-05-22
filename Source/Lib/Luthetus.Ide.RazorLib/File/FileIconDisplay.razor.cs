using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.File;

public partial class FileIconDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public IAbsoluteFilePath AbsoluteFilePath { get; set; } = null!;
}