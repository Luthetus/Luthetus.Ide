using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.FileSystems.Displays;

public partial class FileIconDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public IAbsolutePath AbsolutePath { get; set; } = null!;
}