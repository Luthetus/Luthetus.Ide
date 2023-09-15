using Luthetus.Common.RazorLib.FileSystem.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.FileSystemCase.Displays;

public partial class FileIconDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public IAbsolutePath AbsolutePath { get; set; } = null!;
}