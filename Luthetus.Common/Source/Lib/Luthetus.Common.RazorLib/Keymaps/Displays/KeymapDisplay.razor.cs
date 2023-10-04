using Luthetus.Common.RazorLib.Keymaps.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Keymaps.Displays;

public partial class KeymapDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public Keymap Keymap { get; set; } = null!;
}