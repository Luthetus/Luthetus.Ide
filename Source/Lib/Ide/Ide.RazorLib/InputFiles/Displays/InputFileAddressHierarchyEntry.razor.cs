using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Displays;

public partial class InputFileAddressHierarchyEntry : ComponentBase
{
    [Parameter, EditorRequired]
    public AbsolutePath AbsolutePath { get; set; }
}