using Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CSharpProjectFormCase.Displays;

public partial class ProjectTemplateDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ProjectTemplate ProjectTemplate { get; set; } = null!;
    [Parameter, EditorRequired]
    public string ShortNameOfSelectedProjectTemplate { get; set; } = null!;
    [Parameter, EditorRequired]
    public EventCallback<ProjectTemplate> OnProjectTemplateSelectedEventCallback { get; set; }

    private string IsActiveCssClass => ShortNameOfSelectedProjectTemplate == ProjectTemplate.ShortName
        ? "luth_active"
        : "";
}