using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CSharpProjectForm;

public partial class ProjectTemplateDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ProjectTemplate ProjectTemplate { get; set; } = null!;
}