using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.ShouldRenderBoundaries.Displays;

/// <summary>
/// This component sections off child content that should not receive a cascading StateHasChanged from their parent. (2023-09-24)
/// </summary>
public partial class ShouldRenderBoundary : ComponentBase
{
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    protected override bool ShouldRender() => false;
}