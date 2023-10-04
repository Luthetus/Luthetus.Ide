using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.InputFiles.Displays;

public partial class InputFileAddressHierarchyEntry : ComponentBase
{
    [Parameter, EditorRequired]
    public IAbsolutePath AbsolutePath { get; set; } = null!;
}