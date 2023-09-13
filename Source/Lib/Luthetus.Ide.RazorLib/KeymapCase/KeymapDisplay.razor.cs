using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.KeymapCase;

public partial class KeymapDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public Keymap Keymap { get; set; } = null!;
}