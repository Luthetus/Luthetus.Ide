using Luthetus.Ide.RazorLib.KeymapCase.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.KeymapCase.Displays;

public partial class KeymapDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public Keymap Keymap { get; set; } = null!;
}