using Microsoft.AspNetCore.Components;
using Luthetus.CompilerServices.RazorLib.Websites.ProjectTemplates.Models;

namespace Luthetus.CompilerServices.RazorLib.CSharpProjects.Displays;

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