using Luthetus.Ide.ClassLib.WebsiteProjectTemplatesCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CSharpProjectForm;

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