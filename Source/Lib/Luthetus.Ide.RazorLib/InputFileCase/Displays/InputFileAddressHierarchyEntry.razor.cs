using Luthetus.Common.RazorLib.FileSystem.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.InputFileCase.Displays;

public partial class InputFileAddressHierarchyEntry : ComponentBase
{
    [Parameter, EditorRequired]
    public IAbsolutePath AbsolutePath { get; set; } = null!;
}