using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reactives.Displays;

public partial class ThreadDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public Thread? Thread { get; set; }
    [Parameter, EditorRequired]
    public string DisplayName { get; set; } = null!;
    [Parameter, EditorRequired]
    public (int Id, DateTime DateTime) DateTimeTuple { get; set; } = (-1, DateTime.MinValue);
    [Parameter, EditorRequired]
    public Task LoadingIconTask { get; set; } = null!;
}