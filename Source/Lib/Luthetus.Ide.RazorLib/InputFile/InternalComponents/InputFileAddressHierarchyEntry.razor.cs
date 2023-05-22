using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.InputFile.InternalComponents;

public partial class InputFileAddressHierarchyEntry : ComponentBase
{
    [Parameter, EditorRequired]
    public IAbsoluteFilePath AbsoluteFilePath { get; set; } = null!;
}