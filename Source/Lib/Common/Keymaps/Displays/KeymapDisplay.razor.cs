using Fluxor;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keymaps.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Keymaps.Displays;

public partial class KeymapDisplay : ComponentBase
{
    [Inject]
    private IState<KeymapState> KeymapStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public IKeymap Keymap { get; set; } = null!;
}