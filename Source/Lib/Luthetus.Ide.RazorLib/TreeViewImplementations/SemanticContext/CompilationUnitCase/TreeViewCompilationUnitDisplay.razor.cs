using Microsoft.AspNetCore.Components;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.SemanticContext.CompilationUnitCase;

public partial class TreeViewCompilationUnitDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public CodeBlockNode CodeBlockNode { get; set; } = null!;
}